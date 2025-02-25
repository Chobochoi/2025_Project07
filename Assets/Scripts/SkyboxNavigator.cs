using UnityEngine;

public class SkyboxNavigator : MonoBehaviour
{
    public Transform[] locations; // �̵� ������ ��ü���� ��ġ ���
    public Material[] skyboxes; // �� ��ġ�� ���� Skybox
    public float moveSpeed = 5f; // �̵� �ӵ�

    private int currentIndex = 0;
    private bool isMoving = false;

    void Update()
    {
        if (isMoving) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                for (int i = 0; i < locations.Length; i++)
                {
                    if (hit.transform == locations[i])
                    {
                        StartCoroutine(MoveToLocation(i));
                        break;
                    }
                }
            }
        }
    }

    private System.Collections.IEnumerator MoveToLocation(int index)
    {
        isMoving = true;
        Transform target = locations[index];

        // �ε巴�� �̵�
        while (Vector3.Distance(transform.position, target.position) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Skybox ����
        RenderSettings.skybox = skyboxes[index];
        currentIndex = index;
        isMoving = false;
    }
}

