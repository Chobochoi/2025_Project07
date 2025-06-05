using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SceneNames { LoadingScene = 0, Deck_Preview }

namespace LoadingManager
{
    public class LoadingScene : MonoBehaviour
    {
        public static LoadingScene Instance { get; private set; }

        private WaitForSeconds waitChangeDelay;

        [Header("LoadIMG Settings")]
        [SerializeField] private GameObject loadingImage_KHAN;
        [SerializeField] private GameObject loadingImage_TMS;
        [SerializeField] private Slider loadingSlider;
        [SerializeField] private Button startButton;
        [SerializeField] private Button exitButton;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                waitChangeDelay = new WaitForSeconds(4.0f);

                DontDestroyOnLoad(gameObject);
            }

            // 시작 전 로딩화면 비활성화
            //loadingImage.SetActive(false);
        }

        private void Start()
        {            
            startButton.gameObject.SetActive(true);
            exitButton.gameObject.SetActive(true);
            loadingSlider.gameObject.SetActive(false);
        }

        public void LoadScene(string name)
        {
            loadingSlider.value = 0;

            StartCoroutine(LoadImageDuringMoveToTarget(name));
        }

        public void LoadScene(SceneNames name)
        {
            LoadScene(name.ToString());
        }

        public IEnumerator LoadImageDuringMoveToTarget(string name)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(name);
            asyncOperation.allowSceneActivation = false;
            
            loadingSlider.gameObject.SetActive(true);
            startButton.gameObject.SetActive(false);
            exitButton.gameObject.SetActive(false);

            float percent = 0;
            float loadingTime = 8.0f;

            //while (percent < 1.0f)
            while (percent < 1.0f)
            {
                percent += Time.deltaTime / loadingTime;
                loadingSlider.value = percent;

                Debug.Log($"{Mathf.RoundToInt(percent * 100)}");

                yield return null;
            }



            // 비동기 작업이 완료 될 때까지 반복
            //while (asyncOperation.isDone == false)
            //{
            //    percent += Time.deltaTime / loadingTime;
            //    loadingSlider.value = asyncOperation.progress;

            //    yield return null;
            //}

            asyncOperation.allowSceneActivation = true;

            yield return waitChangeDelay;

        }
    }
}
