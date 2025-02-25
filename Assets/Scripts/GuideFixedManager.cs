using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideFixedManager : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;

    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(0, -0.5f, 0);
    }
}
