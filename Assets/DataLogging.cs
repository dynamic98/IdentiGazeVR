using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataLogging : MonoBehaviour
{
    public DifferentImagePerEye differentImagePerEye; // DifferentImagePerEye 스크립트 참조
    public LeftEyeGaze leftEyeGaze;                 // LeftEyeGaze 스크립트 참조
    public RightEyeGaze rightEyeGaze;               // RightEyeGaze 스크립트 참조

    private string filePath;

    private void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "final_log.txt");
        Debug.Log($"Log File Path: {filePath}");
    }

    public void SaveToText()
    {
        if (differentImagePerEye == null || leftEyeGaze == null || rightEyeGaze == null)
        {
            Debug.LogError("모든 데이터를 가져올 스크립트를 연결해주세요!");
            return;
        }

        // `participant`와 `session` 데이터를 DifferentImagePerEye에서 가져오기
        string participant = differentImagePerEye.participant;
        string session = differentImagePerEye.session;

        // 텍스트 데이터 구조 생성
        string logData = $"Participant: {participant}\n" +
                         $"Session: {session}\n" +
                         $"Stimuli Data: \n{string.Join("\n", differentImagePerEye.ImageLogData ?? new List<string>())}\n" +
                         $"Left Eye Data: \n{string.Join("\n", leftEyeGaze.LeftlogData ?? new List<string>())}\n" +
                         $"Right Eye Data: \n{string.Join("\n", rightEyeGaze.RightlogData ?? new List<string>())}\n";

        // 텍스트 파일 저장
        try
        {
            File.WriteAllText(filePath, logData);
            Debug.Log($"Log saved to: {filePath}");
        }
        catch (IOException e)
        {
            Debug.LogError($"Failed to save log: {e.Message}");
        }
    }


}