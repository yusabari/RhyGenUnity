using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Text;

public class PythonChecker : MonoBehaviour
{
    public Text responseText; // UI Text를 인스펙터에 연결하세요.
    public string serverUrl = "http://localhost:4470/check";

    void Start()
    {
        // 5초마다 반복 실행
        InvokeRepeating(nameof(SendRequestToServer), 0f, 5f);
    }

    void SendRequestToServer()
    {
        StartCoroutine(GetFromPythonServer());
    }

    IEnumerator GetFromPythonServer()
    {
        // GET 요청 (쿼리 파라미터로 메시지 전달)
        string url = serverUrl;
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("서버 응답: " + request.downloadHandler.text);
            if (responseText != null)
                responseText.text = "Server Online";
        }
        else
        {
            Debug.LogError("요청 실패: " + request.error);
            if (responseText != null)
                responseText.text = "Server Offline";
        }
    }
}
