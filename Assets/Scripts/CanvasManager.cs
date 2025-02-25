using ControllerManager;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class CanvasManager : MonoBehaviour
{
    // Instance 추가하여 다른곳에서 참조하여 사용할 수 있게 함.
    public static CanvasManager Instance { get; private set; }
    
    [Header("Camera Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] public Transform cameraTransform;

    // 하위 캔버스를 통합 관리하기 위한 스크립트
    [Header("UI Elements")]
    public List<GameObject> movePoints = new List<GameObject>();    // 모든 UI 오브젝트 저장
    public List<Button> buttons = new List<Button>();               // 버튼만 저장
          
    public List<Vector3> movePositions = new List<Vector3>();
    public static GameObject currentMovePoint;                      // 현재 활성화된 Movepoint 이동 전 저장을 위함
    public int previousIndex = -1;                                  // 이전 Index 저장하기 위함
    public float activationDistance = 2.0f;                         // 거리차이를 이용하여 주변 Button 활성화
    private Vector2 buttonSize = new Vector2(150, 150);             // Button Size 한 번에 관리를 위함

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        RegisterUIElements();
        RegisterButtons();
        MovePointChecker();
    }

    public void Start()
    {
        ButtonSetRectTransform();
    }

    private void Update()
    {
        ButtonLookAtCamera();
        ButtonActiveToDistance();
    }   

    // Canvas 하위의 오브젝트들을 타입별로 호출
    private void RegisterUIElements()
    {
        movePoints.Clear();
        buttons.Clear();
        
        foreach (Transform child in transform) // Canvas 하위 UI 탐색
        {
            movePoints.Add(child.gameObject); // 모든 UI 요소 저장            
        }                     
    }

    public void RegisterButtons()
    {
        buttons.Clear();
        
        foreach (Button button in GetComponentsInChildren<Button>(true)) // 비활성화된 버튼도 포함
        {
            buttons.Add(button);
        }

        for (int i = 0; i < buttons.Count; i++)
        {
            int tempIndex = i;
            Debug.Log(tempIndex);
            buttons[i].onClick.RemoveAllListeners(); // 혹시 모를 중복 이벤트 제거
            buttons[i].onClick.AddListener(() => OnButtonClick(tempIndex)); // 클릭 이벤트 추가
            Debug.Log($"버튼 {buttons[i].name}에 클릭 이벤트 추가됨.");

            movePositions.Add((buttons[i].transform.position));
        }
    }      

    // 모든 MovePoint의 MeshRender를 비활성화하고
    // 시작점의 MovePoint MeshRender만 활성화 및 current에 저장 
    private void MovePointChecker()
    {
        foreach (GameObject movePoint in movePoints)
        {
            movePoint.GetComponent<MeshRenderer>().enabled = false;  // GameObject내의 MeshRender Visible만 끔
        }

        // 시작하기 위한 Setting
        previousIndex = 0;
        movePoints[previousIndex].GetComponent<MeshRenderer>().enabled = true;
    }

    public void OnButtonClick(int index)
    {
        Debug.Log($" 버튼 {index} 클릭됨!");
        Debug.Log($" movePositions.Count: {movePoints.Count}");
                
        if (index >= movePoints.Count)  // 인덱스 초과 방지 체크
        {
            Debug.LogError($"IndexOutOfRangeException 발생! index({index})가 movePositions.Count({movePoints.Count})보다 큼");
            return; // 이동 로직 실행 방지
        }        

        if (PlayerController.Instance != null)
        {
            if(previousIndex != -1)
            {
                Debug.Log($"이동할 위치: {movePositions[index]}");
                PlayerController.Instance.MoveToPosition(movePositions[index], () => OnMoveComplete(index)); //  클릭된 버튼에 해당하는 위치로 이동                             
            }
        }
    }
    
    // 코루틴이 끝나고 함수 실행을 하기위해 PlayerController.cs에 Action을 추가한 후 Invoke 호출
    public void OnMoveComplete(int index)
    {
        if(previousIndex != -1)
        {
            // 이전의 movePoints Renderer 비활성화
            movePoints[previousIndex].GetComponent<MeshRenderer>().enabled = false;

            // 2D Canvas로 화면 전환 시 Renderer가 꺼지지 않는 현상 수정을 위해 추가한 코드
            if (previousIndex != CanvasManager2D.Instance.previousIndex)
            {
                movePoints[CanvasManager2D.Instance.previousIndex].GetComponent<MeshRenderer>().enabled = false;
            }

            // 클릭된 movePoints Renderer 활성화
            movePoints[index].GetComponent<MeshRenderer>().enabled = true;

            previousIndex = index;
        }
    }
    
    // 버튼이 카메라를 바라보게 하기 위함
    public void ButtonLookAtCamera()
    {
        if (cameraTransform == null) return;
        
        cameraTransform = Camera.main.transform;
            
        foreach (var button in buttons)
        {
            button.transform.LookAt(cameraTransform);
            button.transform.Rotate(0, 180, 0);
        }
        
    }

    // 카메라와 버튼의 거리사이를 측정하여 일정거리 이상일때만 Active 되게 함
    // 구와 구사이의 거리 2.0f
    // activationDistance 넉넉잡아 2.5f로 해두었음.
    public void ButtonActiveToDistance()
    {
        if (cameraTransform == null) return;

        Vector3 playerPos = cameraTransform.position;

        foreach (Button button in buttons)
        {
            float distance = Vector3.Distance(playerPos, button.transform.position);
            button.gameObject.SetActive(distance <= activationDistance);
        }
    }

    // 하위에서 관리하는 모든 Button의 크기를 관리하기 위함
    public void ButtonSetRectTransform()
    {
        foreach (Button button in GetComponentsInChildren<Button>(true))
        {
            RectTransform rectTransform = button.GetComponent<RectTransform>();

            if (rectTransform != null)
            {
                rectTransform.sizeDelta = buttonSize;
            }
        }               
    }
}
