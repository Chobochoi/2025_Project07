using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LoadingManager.LoadingScene.Instance.LoadScene(SceneNames.Intro);
    }

    // Update is called once per frame
    void GameExitEvent()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
