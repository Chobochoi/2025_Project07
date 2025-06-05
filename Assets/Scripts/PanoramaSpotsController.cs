using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEngine.UIElements;

namespace ControllerManager
{
    public class PanoramaSpotsController : MonoBehaviour
    {
        public static PanoramaSpotsController Instance { get; private set; }

        [Serializable]
        public class PanoramaTarget
        {
            public GameObject targetObject;
            public Transform[] spots;
        }

        [SerializeField] private PanoramaTarget[] panoramaTargets;
        [SerializeField] private PlayerController playerController;

        [Header("Button Activation Settings")]
        [SerializeField] private float activationRadius = 5f; // 버튼 활성화 거리  
        [SerializeField] private LayerMask playerLayer; // 플레이어 레이어  

        [SerializeField] private Transform mainCamera;
        private List<Button> allSpotButtons = new List<Button>();
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            // 모든 타겟의 스팟 버튼에 리스너 추가 및 초기 비활성화  
            InitializeSpotButtons();
            UpdateButtonActivation();
        }

        private void Update()
        {
            ButtonLookAtCamera();            
        }

        private void LateUpdate()
        {
            // 주기적으로 버튼 활성/비활성 상태 체크  
            //UpdateButtonActivation();
        }

        private void InitializeSpotButtons()
        {
            allSpotButtons.Clear();

            // 테스트용 추가

            foreach (PanoramaTarget target in panoramaTargets)
            {
                foreach (Transform spotTransform in target.spots)
                {
                    // 각 스팟의 캔버스 내 버튼 찾기  
                    Canvas spotCanvas = spotTransform.GetComponentInChildren<Canvas>();
                    if (spotCanvas != null)
                    {
                        Button spotButton = spotCanvas.GetComponentInChildren<Button>();
                        if (spotButton != null)
                        {
                            // 버튼 리스트에 추가  
                            allSpotButtons.Add(spotButton);

                            // 초기에 모든 버튼 비활성화  
                            spotButton.gameObject.SetActive(false);

                            // 테스트용 추가
                            //spotButton.GetComponent<CanvasRenderer>().cullTransparentMesh = false;

                            // 해당 스팟의 위치로 이동하는 리스너 추가  
                            spotButton.onClick.AddListener(() => MoveToSpot(spotTransform));
                        }
                    }
                }
            }
        }

        public void UpdateButtonActivation()
        {
            foreach (Button button in allSpotButtons)
            {
                // 버튼의 부모 스팟 트랜스폼 찾기  
                Transform spotTransform = button.GetComponentInParent<Transform>();

                // 플레이어와의 거리 계산  
                float distance = Vector3.Distance(playerController.transform.position, spotTransform.position);                               

                // 거리 조건에 따라 명시적으로 활성/비활성  
                if (distance <= activationRadius && !button.gameObject.activeSelf)
                {
                    // 거리 이내이고 현재 비활성 상태라면 활성화  
                    button.gameObject.SetActive(true);
                }
                else if (distance > activationRadius && button.gameObject.activeSelf)
                {
                    // 거리 밖이고 현재 활성 상태라면 비활성화  
                    button.gameObject.SetActive(false);
                }
            }
        }

        public void ButtonLookAtCamera()
        {
            if (mainCamera == null) return;

            mainCamera = Camera.main.transform;

            foreach (var button in allSpotButtons)
            {
                // 카메라와 버튼 높이가 다르면 Button Rotation을 돌려 Button을 인식할 수 있게 함.
                if (button.transform.position.y + 2 > mainCamera.transform.position.y)
                {
                    button.transform.rotation = Quaternion.Euler(-90, 90, 0);
                }
                
                // 카메라와 버튼 높이가 같으면 Button Rotation 그대로 Set
                else if (button.transform.position.y + 2 <= mainCamera.transform.position.y)
                {
                    button.transform.rotation = Quaternion.Euler(90, 90, 0);
                }                
            }
        }

        private void MoveToSpot(Transform spotTransform)
        {
            if (playerController != null)
            {
                // 플레이어 이동  
                playerController.MoveToPosition(spotTransform.position, () => {
                    UpdateButtonActivation();
                    // 이동 완료 후 추가 작업 (필요시)  
                    Debug.Log($"Moved to spot: {spotTransform.name}");
                });
            }
            else
            {
                Debug.LogError("PlayerController is not assigned!");
            }
        }

        // 에디터에서 쉽게 설정할 수 있도록 하는 메서드  
        public void AddPanoramaTarget(GameObject targetObject)
        {
            PanoramaTarget newTarget = new PanoramaTarget
            {
                targetObject = targetObject,
                spots = targetObject.GetComponentsInChildren<Transform>()
            };

            // 기존 배열에 추가  
            Array.Resize(ref panoramaTargets, panoramaTargets.Length + 1);
            panoramaTargets[panoramaTargets.Length - 1] = newTarget;
        }
    }
}