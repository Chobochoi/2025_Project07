using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollbarSizeManage : MonoBehaviour
{
    [SerializeField] private RectTransform contentParent;
    [SerializeField] private List<Button> moveButtons;

    public void CreateImageButtons()
    {
        // 콘텐츠 부모의 RectTransform 가져오기  
        RectTransform contentRectTransform = contentParent.GetComponent<RectTransform>();

        // 버튼의 높이 계산을 위한 변수들  
        float buttonHeight = 300f;
        float buttonWidth = 30f;

        // 첫 번째 버튼의 크기를 기준으로 설정(버튼이 있는 경우)  
        if (moveButtons.Count > 0 && moveButtons[0] != null)
        {
            buttonHeight = moveButtons[0].GetComponent<RectTransform>().rect.height;
            buttonWidth = moveButtons[0].GetComponent<RectTransform>().rect.width;
        }

        // 버튼 사이의 수직 간격 설정  
        float verticalSpacing = 10f;

        // Vertical 스크롤뷰이므로 버튼을 세로로 배치  
        int columnsCount = 1; // 세로 스크롤뷰는 1열로 고정  
        int rowsCount = moveButtons.Count; // 버튼 개수만큼 행 생성  

        // 총 높이 계산 (버튼 개수에 따라 조절됨)  
        float totalHeight = (buttonHeight + verticalSpacing) * rowsCount;
        if (rowsCount > 0) totalHeight += verticalSpacing; // 마지막 아래 여백 추가  

        // 세로 스크롤이기 때문에 가로 크기는 고정 (현재 설정 유지 또는 뷰포트 너비에 맞게 설정)  
        // float totalWidth = contentRectTransform.rect.width; // 기존 너비 유지  

        // 콘텐츠 부모의 높이만 조정 (세로 스크롤)  
        contentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
    }
}