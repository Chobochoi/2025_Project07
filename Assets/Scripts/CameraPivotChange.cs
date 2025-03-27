using System.Collections;
using UnityEngine;
using UnityEngine.UI; // UI 요소를 사용하기 위한 네임스페이스  

public class CameraPivotChange : MonoBehaviour
{
    // 이동할 3개 구역의 위치와 회전값  
    [System.Serializable]
    public class CameraPosition
    {
        public Vector3 position;
        public Vector3 rotation;
        public string areaName; // 구역 이름 (디버깅용)  
    }

    // 인스펙터에서 설정 가능한 카메라 위치들  
    public CameraPosition[] cameraPositions = new CameraPosition[3];

    // 이동 속도 설정  
    public float moveSpeed = 2.0f;

    // 현재 선택된 위치 인덱스  
    private int currentPositionIndex = 0;

    // 현재 이동 중인지 확인하는 변수  
    private bool isMoving = false;

    // 버튼 참조  
    public Button switchButton;

    // 타겟이 될 카메라 (null이면 이 스크립트가 붙은 객체 사용)  
    public Camera targetCamera;

    private void Start()
    {
        // 카메라 참조 설정  
        if (targetCamera == null)
        {
            targetCamera = GetComponent<Camera>();
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }
        }

        // 버튼 리스너 설정  
        if (switchButton != null)
        {
            switchButton.onClick.AddListener(SwitchToNextArea);
        }

        // 초기 위치 설정  
        if (cameraPositions.Length > 0)
        {
            // 인스펙터에서 값이 설정되지 않았다면 기본값 설정  
            if (cameraPositions[0] == null)
                InitializeDefaultPositions();

            // 시작 시 첫 번째 위치로 즉시 이동  
            SetCameraPosition(0, false); // false = 바로 이동 (Lerp 없이)  
        }
    }

    private void InitializeDefaultPositions()
    {
        // 기본 카메라 위치 세팅 (기본값 예시)  
        for (int i = 0; i < 3; i++)
        {
            cameraPositions[i] = new CameraPosition();
        }

        // 구역 1: 정면 
        // 현재 0, 25, 6 / 90, 0 , 0        
        cameraPositions[0].position = new Vector3(0, 25, 6);
        cameraPositions[0].rotation = new Vector3(90, 0, 0);
        cameraPositions[0].areaName = "정면 구역";

        // 구역 2: 좌측  
        // 현재 -20, 0, 5 / 0, 90, 0
        cameraPositions[1].position = new Vector3(-20, 0, 5);
        cameraPositions[1].rotation = new Vector3(00, 90, 0);
        cameraPositions[1].areaName = "좌측 구역";

        // 구역 3: 우측 높이  
        // 현재 0, 0, -12 / 0, 0, 0
        cameraPositions[2].position = new Vector3(0, 0, -12);
        cameraPositions[2].rotation = new Vector3(0, 0, 0);
        cameraPositions[2].areaName = "우측 높은 구역";
    }

    // 버튼 클릭 시 호출될 함수  
    public void SwitchToNextArea()
    {
        if (isMoving)
            return; // 이미 이동 중이면 무시  

        // 다음 인덱스로 순환 (0->1->2->0)  
        currentPositionIndex = (currentPositionIndex + 1) % cameraPositions.Length;

        // 해당 위치로 이동 시작  
        SetCameraPosition(currentPositionIndex, true); // true = Lerp로 부드럽게 이동  
    }

    // 특정 인덱스 위치로 이동  
    public void SetCameraPosition(int index, bool useLerp)
    {
        if (index < 0 || index >= cameraPositions.Length)
            return;

        if (useLerp)
        {
            // 코루틴으로 부드럽게 이동  
            StartCoroutine(MoveCameraCoroutine(index));
        }
        else
        {
            // 즉시 이동  
            MoveCameraImmediate(index);
        }
    }

    // 즉시 이동 함수  
    private void MoveCameraImmediate(int index)
    {
        Transform camTransform = targetCamera.transform;
        camTransform.position = cameraPositions[index].position;
        camTransform.eulerAngles = cameraPositions[index].rotation;

        Debug.Log("카메라가 즉시 이동: " + cameraPositions[index].areaName);
    }

    // 부드러운 이동을 위한 코루틴  
    private IEnumerator MoveCameraCoroutine(int index)
    {
        isMoving = true;

        Transform camTransform = targetCamera.transform;
        Vector3 startPosition = camTransform.position;
        Quaternion startRotation = camTransform.rotation;

        Vector3 targetPosition = cameraPositions[index].position;
        Quaternion targetRotation = Quaternion.Euler(cameraPositions[index].rotation);

        float timeElapsed = 0;

        Debug.Log("카메라 이동 시작: " + cameraPositions[index].areaName);

        while (timeElapsed < 1.0f)
        {
            // 실제 보간 비율 계산 (0에서 1 사이의 값)  
            float t = timeElapsed / (1.0f / moveSpeed);

            // 위치와 회전을 부드럽게 변경  
            camTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
            camTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // 정확한 최종 위치로 설정  
        camTransform.position = targetPosition;
        camTransform.rotation = targetRotation;

        Debug.Log("카메라 이동 완료: " + cameraPositions[index].areaName);

        isMoving = false;
    }
}