using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceManager : MonoBehaviour
{
    private Vector3 firstPoint;             // 첫번째 찍을 포인트
    private Vector3 secondPoint;            // 두번째 찍을 포인트
    private bool isFirstPointSet = false;   // 첫번째와 두번째 포인트 Set false로 시작
    private bool isSecondPointSet = false;

    private LineRenderer lineRenderer;
    private Camera mainCamera;

    void Start()
    {
        // LineRenderer 컴포넌트 추가  
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
        // 마우스 왼쪽 버튼 클릭 시  
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 월드 공간의 지점 찾기  
            if (Physics.Raycast(ray, out hit))
            {
                if (!isFirstPointSet)
                {
                    firstPoint = hit.point;
                    isFirstPointSet = true;
                    Debug.Log("첫 번째 점 설정됨");
                }
                else if (!isSecondPointSet)
                {
                    secondPoint = hit.point;
                    isSecondPointSet = true;

                    // 거리 계산  
                    float distance = Vector3.Distance(firstPoint, secondPoint);
                    Debug.Log($"두 점 사이의 거리: {distance} 유닛");

                    // 라인 렌더러로 선 그리기  
                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPosition(0, firstPoint);
                    lineRenderer.SetPosition(1, secondPoint);
                }
            }
        }

        // 우클릭으로 초기화  
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
        Debug.Log("점 초기화");
    }
}
