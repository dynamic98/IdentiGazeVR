using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine;

public class DifferentImagePerEye : MonoBehaviour
{
    public OVRCameraRig ovrCameraRig;
    public GameObject CenterContent;
    public GameObject LeftContent;
    public GameObject RightContent;
    public GameObject BlackoutContent;

    private Camera leftEyeCamera;
    private Camera rightEyeCamera;
    private Renderer leftRenderer;
    private Renderer rightRenderer;

    public float centerDuration = 0.8f;
    public float firstBlackoutDuration = 0.2f;
    public float leftRightDuration = 0.7f;
    public float secondBlackoutDuration = 0.3f;

    private List<string> groups; // 랜덤으로 섞인 그룹 리스트
    private Dictionary<string, List<int>> groupIndices;
    private Dictionary<string, List<Texture>> groupImages;

    public string participant = "p0"; // 참가자 ID
    public string session = "s1";        // 세션 ID

    public List<string> ImageLogData = new List<string>();

    public DataLogging dataLogger; // DataLogging 스크립트 참조

    private void Start()
    {
        leftEyeCamera = ovrCameraRig.leftEyeCamera;
        rightEyeCamera = ovrCameraRig.rightEyeCamera;

        LeftContent.layer = LayerMask.NameToLayer("LeftEyeOnly");
        RightContent.layer = LayerMask.NameToLayer("RightEyeOnly");

        leftEyeCamera.cullingMask = LayerMask.GetMask("LeftEyeOnly", "Default");
        rightEyeCamera.cullingMask = LayerMask.GetMask("RightEyeOnly", "Default");

        leftRenderer = LeftContent.GetComponent<Renderer>();
        rightRenderer = RightContent.GetComponent<Renderer>();

        // JSON 데이터와 이미지 로드
        groupIndices = LoadIndicesFromJson(participant, session);
        groupImages = LoadGroupImages();

        // 그룹 순서 섞기
        groups = new List<string>(groupIndices.Keys);
        groups = ShuffleList(groups);

        // 바로 실행
        StartCoroutine(DisplaySequence());
    }

    private IEnumerator DisplaySequence()
{
    bool hasMoreIndices = true;

    while (hasMoreIndices)
    {
        hasMoreIndices = groups.Any(group => groupIndices.ContainsKey(group) && groupIndices[group].Count > 0);
        if (!hasMoreIndices)
        {
            Debug.Log("All indices have been processed.");
            break;
        }


        foreach (string group in groups)
        {
            if (group == "t1")
            {
                SetActiveContent(CenterContent);
                yield return new WaitForSeconds(centerDuration);

                SetActiveContent(BlackoutContent);
                yield return new WaitForSeconds(firstBlackoutDuration);

                ApplyBaselineImage();

                // t1 인덱스 소진 처리
                if (groupIndices[group].Count > 0)
                {
                    groupIndices[group].RemoveAt(0);
                }

                SetActiveContent(LeftContent, RightContent);
                yield return new WaitForSeconds(leftRightDuration);

                SetActiveContent(BlackoutContent);
                yield return new WaitForSeconds(secondBlackoutDuration);

                continue; // 다음 그룹으로 넘어감
            }

                if (!groupIndices.ContainsKey(group) || groupIndices[group].Count == 0)
                {
                    Debug.LogWarning($"No indices left for group: {group}");
                    continue;
                }

            SetActiveContent(CenterContent);
            yield return new WaitForSeconds(centerDuration);
            LogStimulus("center", 0);

            SetActiveContent(BlackoutContent);
            yield return new WaitForSeconds(firstBlackoutDuration);
            LogStimulus("blackout", 0);

            // ApplyImageFromGroup 호출 후 반환값으로 로그 기록
            int appliedIndex = ApplyImageFromGroup(group); // 반환된 인덱스
            if (appliedIndex >= 0)
            {
                LogStimulus(group, appliedIndex); // 반환된 값으로 로그 기록
            }


            SetActiveContent(LeftContent, RightContent);
            yield return new WaitForSeconds(leftRightDuration);

            SetActiveContent(BlackoutContent);
            yield return new WaitForSeconds(secondBlackoutDuration);
            LogStimulus("blackout", 0);
        }
    }

    if (dataLogger != null)
    {
        Debug.Log("Triggering DataLogging.SaveToText()");
        dataLogger.SaveToText();
    }
    else
    {
        Debug.LogWarning("DataLogger is not assigned!");
    }
}


