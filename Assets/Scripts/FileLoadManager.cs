using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class FileLoadManager : MonoBehaviour
{
    [SerializeField] private Canvas infoCanvas;           // Panel의 캔버스 (Close Button를 한번에 담기 위함)
    [SerializeField] private List<Canvas> panelListImages = new List<Canvas>();     // Canvas의 Panel 리스트 네이밍:IMG_A000N
    [SerializeField] private List<TextMeshProUGUI> textSummaryPanel = new List<TextMeshProUGUI>();        // 설명 텍스트 리스트
    private string csvFileName = "Summary.csv";

    private void Awake()
    {
        // CSV 파일 읽기
        List<string[]> csvData = LoadCSV();

        // 특정 행 및 열의 데이터 조회  
        if (csvData != null && csvData.Count > 0)
        {
            for (int i = 0; i < csvData.Count; i++)
            {
                string specificData = GetDataFromCSV(csvData, i, 1); // 2행 i열  
                Debug.Log("Specific Data: " + specificData);
            }
        }
    }
    void Start()
    {
        
    }

    List<string[]> LoadCSV()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, csvFileName);

        // 파일 존재 여부 확인  
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            List<string[]> csvContent = new List<string[]>();

            using (StreamReader reader = new StreamReader(filePath, Encoding.Default))
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
}
