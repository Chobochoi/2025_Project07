using UnityEngine;
using System;

namespace ControllerManager
{
    public class SkyboxManager : MonoBehaviour
    {
        [Serializable]
        public class SkyboxTarget
        {
            public GameObject targetObject;
            public GameObject[] skyboxes;
            [HideInInspector] public float[] skyboxIntensities; // 강도 배열 추가  
        }

        [SerializeField] private SkyboxTarget[] skyboxTargets;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private float activationRadius = 2f;
        [SerializeField] private float fadeSpeed = 2f; // 페이드 속도 조절 변수  

        private void Start()
        {
            // 스카이박스 강도 배열 초기화  
            InitializeSkyboxIntensities();
            DeactivateAllSkyboxes();
        }

        private void InitializeSkyboxIntensities()
        {
            foreach (SkyboxTarget target in skyboxTargets)
            {
                target.skyboxIntensities = new float[target.skyboxes.Length];
                for (int i = 0; i < target.skyboxes.Length; i++)
                {
                    target.skyboxIntensities[i] = 0f; // 초기 강도를 0으로 설정  
                }
            }
        }

        private void Update()
        {
            CheckAndManageSkyboxes();
        }

        private void DeactivateAllSkyboxes()
        {
            foreach (SkyboxTarget target in skyboxTargets)
            {
                foreach (GameObject skybox in target.skyboxes)
                {
                    SetSkyboxIntensity(skybox, 0f);
                }
            }
        }

        // 해당 함수에서 Null Reference가 뜰 경우
        // Target Object 내부에 Spots가 비어있는건 없는지 확인하기.
        private void CheckAndManageSkyboxes()
        {
            // playerController가 null인지 먼저 확인  
            if (playerController == null)
            {
                Debug.LogError("PlayerController is not assigned!");
                return;
            }

            foreach (SkyboxTarget target in skyboxTargets)
            {
                if (target.skyboxes == null) continue;

                for (int i = 0; i < target.skyboxes.Length; i++)
                {
                    GameObject skybox = target.skyboxes[i];

                    // 플레이어와 스카이박스의 거리 계산  
                    float distance = Vector3.Distance(playerController.transform.position, skybox.transform.position);

                    // 활성화 조건: 플레이어가 Player 태그이고 거리 조건 만족  
                    bool shouldBeActive = playerController.transform.CompareTag("Player") &&
                                          distance <= activationRadius;

                    // 부드러운 강도 페이드 인/아웃  
                    float targetIntensity = shouldBeActive ? 1f : 0f;
                    target.skyboxIntensities[i] = Mathf.Lerp(target.skyboxIntensities[i], targetIntensity, Time.deltaTime * fadeSpeed);

                    // 강도에 따라 스카이박스 설정  
                    SetSkyboxIntensity(skybox, target.skyboxIntensities[i]);
                }
            }
        }

        private void SetSkyboxIntensity(GameObject skybox, float intensity)
        {
            Renderer renderer = skybox.GetComponent<Renderer>();
            
            if (renderer != null)
            {
                Material material = renderer.material;

                // 스카이박스/파노라마 머터리얼에 맞는 속성 조절  
                // _Exposure 또는 _Intensity 등의 속성을 사용  
                material.SetFloat("_Exposure", intensity);

                // 완전히 어두워지면 렌더러 비활성화  
                renderer.enabled = intensity > 0.5f;
            }
        }

        // 에디터에서 쉽게 설정할 수 있도록 하는 메서드  
        public void AddSkyboxTarget(GameObject targetObject, GameObject[] skyboxes)
        {
            SkyboxTarget newTarget = new SkyboxTarget
            {
                targetObject = targetObject,
                skyboxes = skyboxes,
                skyboxIntensities = new float[skyboxes.Length]
            };

            // 기존 배열에 추가  
            Array.Resize(ref skyboxTargets, skyboxTargets.Length + 1);
            skyboxTargets[skyboxTargets.Length - 1] = newTarget;
        }
    }
}