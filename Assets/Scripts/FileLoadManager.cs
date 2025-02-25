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
    [SerializeField] private Canvas infoCanvas;           // Panel�� ĵ���� (Close Button�� �ѹ��� ��� ����)
    [SerializeField] private List<Canvas> panelListImages = new List<Canvas>();     // Canvas�� Panel ����Ʈ ���̹�:IMG_A000N
    [SerializeField] private List<TextMeshProUGUI> textSummaryPanel = new List<TextMeshProUGUI>();        // ���� �ؽ�Ʈ ����Ʈ
    private string csvFileName = "Summary.csv";

    private void Awake()
    {
        // CSV ���� �б�
        List<string[]> csvData = LoadCSV();

        // Ư�� �� �� ���� ������ ��ȸ  
        if (csvData != null && csvData.Count > 0)
        {
            for (int i = 0; i < csvData.Count; i++)
            {
                string specificData = GetDataFromCSV(csvData, i, 1); // 2�� i��  
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

        // ���� ���� ���� Ȯ��  
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            List<string[]> csvContent = new List<string[]>();

            using (StreamReader reader = new StreamReader(filePath, Encoding.Default))
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
}
