using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class randomindextest : MonoBehaviour
{
    private List<string> groups = new List<string>(); // 랜덤 정렬을 위한 리스트
    private List<int> t2Indices = new List<int>();
    private List<int> t3rIndices = new List<int>();
    private List<int> t3lIndices = new List<int>();
    private List<int> t4Indices = new List<int>();

    public string participant = "p0"; // 참가자 ID
    public string session = "s1"; // 세션 ID

    private void Start()
    {
        // 출력 파일 경로 설정
        string outputFilePath = Path.Combine(Application.persistentDataPath, "pilot_p0_s1.txt");
        Debug.Log(outputFilePath);

        // JSON 파일 읽기
        LoadIndicesFromJson(participant, session);

        // 각 그룹 개수만큼 추가
        AddGroups("t1", 27); // t1은 고정된 값 사용 (또는 JSON 데이터에 포함 가능)
        AddRandomizedIndices("t2", t2Indices, t2Indices.Count); // JSON 데이터의 개수 사용
        AddRandomizedIndices("t3r", t3rIndices, t3rIndices.Count);
        AddRandomizedIndices("t3l", t3lIndices, t3lIndices.Count);
        AddRandomizedIndices("t4", t4Indices, t4Indices.Count);

        // 그룹 섞기
        groups = ShuffleList(groups);

        // 결과 출력
        Debug.Log("Randomly Selected Groups: " + string.Join(", ", groups));

        // 결과를 파일에 저장
        SaveToFile(outputFilePath, groups);
    }

    private void LoadIndicesFromJson(string participant, string session)
    {
        try
        {
            // Resources에서 JSON 파일 읽기
            TextAsset jsonText = Resources.Load<TextAsset>("Index/pilot_index");
            if (jsonText == null)
            {
                Debug.LogError("JSON file not found in Resources/Index/pilot_index");
                return;
            }

            // JSON 내용을 파싱
            string jsonContent = jsonText.text;
            JObject json = JObject.Parse(jsonContent);

            // 참가자와 세션 데이터 가져오기
            var sessionData = json[participant]?[session];
            if (sessionData == null)
            {
                Debug.LogError($"Session data not found for participant {participant}, session {session}");
                return;
            }

            // t2, t3r, t3l, t4 데이터 가져오기
            t2Indices = sessionData["t2"]?.ToObject<List<int>>() ?? new List<int>();
            t3rIndices = sessionData["t3r"]?.ToObject<List<int>>() ?? new List<int>();
            t3lIndices = sessionData["t3l"]?.ToObject<List<int>>() ?? new List<int>();
            t4Indices = sessionData["t4"]?.ToObject<List<int>>() ?? new List<int>();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error reading JSON file: {ex.Message}");
        }
    }

    private void AddGroups(string groupName, int count)
    {
        for (int i = 0; i < count; i++)
        {
            groups.Add(groupName);
        }
    }

    private void AddRandomizedIndices(string groupName, List<int> indices, int count)
    {
        if (indices.Count < count)
        {
            Debug.LogWarning($"Not enough indices in {groupName}. Using all available indices.");
            count = indices.Count;
        }

        List<int> selectedIndices = RandomlySelect(indices, count);
        foreach (int index in selectedIndices)
        {
            groups.Add($"{groupName}_{index}");
        }
    }

    private List<int> RandomlySelect(List<int> sourceList, int count)
    {
        List<int> selectedItems = new List<int>();
        List<int> tempList = new List<int>(sourceList);

        while (selectedItems.Count < count)
        {
            int randomIndex = Random.Range(0, tempList.Count);
            selectedItems.Add(tempList[randomIndex]);
            tempList.RemoveAt(randomIndex);
        }

        return selectedItems;
    }

    private List<string> ShuffleList(List<string> list)
    {
        List<string> shuffledList = new List<string>(list);
        for (int i = 0; i < shuffledList.Count; i++)
        {
            int randomIndex = Random.Range(0, shuffledList.Count);
            string temp = shuffledList[i];
            shuffledList[i] = shuffledList[randomIndex];
            shuffledList[randomIndex] = temp;
        }
        return shuffledList;
    }

    private void SaveToFile(string filePath, List<string> content)
    {
        try
        {
            File.WriteAllLines(filePath, content);
            Debug.Log($"Results successfully saved to {filePath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error writing to file: {ex.Message}");
        }
    }
}