    private void ApplyBaselineImage()
    {
        Texture baselineTexture = Resources.Load<Texture>("Images/baseline");
        if (baselineTexture != null)
        {
            leftRenderer.material.mainTexture = baselineTexture;
            rightRenderer.material.mainTexture = baselineTexture;
            Debug.Log("Applied baseline texture for group t1.");
        }
        else
        {
            Debug.LogWarning("Baseline texture not found in Resources/Images/baseline");
        }
    }

    private void LogStimulus(string stimulus, int index)
    {
        string currentTime = System.DateTime.Now.ToString("HH:mm:ss.fff");
        string log = $"{currentTime} / {stimulus} / {index}";
        ImageLogData.Add(log);

        Debug.Log($"Logged: {log}");
    }

    private Dictionary<string, List<int>> LoadIndicesFromJson(string participant, string session)
{
    Dictionary<string, List<int>> indices = new Dictionary<string, List<int>>();

    try
    {
        TextAsset jsonText = Resources.Load<TextAsset>("Index/pilot_index");
        if (jsonText == null)
        {
            Debug.LogError("JSON file not found in Resources/Index/pilot_index");
            return indices;
        }

        string jsonContent = jsonText.text;
        JObject json = JObject.Parse(jsonContent);

        var sessionData = json[participant]?[session];
        if (sessionData == null)
        {
            Debug.LogError($"Session data not found for participant {participant}, session {session}");
            return indices;
        }

        // 각 그룹에 대한 데이터 확인 및 로그 출력
        Debug.Log($"Checking data for participant: {participant}, session: {session}");


        // 각 그룹 데이터를 딕셔너리에 추가

        //수정
        indices["t1"] = Enumerable.Range(0, 27).ToList(); // t1에 대해 27개 (T1~T4 섞는 경우)
        // indices["t1"] = Enumerable.Empty<int>().ToList(); // 각 T2~T4까지         
        indices["t2"] = sessionData["t2"]?.ToObject<List<int>>() ?? new List<int>();
        indices["t3r"] = sessionData["t3r"]?.ToObject<List<int>>() ?? new List<int>();
        indices["t3l"] = sessionData["t3l"]?.ToObject<List<int>>() ?? new List<int>();
        indices["t4"] = sessionData["t4"]?.ToObject<List<int>>() ?? new List<int>();


        Debug.Log($"t1 data: manually set (count: {indices["t1"].Count})");
        Debug.Log($"t2 data: {(sessionData["t2"] == null ? "null" : "exists")}");
        Debug.Log($"t3r data: {(sessionData["t3r"] == null ? "null" : "exists")}");
        Debug.Log($"t3l data: {(sessionData["t3l"] == null ? "null" : "exists")}");
        Debug.Log($"t4 data: {(sessionData["t4"] == null ? "null" : "exists")}");

        // 데이터가 비어 있는 그룹 로그 출력 및 제거
        foreach (var group in indices.Keys.ToList())
        {
            if (indices[group] == null || indices[group].Count == 0)
            {
                Debug.Log($"Removing empty group: {group}");
                indices.Remove(group);
            }
        }
    }
    catch (System.Exception ex)
    {
        Debug.LogError($"Error reading JSON file: {ex.Message}");
    }

    return indices;
}


