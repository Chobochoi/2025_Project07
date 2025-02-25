using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetting : MonoBehaviour
{
    [SerializeField] GameObject startPoint;
    [SerializeField] Camera startCamera;
    
    // Start is called before the first frame update
    void Start()
    {        
        startCamera.transform.position = startPoint.transform.position;
        Debug.Log(startCamera.transform.position);        

    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
    }
    
    IEnumerator MoveCamera()
    {        
        yield return new WaitForSeconds(2.0f);

        startCamera.transform.position = new Vector3(2, 0, 0);
    }
}
