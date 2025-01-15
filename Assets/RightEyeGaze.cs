using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RightEyeGaze : MonoBehaviour
{
    public OVREyeGaze rightEyeGaze;
    public OVRFaceExpressions faceComponent;

    private string filePath;
    private bool isRecording = true;
    private bool wasHMDWorn = true; // 이전 프레임에서 HMD가 착용된 상태였는지 확인

    // 모든 로그 데이터를 저장할 리스트
    public List<string> RightlogData = new List<string>();

    void Start()
    {
        // 파일 경로 설정
        filePath = Path.Combine(Application.persistentDataPath, "RightEyeTrackingLog.txt");
        Debug.Log(filePath);
    }

    void Update()
    {
        bool isHMDWorn = OVRPlugin.userPresent;

        // HMD 상태가 변경되었을 때 로그 기록
        if (isHMDWorn != wasHMDWorn)
        {
            if (!isHMDWorn)
            {
                LogHMDRemoved(); // 헤드셋이 벗겨진 상태를 파일에 기록
            }
            else
            {
                LogHMDWorn(); // 헤드셋이 다시 착용된 상태를 파일에 기록
            }
            wasHMDWorn = isHMDWorn;
        }

        // HMD를 착용하고 있지 않다면 시선 데이터를 처리하지 않음
        if (!isHMDWorn || !isRecording || (rightEyeGaze == null)) return;

        // 눈 깜박임 상태 확인
        float rightBlinkWeight;
        faceComponent.TryGetFaceExpressionWeight(OVRFaceExpressions.FaceExpression.EyesClosedR, out rightBlinkWeight);

        // 현재 시간
        string currentTime = System.DateTime.Now.ToString("HH:mm:ss.fff");

        // 시선 데이터 수집
        Vector3 gazeOrigin = rightEyeGaze.transform.position;
        Vector3 gazeDirection = rightEyeGaze.transform.forward;
        Vector3 gazePoint = gazeOrigin + gazeDirection * 10f; // 임의로 10미터 앞 지점을 시선으로 설정

        // 로그 데이터를 포맷에 맞게 저장
        string log;
        if (rightBlinkWeight >= 0.5f)
        {
            log = currentTime + "/Closed";
            Debug.Log("Right eye is closed.");
        }
        else
        {
            log = currentTime + "/Open/" + gazePoint.ToString();
            Debug.Log("Right Eye Gaze Position: " + gazePoint);
        }

        // 로그를 리스트에 추가
        RightlogData.Add(log);

    }

        // 헤드셋 벗겨진 상태를 기록하는 함수
    void LogHMDRemoved()
    {
        string currentTime = System.DateTime.Now.ToString("HH:mm:ss.fff");
        string log = currentTime + "/HMD Removed";
        RightlogData.Add(log);
        Debug.Log("HMD is removed.");
    }

    // 헤드셋 다시 착용된 상태를 기록하는 함수
    void LogHMDWorn()
    {
        string currentTime = System.DateTime.Now.ToString("HH:mm:ss.fff");
        string log = currentTime + "/HMD Worn";
        RightlogData.Add(log);
        Debug.Log("HMD is worn.");
    }

    public void StopRecording()
    {
        isRecording = false;
        Debug.Log("Stopped recording gaze data.");

        // 모든 로그 데이터를 파일에 저장
        File.WriteAllLines(filePath, RightlogData);

        // 로그 리스트 비우기
        RightlogData.Clear();
    }

    public void StartRecording()
    {
        isRecording = true;
        Debug.Log("Started recording gaze data.");
    }
}




