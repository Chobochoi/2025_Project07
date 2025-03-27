using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager2D : MonoBehaviour
{
    public static CanvasManager2D Instance { get; private set; }
    
    [Header ("Default Settings")]
    [SerializeField] public Transform cameraTransform;
    [SerializeField] public Transform playerTransform;

    [Header("Button List")]
    [SerializeField] private List<Button> buttons = new List<Button>();

    [Header("Target List")]
    [SerializeField] private List<Transform> targetPositions = new List<Transform>();    

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
    }    

    // 이동가능하게 할 버튼들을 등록하기
    private void RegisterButtons()
    {
        // 버튼 초기화
        buttons.Clear();              

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

        cameraTransform = targetPositions[index];
        playerTransform = targetPositions[index];

        if (playerTransform != null)
        {
            playerTransform.position = targetPositions[index].position;
        }

        if (cameraTransform != null)
        {
            cameraTransform.position = targetPositions[index].position;
        }

        Debug.Log($"플레이어가 {targetPositions[index]} 위치로 이동함");

        StartCoroutine(LoadImageDuringMoveToTarget());            
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
            Debug.Log($"{Mathf.RoundToInt(percent * 100)}");

            yield return null;
        }

        loadingImage.SetActive(false);
    }    
}