    private Dictionary<string, List<Texture>> LoadGroupImages()
    {
        Dictionary<string, List<Texture>> images = new Dictionary<string, List<Texture>>();

        images["t1"] = new List<Texture> { Resources.Load<Texture>("Images/baseline") };
        images["t2"] = new List<Texture>(Resources.LoadAll<Texture>("Images/stimuli_combined")).OrderBy(texture => texture.name).ToList();
        images["t3r"] = new List<Texture>(Resources.LoadAll<Texture>("Images/stimuli_combined")).OrderBy(texture => texture.name).ToList();
        images["t3l"] = new List<Texture>(Resources.LoadAll<Texture>("Images/stimuli_combined")).OrderBy(texture => texture.name).ToList();
        images["t4l"] = new List<Texture>(Resources.LoadAll<Texture>("Images/stimuli_left")).OrderBy(texture => texture.name).ToList();
        images["t4r"] = new List<Texture>(Resources.LoadAll<Texture>("Images/stimuli_right")).OrderBy(texture => texture.name).ToList();

        Debug.Log($"Loaded {images["t4l"].Count} textures for t4l and {images["t4r"].Count} textures for t4r.");



        return images;
    }

private int ApplyImageFromGroup(string group)
{
    // 그룹이 존재하지 않거나 리스트가 비어 있는지 확인
    if (!groupIndices.ContainsKey(group) || groupIndices[group] == null || groupIndices[group].Count == 0)
    {
        Debug.LogWarning($"No indices left for group: {group}");
        return -1;
    }

    // JSON에서 가져온 원래 인덱스 값 가져오기
    int imageIndex = groupIndices[group][0]; // 제거하지 않고 첫 번째 값만 가져옴
    // 인덱스 제거 처리
    groupIndices[group].RemoveAt(0);

    // // groupImages[group]의 범위를 벗어나는 인덱스인지 확인
    // if (!groupImages.ContainsKey(group) || groupImages[group] == null || imageIndex >= groupImages[group].Count)
    // {
    //     Debug.LogError($"Invalid imageIndex for group {group}. Index: {imageIndex}, Available: {groupImages[group]?.Count ?? 0}");
    //     return -1;
    // }

    // t1 그룹 처리
    if (group == "t1")
    {
        ApplyBaselineImage(); // t1은 항상 같은 이미지 사용
        Debug.Log($"t1 processed with index {imageIndex}");
        return -1;
    }

    // 그룹별로 이미지 적용
    switch (group)
    {
        case "t2":
            Texture t2Texture = groupImages[group][imageIndex];
            leftRenderer.material.mainTexture = t2Texture;
            rightRenderer.material.mainTexture = t2Texture;
            break;

        case "t3r":
            Texture t3rTexture = groupImages[group][imageIndex];
            Texture t3rBlackout = Resources.Load<Texture>("Images/blackout");
            leftRenderer.material.mainTexture = t3rBlackout;
            rightRenderer.material.mainTexture = t3rTexture;
            break;

        case "t3l":
            Texture t3lTexture = groupImages[group][imageIndex];
            Texture t3lBlackout = Resources.Load<Texture>("Images/blackout");
            leftRenderer.material.mainTexture = t3lTexture;
            rightRenderer.material.mainTexture = t3lBlackout;
            break;

        case "t4":
            Texture t4LeftImage = groupImages["t4l"][imageIndex];
            Texture t4RightImage = groupImages["t4r"][imageIndex];
            leftRenderer.material.mainTexture = t4LeftImage;
            rightRenderer.material.mainTexture = t4RightImage;
            break;


        default:
            Debug.LogWarning($"Unknown group: {group}");
            break;
    }



    // 로그 기록 시 JSON 인덱스 값 사용
    Debug.Log($"Applied image for group {group}, index {imageIndex}");
    return imageIndex; // 적용한 인덱스를 반환
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

    private void SetActiveContent(GameObject activeContent1 = null, GameObject activeContent2 = null)
    {
        CenterContent.SetActive(activeContent1 == CenterContent);
        LeftContent.SetActive(activeContent1 == LeftContent || activeContent2 == LeftContent);
        RightContent.SetActive(activeContent1 == RightContent || activeContent2 == RightContent);
        BlackoutContent.SetActive(activeContent1 == BlackoutContent);
    }
}
