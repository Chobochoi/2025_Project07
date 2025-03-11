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
    [SerializeField] private Canvas infoCanvas;           // Panel의 캔버스 (Close Button를 한번에 담기 위함)
    //[SerializeField] private List<Canvas> panelListImages = new List<Canvas>();     // Canvas의 Panel 리스트 네이밍:IMG_A000N
    [SerializeField] private List<TextMeshProUGUI> textSummaryPanel = new List<TextMeshProUGUI>();        // 설명 텍스트 리스트
    [SerializeField] private List<TextMeshProUGUI> textTitlePanel = new List<TextMeshProUGUI>();        // 설명 텍스트 리스트
        
    private string summaryTextName = "Summary";         // Text 명칭에 따라 분류하기 위함
    private string titleTextName = "Title";         // Text 명칭에 따라 분류하기 위함
    private string csvFileName = "Summary.CSV";         // csv 파일

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

    #region CSV 파일 불러오기
    List<string[]> LoadCSV()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, csvFileName);

        // 파일 존재 여부 확인  
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            List<string[]> csvContent = new List<string[]>();

            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    csvContent.Add(line.Split(',')); // 쉼표로 분리하여 배열에 저장  
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
            return csvData[row][column]; // 특정 행, 열의 데이터 반환  
        }
        else
        {
            Debug.LogError("Row or column index is out of bounds.");
            return null;
        }
    }
    #endregion

    #region TextSummaryPanel Set
    // TextSummary 내용을 입력하기 위함
    // 현재는 2열(index 1) 부분이 Summary 내용이다. 추후 더 많은 소스를 추가하거나 변경 할 수도 있음.
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
        // CSV 파일 읽기
        List<string[]> csvData = LoadCSV();               

        // Panel 개수와 csvData 개수 비교
        int textPanelCount = textSummaryPanel.Count;
        int dataCount = csvData.Count;

        for (int i = 0; i < textSummaryPanel.Count; i++)
        {
            // CSV 데이터가 충분한 경우
            if (i < dataCount)
            {
                // i번째 행, 1번째 열 데이터
                string textValue = GetDataFromCSV(csvData, i, 1);
                textSummaryPanel[i].text = textValue;
            }

            // CSV 데이터가 부족한 경우
            else
            {
                textSummaryPanel[i].text = "Data 부족";
                Debug.LogWarning("CSV Data 부족");
            }
        }
    }
    #endregion

    #region TextTitlePanel Set
    // TextTitle 내용을 입력하기 위함
    // 현재는 1열(index 0) 부분이 Title 명칭이다. 추후 더 많은 소스를 추가하거나 변경 할 수도 있음.
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
        // CSV 파일 읽기
        List<string[]> csvData = LoadCSV();

        // Panel 개수와 csvData 개수 비교
        int textPanelCount = textTitlePanel.Count;
        int dataCount = csvData.Count;

        for (int i = 0; i < textTitlePanel.Count; i++)
        {
            // CSV 데이터가 충분한 경우
            if (i < dataCount)
            {
                // i번째 행, 1번째 열 데이터
                string textValue = GetDataFromCSV(csvData, i, 0);

                //textValue = textValue.Replace("\uFFFD", " ");

                if (string.IsNullOrEmpty(textValue))
                {
                    textValue = "No Data";
                }
                
                textTitlePanel[i].text = textValue;
            }

            // CSV 데이터가 부족한 경우
            else
            {
                textTitlePanel[i].text = "Data 부족";
                Debug.LogWarning("CSV Data 부족");
            }
        }
    }
    #endregion
}
