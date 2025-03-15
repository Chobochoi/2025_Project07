using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramExit : MonoBehaviour
{
    public void GameExitEvent()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
