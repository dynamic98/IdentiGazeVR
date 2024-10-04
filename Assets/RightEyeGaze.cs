using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class RightEyeGaze : MonoBehaviour
{
    public OVREyeGaze rightEyeGaze;
    public OVRFaceExpressions faceComponent;

    // LineRenderer를 각각의 눈에 대해 추가
    private LineRenderer rightEyeLineRenderer;

    private string filePath;
    private float timeElapsed = 0f;
    private float saveInterval = 1f;
    private bool isRecording = true;

    private List<Vector3> rightEyeGazeDataList = new List<Vector3>();

    void Start()
    {
        // 파일 경로 설정
        filePath = Path.Combine(Application.persistentDataPath, "RightEyeTrackingLog.txt");
        Debug.Log(filePath);
        File.WriteAllText(filePath, "Right Gaze Data Log\n");

        // LineRenderer 설정
        rightEyeLineRenderer = CreateLineRenderer(Color.red);
    }

    void Update()
    {
        if (!isRecording || (rightEyeGaze == null)) return;


        // 눈 깜박임 상태 확인
        float rightBlinkWeight;
        faceComponent.TryGetFaceExpressionWeight(OVRFaceExpressions.FaceExpression.EyesClosedR, out rightBlinkWeight);

        // 눈이 감겨있는지 확인 (예: 0.5 이상이면 반 이상 감겨있는 상태로 간주)
        if (rightBlinkWeight >= 0.5f)
        {
            Debug.Log("Right eye is closed. Logging closed state.");
            LogEyeClosed();  // 눈이 감긴 상태를 파일에 기록
            return;
        }
        // 오른쪽 눈 시선 데이터 처리
        if (rightEyeGaze != null && rightEyeGaze.EyeTrackingEnabled)
        {
            ProcessGazeData(rightEyeGaze, rightEyeGazeDataList, "Right Eye", rightEyeLineRenderer);
        }

        // 1초마다 데이터를 저장
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= saveInterval)
        {
            SaveGazeData();
            Debug.Log("Gaze Data Saved: " + filePath);

            rightEyeGazeDataList.Clear();
            timeElapsed = 0f;
        }
    }


    // 눈이 감긴 상태를 텍스트 파일에 기록하는 함수
    void LogEyeClosed()
    {
        string currentTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string log = currentTime + " - Right eye is closed.\n";
        File.AppendAllText(filePath, log);
    }

    // LineRenderer를 생성하는 함수
    LineRenderer CreateLineRenderer(Color color)
    {
        GameObject lineObj = new GameObject("LineRenderer");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();

        lr.startWidth = 0.02f;
        lr.endWidth = 0.02f;
        lr.positionCount = 2; // 시작점과 끝점으로 구성된 2개의 점
        lr.material = new Material(Shader.Find("Sprites/Default")); // 기본 Shader 사용
        lr.startColor = color;
        lr.endColor = color;
        lr.useWorldSpace = true;

        return lr;
    }

    void ProcessGazeData(OVREyeGaze eyeGaze, List<Vector3> gazeDataList, string eyeName, LineRenderer lineRenderer)
    {
        Vector3 gazeOrigin = eyeGaze.transform.position;
        Vector3 gazeDirection = eyeGaze.transform.forward;

        RaycastHit hitInfo;
        if (Physics.Raycast(gazeOrigin, gazeDirection, out hitInfo))
        {
            Vector3 hitPoint = hitInfo.point;
            Debug.Log(eyeName + " Hit Point: " + hitPoint);

            // Ray의 시작점과 끝점을 LineRenderer로 설정
            lineRenderer.SetPosition(0, gazeOrigin);
            lineRenderer.SetPosition(1, hitPoint);

            // 히트된 오브젝트가 Cube인지 확인
            if (hitInfo.collider.CompareTag("Cube"))
            {
                gazeDataList.Add(hitPoint);
                Debug.Log(eyeName + " Cube hit, data added");
            }
            else
            {
                Debug.Log(eyeName + " Hit object is not Cube");
            }
        }
        else
        {
            Debug.Log(eyeName + " No hit");
            // Raycast가 맞지 않았을 때, Ray를 화면 밖으로 보내는 식으로 처리 가능
            // lineRenderer.SetPosition(0, gazeOrigin);
            // lineRenderer.SetPosition(1, gazeOrigin + gazeDirection * 1000f); // 최대 거리까지
        }
    }


    void SaveGazeData()
    {
        string currentTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        string log = currentTime + " - Right Eye Gaze Data:\n";
        foreach (Vector3 gazePoint in rightEyeGazeDataList)
        {
            log += "Right Eye Gaze Position: " + gazePoint.ToString() + "\n";
        }

        File.AppendAllText(filePath, log);
    }

    public void StopRecording()
    {
        isRecording = false;
        Debug.Log("Stopped recording gaze data.");
    }

    public void StartRecording()
    {
        isRecording = true;
        Debug.Log("Started recording gaze data.");
    }
}
