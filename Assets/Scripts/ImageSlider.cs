using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.Networking;
using ControllerManager;

[System.Serializable]
public class CategoryData
{
    public string subfolderName;
    public GameObject transformParentObject;
}

[System.Serializable]
public class FadeSettings
{
    public float fadeDuration = 0.5f;
    public Color fadeColor = Color.black;
}

public class ImageSlider : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform contentParent;
    [SerializeField] private List<CategoryData> categoryDataList;

    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform cameraTransform;

    [Header("Smooth Movement Settings")]
    [SerializeField] private float movementDuration = 2f;
    [SerializeField] private float rotationDuration = 2f;
    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Fade Settings")]
    [SerializeField] private FadeSettings fadeSettings;
    [SerializeField] private RawImage fadeOverlayImage;

    [Header("MiniMap Setting")]
    [SerializeField] private Transform miniMapCameraTransform;

    private List<string> imagePaths = new List<string>();
    private List<Transform> availableTransforms = new List<Transform>();
    private ScrollRect scrollRect;
    private int currentCategoryIndex = 0;
    private bool isTransitioning = false;

    // Hotspot Button 업데이트를 위한 Instance 호출
    [SerializeField] private PanoramaSpotsController panoramaSpotsController;

    private void Start()
    {
        // 페이드 오버레이 이미지 초기 설정  
        if (fadeOverlayImage != null)
        {
            fadeOverlayImage.color = Color.clear;
            fadeOverlayImage.raycastTarget = false;
        }

        scrollRect = GetComponent<ScrollRect>();

        LoadImagesFromStreamingAssets(currentCategoryIndex);
        CreateImageButtons();
    }

    public void ChangeImageCategory(int categoryIndex)
    {
        if (categoryIndex < 0 || categoryIndex >= categoryDataList.Count)
        {
            Debug.LogError("Invalid category index");
            return;
        }

        currentCategoryIndex = categoryIndex;
        LoadImagesFromStreamingAssets(currentCategoryIndex);
        CreateImageButtons();
    }

    private void LoadImagesFromStreamingAssets(int categoryIndex)
    {
        imagePaths.Clear();
        availableTransforms.Clear();              

        try
        {
            CategoryData currentCategory = categoryDataList[categoryIndex];

            string folderPath = Path.Combine(Application.streamingAssetsPath, currentCategory.subfolderName);

            string[] imageExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };

            imagePaths = Directory.GetFiles(folderPath)
                .Where(path => imageExtensions.Contains(Path.GetExtension(path).ToLower()))
                .ToList();

            availableTransforms = currentCategory.transformParentObject
                .GetComponentsInChildren<Transform>()
                .Where(t => t.parent == currentCategory.transformParentObject.transform)
                .ToList();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load images or transforms: {e.Message}");
        }
    }

    private void CreateImageButtons()
    {
        // 기존 자식 오브젝트들 모두 제거  
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 이미지 개수와 사용 가능한 트랜스폼 개수 중 작은 값으로 아이템 수 결정  
        int itemCount = Mathf.Min(imagePaths.Count, availableTransforms.Count);

        // 콘텐츠 부모의 RectTransform 가져오기  
        RectTransform contentRectTransform = contentParent.GetComponent<RectTransform>();

        // 버튼 프리팹의 RectTransform 가져오기  
        RectTransform buttonPrefabRectTransform = buttonPrefab.GetComponent<RectTransform>();

        // 버튼의 높이와 너비 계산  
        float buttonHeight = buttonPrefabRectTransform.rect.height;
        float buttonWidth = buttonPrefabRectTransform.rect.width;

        // 버튼 사이의 수직, 수평 간격 설정 (필요에 따라 조정 가능)  
        float verticalSpacing = 10f;
        float horizontalSpacing = 10f;

        // 수평 레이아웃 설정 (원하는 레이아웃에 따라 조정)  
        int columnsCount = 4; // 예: 4개의 열로 배치  
        int rowsCount = Mathf.CeilToInt((float)itemCount / columnsCount);

        // 총 높이와 너비 계산  
        float totalHeight = (buttonHeight + verticalSpacing) * rowsCount;
        float totalWidth = (buttonWidth + horizontalSpacing) * itemCount + (horizontalSpacing * 6);

        // 콘텐츠 부모의 높이와 너비 설정  
        contentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
        contentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, totalWidth);

        for (int i = 0; i < itemCount; i++)
        {
            // 버튼 프리팹을 콘텐츠 부모에 인스턴스화  
            GameObject buttonObject = Instantiate(buttonPrefab, contentParent);

            // 버튼 위치 계산 (그리드 레이아웃 스타일)  
            int row = i / columnsCount;
            int col = i % columnsCount;

            RectTransform buttonRectTransform = buttonObject.GetComponent<RectTransform>();
            buttonRectTransform.anchoredPosition = new Vector2(
                col * (buttonWidth + horizontalSpacing),
                -row * (buttonHeight + verticalSpacing)
            );

            // 이미지 로딩 및 버튼 클릭 이벤트 설정 (이전 코드와 동일)  
            Image buttonImage = buttonObject.GetComponent<Image>();
            if (buttonImage != null)
            {
                StartCoroutine(LoadImageCoroutine(imagePaths[i], buttonImage));
            }

            int index = i;
            Button button = buttonObject.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnButtonClicked(index));
            }
        }
    }

    private IEnumerator LoadImageCoroutine(string path, Image targetImage)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture("file://" + path);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            targetImage.sprite = sprite;
        }
        else
        {
            Debug.LogError($"Failed to load image: {path}");
        }
    }

    //private IEnumerator LoadImageCoroutine(string path, Image targetImage)
    //{
    //    // WebGL에서는 StreamingAssets 경로를 직접 사용  
    //    string webPath = Path.Combine("StreamingAssets","Resource", "Image","Temp02_SnapShot", Path.GetFileName(path));

    //    UnityWebRequest request = UnityWebRequestTexture.GetTexture(webPath);

    //    yield return request.SendWebRequest();

    //    if (request.result == UnityWebRequest.Result.Success)
    //    {
    //        Texture2D texture = DownloadHandlerTexture.GetContent(request);
    //        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    //        targetImage.sprite = sprite;
    //    }
    //    else
    //    {
    //        Debug.LogError($"Failed to load image: {webPath}");
    //    }
    //}

    private void OnButtonClicked(int index)
    {
        if (index >= 0 && index < availableTransforms.Count && !isTransitioning)
        {
            StartCoroutine(SmoothTransition(availableTransforms[index]));            
        }        
    }

    private IEnumerator SmoothTransition(Transform targetTransform)
    {
        if (fadeOverlayImage == null)
        {
            Debug.LogError("Fade Overlay Image is not assigned!");
            yield break;
        }

        isTransitioning = true;

        // 페이드 아웃  
        yield return StartCoroutine(FadeScreen(true));

        // 위치 및 회전 설정  
        Vector3 startPlayerPosition = playerTransform.position;
        Vector3 startCameraPosition = cameraTransform.position;
        Quaternion startPlayerRotation = playerTransform.rotation;
        Quaternion startCameraRotation = cameraTransform.rotation;

        Vector3 targetPlayerPosition = targetTransform.position;
        Vector3 targetCameraPosition = targetTransform.position;
        Quaternion targetPlayerRotation = targetTransform.rotation;
        Quaternion targetCameraRotation = targetTransform.rotation;

        // Minimap 위치
        Vector3 newMiniMapPoistion = miniMapCameraTransform.position;
        newMiniMapPoistion.x = targetPlayerPosition.x;
        newMiniMapPoistion.z = targetPlayerPosition.z;
        miniMapCameraTransform.position = newMiniMapPoistion;

        // 실제 이동  
        float elapsedTime = 0f;
        while (elapsedTime < movementDuration)
        {
            float time = movementCurve.Evaluate(elapsedTime / movementDuration);

            if (playerTransform != null)
            {
                playerTransform.position = Vector3.Lerp(startPlayerPosition, targetPlayerPosition, time);
                playerTransform.rotation = Quaternion.Slerp(startPlayerRotation, targetPlayerRotation, time);
            }

            if (cameraTransform != null)
            {
                cameraTransform.position = Vector3.Lerp(startCameraPosition, targetCameraPosition, time);
                cameraTransform.rotation = Quaternion.Slerp(startCameraRotation, targetCameraRotation, time);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 최종 위치 설정  
        if (playerTransform != null)
        {
            playerTransform.position = targetPlayerPosition;
            playerTransform.rotation = targetPlayerRotation;
            newMiniMapPoistion.x = targetPlayerPosition.x;
            newMiniMapPoistion.z = targetPlayerPosition.z;
        }

        if (cameraTransform != null)
        {
            cameraTransform.position = targetCameraPosition;
            cameraTransform.rotation = targetCameraRotation;
            newMiniMapPoistion.x = targetPlayerPosition.x;
            newMiniMapPoistion.z = targetPlayerPosition.z;
        }

        // 페이드 인  
        yield return StartCoroutine(FadeScreen(false));
        
        // 왜 해당 함수가 호출이 안되는지?
        ButtonUpdate();

        isTransitioning = false;
    }

    private IEnumerator FadeScreen(bool fadeOut)
    {
        float elapsedTime = 0f;
        Color startColor = fadeOut ? Color.clear : fadeSettings.fadeColor;
        Color targetColor = fadeOut ? fadeSettings.fadeColor : Color.clear;

        while (elapsedTime < fadeSettings.fadeDuration)
        {
            float t = elapsedTime / fadeSettings.fadeDuration;
            fadeOverlayImage.color = Color.Lerp(startColor, targetColor, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 최종 색상 설정  
        fadeOverlayImage.color = targetColor;
    }

    public void ButtonUpdate()
    {
        if (panoramaSpotsController != null)
        {
            panoramaSpotsController.UpdateButtonActivation();
        }
    }
}