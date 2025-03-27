using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PanelSlider : MonoBehaviour
{
    public RectTransform panel;
    public float slideDuration = 0.5f;
    public float slideDistance = 220;
    private bool isVisible = true;
    private Vector2 showPosition;
    [SerializeField] public RawImage imagePivot;

    void Start()
    {
        showPosition = panel.anchoredPosition;
    }

    public void TogglePanel()
    {
        if (isVisible)
        {
            // 아래로 슬라이드  
            Vector2 targetPos = new Vector2(showPosition.x, showPosition.y - slideDistance);
            panel.DOAnchorPos(targetPos, slideDuration).SetEase(Ease.OutQuad);
            imagePivot.rectTransform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            // 위로 슬라이드  
            panel.DOAnchorPos(showPosition, slideDuration).SetEase(Ease.OutQuad);
            imagePivot.rectTransform.localScale = new Vector3(1, -1, 1);
        }

        isVisible = !isVisible;
    }
}