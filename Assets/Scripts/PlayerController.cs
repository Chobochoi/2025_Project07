using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

//using UnityEngine.UIElements;


namespace ControllerManager
{ 
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }
        
        [Header("Camera Settings")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float maxLookAngle = 80f;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 1.0f;
        //[SerializeField] private GameObject[] movePoints; 

        [Header("Zoom Settings (FOV)")]
        [SerializeField] private float zoomSpeed = 3f;
        [SerializeField] private float minFOV = 30f; // 최소 FOV (줌 인)
        [SerializeField] private float maxFOV = 60f; // 최대 FOV (줌 아웃)
        private float targetFOV;

        private bool isMoving = false;
        private float yaw = 0f, pitch = 0f;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {     
            // 마우스 커서 보이게 하기.
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;                      

            targetFOV = 60f; // 처음 FOV 60으로 Set           
        }

        private void Update()
        {
            HandleMouseRotation();
            HandleMouseClickMovement();
            HandleMouseZoom();
        }

        #region Player의 마우스 Input 모음
        // 마우스 우클릭 회전
        private void HandleMouseRotation()
        {
            if (Input.GetMouseButton(1)) // 우클릭 드래그
            {
                yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
                pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
                pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

                cameraTransform.rotation = Quaternion.Euler(pitch, yaw, 0f);
            }
        }
        private void HandleMouseZoom()
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollInput != 0f)
            {
                targetFOV -= scrollInput * zoomSpeed;
                targetFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);
            }

            // 부드러운 줌 적용 (Lerp 사용)
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
        }

        // 마우스 좌클릭 이동 & MovePoint 활성화
        private void HandleMouseClickMovement()
        {
            if (isMoving) return; // 이동 중이면 입력 무시

            if (Input.GetMouseButtonDown(0))
            {
                if (IsPointerOverUIObject())
                {
                    Debug.Log("2D Canvas 감지함");
                    return;
                }
                
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                int movePointLayerMask = LayerMask.GetMask("Button"); // MovePoint Layer만 감지

                Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f); // Ray Debug
                Debug.Log($"Ray 발사 위치: {ray.origin}, 방향: {ray.direction}");                              
            }
        }
        #endregion

        #region Player 및 Camera 위치 이동
        public void MoveToPosition(Vector3 targetPosition, Action onComplete)
        {
            if (!isMoving)
            {
                StartCoroutine(MoveToTarget(targetPosition, onComplete));
            }
        }        
        
        // MoveToTarget 코루틴이 완료된 후에 실행이 필요한 콜백 함수가 있어서 Action을 추가함.
        private IEnumerator MoveToTarget(Vector3 targetPosition, Action onComplete)
        {
            isMoving = true;

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }            

            transform.position = targetPosition;
            cameraTransform.position = targetPosition;
            isMoving = false;

            // 추가 해보는 Invoke
            onComplete?.Invoke();

            Debug.Log($"이동 완료! 현재 위치: {targetPosition}");
        }
        #endregion

        #region Player의 2D Canvas 인식
        // 플레이어가 2D Canvas를 인식하게 하기 위함
        private bool IsPointerOverUIObject()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            
            return results.Count > 0;
        }
        #endregion
    }
}
