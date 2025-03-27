using System.Collections;
using UnityEngine;

public class CameraPositionManager : MonoBehaviour
{
    [System.Serializable]
    public class HotspotPosition
    {
        public string name;
        public Vector3 position;
        public Vector3 rotation;
        [TextArea(1, 3)]
        public string description; // 해당 핫스팟 설명  
    }

    [Header("카메라 설정")]
    public Camera mainCamera;
    public float transitionSpeed = 2.0f;

    [Header("핫스팟 위치 목록")]
    public HotspotPosition[] positions;

    private bool isMoving = false;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // 초기 위치 설정 (필요한 경우)  
        if (positions.Length > 0)
        {
            SetCameraPosition(0, false);
        }
    }

    // 인덱스로 위치 이동  
    public void MoveToPosition(int index)
    {
        if (index < 0 || index >= positions.Length || isMoving)
            return;

        SetCameraPosition(index, true);
    }

    private void SetCameraPosition(int index, bool useLerp)
    {
        if (useLerp)
        {
            StartCoroutine(MoveCameraCoroutine(positions[index]));
        }
        else
        {
            Transform camTransform = mainCamera.transform;
            camTransform.position = positions[index].position;
            camTransform.eulerAngles = positions[index].rotation;
        }
    }

    private IEnumerator MoveCameraCoroutine(HotspotPosition targetPosition)
    {
        isMoving = true;

        Transform camTransform = mainCamera.transform;
        Vector3 startPosition = camTransform.position;
        Quaternion startRotation = camTransform.rotation;

        Vector3 targetPos = targetPosition.position;
        Quaternion targetRot = Quaternion.Euler(targetPosition.rotation);

        float timeElapsed = 0;

        while (timeElapsed < 1.0f)
        {
            float t = timeElapsed / (1.0f / transitionSpeed);

            // 부드러운 이동 곡선 적용  
            float smoothT = Mathf.SmoothStep(0, 1, t);

            camTransform.position = Vector3.Lerp(startPosition, targetPos, smoothT);
            camTransform.rotation = Quaternion.Slerp(startRotation, targetRot, smoothT);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // 정확한 위치로 마무리  
        camTransform.position = targetPos;
        camTransform.rotation = targetRot;

        isMoving = false;
    }
}