using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoManager : MonoBehaviour
{

    [SerializeField] private Canvas buttonCanvas;         //Infomation Button을 관리하는 Canvas
    [SerializeField] private Canvas infoCanvas;           // Panel의 캔버스 (Close Button를 한번에 담기 위함)

    [SerializeField] private List<Button> infoButtons = new List<Button>();         // infomation Button 리스트
    [SerializeField] private List<Button> closeButtons = new List<Button>();        // Close Button 리스트
    [SerializeField] private List<Canvas> panelListImages = new List<Canvas>();     // Canvas의 Panel 리스트 네이밍:IMG_A000N
    [SerializeField] private List<RawImage> panelListTextures = new List<RawImage>();     // Canvas의 Panel 리스트 네이밍:IMG_A000N
    [SerializeField] private List<Texture2D> imageTextures = new List<Texture2D>();
    [SerializeField] private List<TextMeshProUGUI> textSummaryPanel = new List<TextMeshProUGUI>();        // 설명 텍스트 리스트
    private string closeButtonName = "Exit";                                        // Exit 이름을 가진 button을 추가하기 위함
    private string imageTexturePath = "Textures";
    private string csvFileName = "Summary.csv";
    public int currentIndex = -1; 

    private void Awake()
    {
        SetAllInfomationButtons();
        SetAllInformationPanel();
        SetCloseButtons();
        SetAllInformationPanelInActive();
        SetAllImageTexturePanel();
        LoadPanelImageTexture();
        AssignTexturesToPanels();
        LoadCSVFile();
        SetTextSummary();
    }

    private void Start()
    {
            
    }

    // ButtonCanvas 내부에 있는 BTN_A00N 들의 초기화 및 Set하기 위함
    public void SetAllInfomationButtons()
    {
        infoButtons.Clear();    // 시작 전 버튼 초기화

        // ButtonCanvans 내부에 있는 button만 가져오기 위함
        Button[] allInfoButtons = buttonCanvas.GetComponentsInChildren<Button>(true);

        foreach (Button button in allInfoButtons)
        {
            infoButtons.Add(button);
        }

        for (int i = 0; i < infoButtons.Count; i++)
        {
            int tempIndex = i;
            infoButtons[i].onClick.RemoveAllListeners();    // 혹시 모를 중복 이벤트 제거
            infoButtons[i].onClick.AddListener(() => OpenInfoCanvas(tempIndex));
        }
    }

    // 모든 Panel을 초기화 및 Set하기 위함       
    public void SetAllInformationPanel()
    {
        panelListImages.Clear();

        foreach (Transform panel in infoCanvas.transform)
        {
            Canvas[] imagePanels = panel.GetComponentsInChildren<Canvas>(true);

            foreach (Canvas imagePanel in imagePanels)
            {
                if(imagePanel.name.Contains("Panel"))
                {
                    panelListImages.Add(imagePanel);
                }
            }
        }
    }

    // 초기에 Panel들을 숨기기 위함
    private void SetAllInformationPanelInActive()
    {
        foreach (var panel in panelListImages)
        {
            panel.enabled = false;
        }
    }    

    // 모든 닫기 버튼을 초기화 및 Set하기 위함
    public void SetCloseButtons()
    {
        closeButtons.Clear();
        
        Button[] Buttons = infoCanvas.GetComponentsInChildren<Button>(true);
            
        foreach (Button button in Buttons)
        {
            if (button.name.Contains(closeButtonName))
            {
                closeButtons.Add(button);
            }
        }

        for (int i = 0; i < closeButtons.Count; i++)
        {
            int tempIndex = i;
            closeButtons[i].onClick.RemoveAllListeners();    // 혹시 모를 중복 이벤트 제거
            closeButtons[i].onClick.AddListener(() => CloseInfoCanvas(tempIndex));
        }

    }

    // Main Image Texture을 지정하기 위함
    // 추후에 특정 경로에서 특정 이름으로 모두 로드할 것임.
    // 현재도 해당 방식은 좋지 않아 추후에 딕셔너리 등으로 변경 예정
    public void SetAllImageTexturePanel()
    {
        panelListTextures.Clear();            
        
        RawImage[] imagePanels = infoCanvas.GetComponentsInChildren<RawImage>(true);
                               
        foreach (RawImage imagePanel in imagePanels)
        {
            if(imagePanel.name.Contains("Main"))
            {
                panelListTextures.Add(imagePanel);
            }
        }
        
    }

    // Texture 이미지 불러오기 위함
    public void LoadPanelImageTexture()
    {
        imageTextures.Clear();
        Texture2D[] textures = Resources.LoadAll<Texture2D>(imageTexturePath);
        imageTextures.AddRange(textures);
    }
    
    private void AssignTexturesToPanels()
    {       
        for (int i = 0; i < panelListTextures.Count;i++)
        {
            RawImage rawImage = panelListTextures[i].GetComponentInChildren<RawImage>();
            
            if (rawImage != null && i < panelListTextures.Count)
            {
                rawImage.texture = imageTextures[i];
            }
        }
    }

    public void LoadCSVFile()
    {

        string filePath = Path.Combine(Application.streamingAssetsPath, csvFileName);
        
        if (File.Exists(filePath))
        {
            Debug.Log("파일있음");
            string[] lines = File.ReadAllLines(filePath);
            //UpdateTextSummary(lines);
        }

        else
        {
            Debug.Log("파일없음");
        }
    }

    public void SetTextSummary()
    {
        TextMeshProUGUI[] textSummaryPanels = infoCanvas.GetComponentsInChildren<TextMeshProUGUI>(true);

        foreach (TextMeshProUGUI textDataSummary in textSummaryPanels)
        {
            if (textDataSummary.name.Contains("Summary"))
            {
                textSummaryPanel.Add(textDataSummary);
            }
        }        
    }

    public void Test(string[] textSummary)
    {
        int minCount = Mathf.Min(textSummaryPanel.Count, textSummary.Length);
        
        for (int i = 0; i < minCount; i++)
        {
            textSummaryPanel[i].text = textSummary[i];
        }
    }

    public void OpenInfoCanvas(int index)
    {
        if (index < 0 || index >= panelListImages.Count)
        {
            Debug.LogError($"잘못된 인덱스: {index} (총 패널 개수: {panelListImages.Count})");
            return;
        }

        // 선택한 패널만 활성화
        panelListImages[index].enabled = true;
        currentIndex = index;        
    }

    public void CloseInfoCanvas(int index)
    {
        if (index < 0 || index >= panelListImages.Count)
        {
            Debug.LogError($"잘못된 인덱스: {index} (총 패널 개수: {panelListImages.Count})");
            return;
        }

        currentIndex = index;
        panelListImages[index].enabled = false;
    }
}
;