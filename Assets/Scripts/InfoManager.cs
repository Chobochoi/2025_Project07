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

    [SerializeField] private Canvas buttonCanvas;         //Infomation Button�� �����ϴ� Canvas
    [SerializeField] private Canvas infoCanvas;           // Panel�� ĵ���� (Close Button�� �ѹ��� ��� ����)

    [SerializeField] private List<Button> infoButtons = new List<Button>();         // infomation Button ����Ʈ
    [SerializeField] private List<Button> closeButtons = new List<Button>();        // Close Button ����Ʈ
    [SerializeField] private List<Canvas> panelListImages = new List<Canvas>();     // Canvas�� Panel ����Ʈ ���̹�:IMG_A000N
    [SerializeField] private List<RawImage> panelListTextures = new List<RawImage>();     // Canvas�� Panel ����Ʈ ���̹�:IMG_A000N
    [SerializeField] private List<Texture2D> imageTextures = new List<Texture2D>();
    [SerializeField] private List<TextMeshProUGUI> textSummaryPanel = new List<TextMeshProUGUI>();        // ���� �ؽ�Ʈ ����Ʈ
    private string closeButtonName = "Exit";                                        // Exit �̸��� ���� button�� �߰��ϱ� ����
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

    // ButtonCanvas ���ο� �ִ� BTN_A00N ���� �ʱ�ȭ �� Set�ϱ� ����
    public void SetAllInfomationButtons()
    {
        infoButtons.Clear();    // ���� �� ��ư �ʱ�ȭ

        // ButtonCanvans ���ο� �ִ� button�� �������� ����
        Button[] allInfoButtons = buttonCanvas.GetComponentsInChildren<Button>(true);

        foreach (Button button in allInfoButtons)
        {
            infoButtons.Add(button);
        }

        for (int i = 0; i < infoButtons.Count; i++)
        {
            int tempIndex = i;
            infoButtons[i].onClick.RemoveAllListeners();    // Ȥ�� �� �ߺ� �̺�Ʈ ����
            infoButtons[i].onClick.AddListener(() => OpenInfoCanvas(tempIndex));
        }
    }

    // ��� Panel�� �ʱ�ȭ �� Set�ϱ� ����       
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

    // �ʱ⿡ Panel���� ����� ����
    private void SetAllInformationPanelInActive()
    {
        foreach (var panel in panelListImages)
        {
            panel.enabled = false;
        }
    }    

    // ��� �ݱ� ��ư�� �ʱ�ȭ �� Set�ϱ� ����
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
            closeButtons[i].onClick.RemoveAllListeners();    // Ȥ�� �� �ߺ� �̺�Ʈ ����
            closeButtons[i].onClick.AddListener(() => CloseInfoCanvas(tempIndex));
        }

    }

    // Main Image Texture�� �����ϱ� ����
    // ���Ŀ� Ư�� ��ο��� Ư�� �̸����� ��� �ε��� ����.
    // ���絵 �ش� ����� ���� �ʾ� ���Ŀ� ��ųʸ� ������ ���� ����
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

    // Texture �̹��� �ҷ����� ����
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
            Debug.Log("��������");
            string[] lines = File.ReadAllLines(filePath);
            //UpdateTextSummary(lines);
        }

        else
        {
            Debug.Log("���Ͼ���");
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
            Debug.LogError($"�߸��� �ε���: {index} (�� �г� ����: {panelListImages.Count})");
            return;
        }

        // ������ �гθ� Ȱ��ȭ
        panelListImages[index].enabled = true;
        currentIndex = index;        
    }

    public void CloseInfoCanvas(int index)
    {
        if (index < 0 || index >= panelListImages.Count)
        {
            Debug.LogError($"�߸��� �ε���: {index} (�� �г� ����: {panelListImages.Count})");
            return;
        }

        currentIndex = index;
        panelListImages[index].enabled = false;
    }
}
;