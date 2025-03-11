using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager2D : MonoBehaviour
{
    public static CanvasManager2D Instance { get; private set; }
    
    [Header ("Default Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform playerTransform;

    [Header("Button List")]
    [SerializeField] private List<Button> buttons = new List<Button>();

    [Header("Target List")]
    [SerializeField] private List<Vector3> targetPositions = new List<Vector3>();

    public int previousIndex;
    public List<GameObject> movePoints;

    [Header("LoadIMG Settings")]
    [SerializeField] private GameObject loadingImage;
    [SerializeField] private Slider loadingSlider;

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

        // 시작 전 로딩화면 비활성화
        loadingImage.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        RegisterButtons();
        int previousIndex = CanvasManager.Instance.previousIndex;
        movePoints = CanvasManager.Instance.movePoints;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 이동가능하게 할 버튼들을 등록하기
    private void RegisterButtons()
    {
        // 버튼 초기화
        buttons.Clear();
        
        foreach (Button button in GetComponentsInChildren<Button>(true)) 
        {
            buttons.Add(button);
        }

        for (int i = 0; i < buttons.Count; i++)
        {
            int tempIndex = i;
            //Debug.Log(tempIndex);
            buttons[i].onClick.RemoveAllListeners(); // 혹시 모를 중복 이벤트 제거
            buttons[i].onClick.AddListener(() => MoveToTargetTransform(tempIndex)); // 클릭 이벤트 추가
            Debug.Log($"버튼 {buttons[i].name}에 클릭 이벤트 추가됨.");
        }
    }

    public void MoveToTargetTransform(int index)
    {
        if (index < 0 || index >= targetPositions.Count) return;
        
        playerTransform.position = targetPositions[index];
        cameraTransform.position = targetPositions[index];
        Debug.Log($"플레이어가 {targetPositions[index]} 위치로 이동함");

        StartCoroutine(LoadImageDuringMoveToTarget());

        // 이동 버튼 클릭 시에 비/활성화되는 MovePoints 관리를 위함
        if (Vector3.Distance(targetPositions[index], CanvasManager.Instance.movePositions[0]) < 0.01f)
        {
            for (int i = 0; movePoints.Count > i; i++)
            {
                movePoints[i].GetComponent<MeshRenderer>().enabled = false;
            }

            previousIndex = 0;
            movePoints[previousIndex].GetComponent<MeshRenderer>().enabled = true;
        }

        if (Vector3.Distance(targetPositions[index], CanvasManager.Instance.movePositions[9]) < 0.01f)
        {
            for (int i = 0; movePoints.Count > i; i++)
            {
                movePoints[i].GetComponent<MeshRenderer>().enabled = false;
            }

            previousIndex = 9;
            movePoints[previousIndex].GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public IEnumerator LoadImageDuringMoveToTarget()
    {
        loadingImage.SetActive(true);
        float percent = 0;
        float loadingTime = 1.5f;
        
        while (percent < 1.0f)
        {
            percent += Time.deltaTime / loadingTime;
            loadingSlider.value = percent;
            //Debug.Log($"{Mathf.RoundToInt(percent * 100)}");

            yield return null;
        }

        loadingImage.SetActive(false);
    }
}
