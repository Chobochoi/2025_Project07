using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceManager : MonoBehaviour
{
    private Vector3 firstPoint;             // ù��° ���� ����Ʈ
    private Vector3 secondPoint;            // �ι�° ���� ����Ʈ
    private bool isFirstPointSet = false;   // ù��°�� �ι�° ����Ʈ Set false�� ����
    private bool isSecondPointSet = false;

    private LineRenderer lineRenderer;
    private Camera mainCamera;

    void Start()
    {
        // LineRenderer ������Ʈ �߰�  
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;

        mainCamera = Camera.main;
    }

    void Update()
    {
        // ���콺 ���� ��ư Ŭ�� ��  
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // ���� ������ ���� ã��  
            if (Physics.Raycast(ray, out hit))
            {
                if (!isFirstPointSet)
                {
                    firstPoint = hit.point;
                    isFirstPointSet = true;
                    Debug.Log("ù ��° �� ������");
                }
                else if (!isSecondPointSet)
                {
                    secondPoint = hit.point;
                    isSecondPointSet = true;

                    // �Ÿ� ���  
                    float distance = Vector3.Distance(firstPoint, secondPoint);
                    Debug.Log($"�� �� ������ �Ÿ�: {distance} ����");

                    // ���� �������� �� �׸���  
                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPosition(0, firstPoint);
                    lineRenderer.SetPosition(1, secondPoint);
                }
            }
        }

        // ��Ŭ������ �ʱ�ȭ  
        if (Input.GetMouseButtonDown(1))
        {
            ResetPoints();
        }
    }

    void ResetPoints()
    {
        isFirstPointSet = false;
        isSecondPointSet = false;
        lineRenderer.positionCount = 0;
        Debug.Log("�� �ʱ�ȭ");
    }
}
