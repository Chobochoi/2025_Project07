using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
//using OpenCover.Framework.Model;

public class FileLoadManager : MonoBehaviour
{
    [SerializeField] private Canvas infoCanvas;           // Panel�� ĵ���� (Close Button�� �ѹ��� ��� ����)
    //[SerializeField] private List<Canvas> panelListImages = new List<Canvas>();     // Canvas�� Panel ����Ʈ ���̹�:IMG_A000N
    [SerializeField] private List<TextMeshProUGUI> textSummaryPanel = new List<TextMeshProUGUI>();        // ���� �ؽ�Ʈ ����Ʈ
    [SerializeField] private List<TextMeshProUGUI> textTitlePanel = new List<TextMeshProUGUI>();        // ���� �ؽ�Ʈ ����Ʈ
        
    private string summaryTextName = "Summary";         // Text ��Ī�� ���� �з��ϱ� ����
    private string titleTextName = "Title";         // Text ��Ī�� ���� �з��ϱ� ����
    private string csvFileName = "Summary.CSV";         // csv ����

    private void Awake()
    {      
        SetTextSummaryPanel();
        SetTextTitlePanel();

        ApplyCSVDataToTextTitlePanel();
    }
    void Start()
    {
        ApplyCSVDataToTextSummaryPanel();        
    }

    #region CSV ���� �ҷ�����
    List<string[]> LoadCSV()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, csvFileName);

        // ���� ���� ���� Ȯ��  
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            List<string[]> csvContent = new List<string[]>();

            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    csvContent.Add(line.Split(',')); // ��ǥ�� �и��Ͽ� �迭�� ����  
                }
            }
            return csvContent;
        }
        else
        {
            Debug.LogError("CSV file not found: " + filePath);
            return null;
        }
    }

    string GetDataFromCSV(List<string[]> csvData, int row, int column)
    {
        if (row < csvData.Count && column < csvData[row].Length)
        {
            return csvData[row][column]; // Ư�� ��, ���� ������ ��ȯ  
        }
        else
        {
            Debug.LogError("Row or column index is out of bounds.");
            return null;
        }
    }
    #endregion

    #region TextSummaryPanel Set
    // TextSummary ������ �Է��ϱ� ����
    // ����� 2��(index 1) �κ��� Summary �����̴�. ���� �� ���� �ҽ��� �߰��ϰų� ���� �� ���� ����.
    public void SetTextSummaryPanel()
    {
        textSummaryPanel.Clear();

        foreach (Transform panel in infoCanvas.transform)
        {
            TextMeshProUGUI[] tmpGUIs = panel.GetComponentsInChildren<TextMeshProUGUI>();

            foreach (TextMeshProUGUI tmpGUI in tmpGUIs)
            {
                if(tmpGUI.name.Contains(summaryTextName))
                {
                    textSummaryPanel.Add(tmpGUI);
                }
            }
        }
    }    
    
    public void ApplyCSVDataToTextSummaryPanel()
    {
        // CSV ���� �б�
        List<string[]> csvData = LoadCSV();               

        // Panel ������ csvData ���� ��
        int textPanelCount = textSummaryPanel.Count;
        int dataCount = csvData.Count;

        for (int i = 0; i < textSummaryPanel.Count; i++)
        {
            // CSV �����Ͱ� ����� ���
            if (i < dataCount)
            {
                // i��° ��, 1��° �� ������
                string textValue = GetDataFromCSV(csvData, i, 1);
                textSummaryPanel[i].text = textValue;
            }

            // CSV �����Ͱ� ������ ���
            else
            {
                textSummaryPanel[i].text = "Data ����";
                Debug.LogWarning("CSV Data ����");
            }
        }
    }
    #endregion

    #region TextTitlePanel Set
    // TextTitle ������ �Է��ϱ� ����
    // ����� 1��(index 0) �κ��� Title ��Ī�̴�. ���� �� ���� �ҽ��� �߰��ϰų� ���� �� ���� ����.
    public void SetTextTitlePanel()
    {
        textTitlePanel.Clear();

        foreach (Transform panel in infoCanvas.transform)
        {
            TextMeshProUGUI[] tmpGUIs = panel.GetComponentsInChildren<TextMeshProUGUI>();

            foreach (TextMeshProUGUI tmpGUI in tmpGUIs)
            {
                if (tmpGUI.name.Contains(titleTextName))
                {
                    textTitlePanel.Add(tmpGUI);
                }
            }
        }
    }

    public void ApplyCSVDataToTextTitlePanel()
    {
        // CSV ���� �б�
        List<string[]> csvData = LoadCSV();

        // Panel ������ csvData ���� ��
        int textPanelCount = textTitlePanel.Count;
        int dataCount = csvData.Count;

        for (int i = 0; i < textTitlePanel.Count; i++)
        {
            // CSV �����Ͱ� ����� ���
            if (i < dataCount)
            {
                // i��° ��, 1��° �� ������
                string textValue = GetDataFromCSV(csvData, i, 0);

                //textValue = textValue.Replace("\uFFFD", " ");

                if (string.IsNullOrEmpty(textValue))
                {
                    textValue = "No Data";
                }
                
                textTitlePanel[i].text = textValue;
            }

            // CSV �����Ͱ� ������ ���
            else
            {
                textTitlePanel[i].text = "Data ����";
                Debug.LogWarning("CSV Data ����");
            }
        }
    }
    #endregion
}
