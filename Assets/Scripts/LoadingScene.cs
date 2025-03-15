using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SceneNames { LoadingScene = 0, Intro }

namespace LoadingManager
{
    public class LoadingScene : MonoBehaviour
    {
        public static LoadingScene Instance { get; private set; }

        private WaitForSeconds waitChangeDelay;

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
                waitChangeDelay = new WaitForSeconds(4.0f);

                DontDestroyOnLoad(gameObject);
            }

            // 시작 전 로딩화면 비활성화
            //loadingImage.SetActive(false);
        }

        public void LoadScene(string name)
        {
            loadingSlider.value = 0;
            loadingImage.SetActive(true);

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

            float percent = 0;
            float loadingTime = 3.0f;

            //while (percent < 1.0f)
            while (percent < 1.0f)
            {
                percent += Time.deltaTime / loadingTime;
                loadingSlider.value = percent;

                Debug.Log($"{Mathf.RoundToInt(percent * 100)}");
                
                yield return null;
            }

            asyncOperation.allowSceneActivation = true;

            /*
            // 비동기 작업이 완료 될 때까지 반복
            while (asyncOperation.isDone == false)
            {
                percent += Time.deltaTime / loadingTime;
                loadingSlider.value = asyncOperation.progress;

                yield return null;
            }
            */

            yield return waitChangeDelay;

            loadingImage.SetActive(false);
        }
    }
}
