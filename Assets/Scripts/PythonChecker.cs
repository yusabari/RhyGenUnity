using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Text;

public class PythonChecker : MonoBehaviour
{
    public Text responseText; // UI Text�� �ν����Ϳ� �����ϼ���.
    public string serverUrl = "http://localhost:4470/check";

    void Start()
    {
        // 5�ʸ��� �ݺ� ����
        InvokeRepeating(nameof(SendRequestToServer), 0f, 5f);
    }

    void SendRequestToServer()
    {
        StartCoroutine(GetFromPythonServer());
    }

    IEnumerator GetFromPythonServer()
    {
        // GET ��û (���� �Ķ���ͷ� �޽��� ����)
        string url = serverUrl;
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("���� ����: " + request.downloadHandler.text);
            if (responseText != null)
                responseText.text = "Server Online";
        }
        else
        {
            Debug.LogError("��û ����: " + request.error);
            if (responseText != null)
                responseText.text = "Server Offline";
        }
    }
}
