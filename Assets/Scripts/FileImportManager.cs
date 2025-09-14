using SFB;               // StandaloneFileBrowser
using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FileImportManager : MonoBehaviour
{
    public static FileImportManager Instance;
    bool isStarted = false;
    public string chart = "";
    public string filePath;
    public double bpm;
    public Button openFileButton; // 인스펙터에서 버튼 연결
    public InputField BPMInput;
    private Process pythonProcess;

    [Header("설정")]
    public string pythonExeName = "python";       // 빌드 환경에 맞게 python 실행 파일 이름
    public string pythonServerRelativePath = "SERVER/model.py"; // StreamingAssets 또는 빌드 포함 경로


    void Start()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }   


        if (openFileButton != null)
        {
            openFileButton.onClick.AddListener(OpenFileBrowser);
        }

    }

    private void Update()
    {
        if (chart != "" && !isStarted) 
        {
            SceneManager.LoadScene(1);
            isStarted = true;
            pythonProcess.Kill();
            pythonProcess.Dispose();
        }
    }

    void OpenFileBrowser()
    {
        UnityEngine.Debug.Log("Clicked!");
        var extensions = new[] {
            new ExtensionFilter("모든 파일", "*"),
        };

        string[] paths = StandaloneFileBrowser.OpenFilePanel("파일 선택", "", extensions, false);

        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            bpm = double.Parse(BPMInput.text);
            filePath = paths[0];
            UnityEngine.Debug.Log("선택한 파일: " + filePath);

            StartPythonScript(filePath, bpm);
        }
    }

    void StartPythonScript(string filename, double bpm)
    {
        string serverPath = Path.Combine(Application.streamingAssetsPath, pythonServerRelativePath);

        if (!File.Exists(serverPath))
        {
            UnityEngine.Debug.LogError("Python 서버 스크립트를 찾을 수 없습니다: " + serverPath);
            return;
        }

        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName =  pythonExeName;
            startInfo.Arguments = serverPath  + " " + filename + " " + bpm;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = true;

            pythonProcess = new Process();
            pythonProcess.StartInfo = startInfo;
            pythonProcess.OutputDataReceived += (sender, e) =>
            {
                if (string.IsNullOrEmpty(e.Data)) return;

                LogUpdater.Instance.log += e.Data + "\n";

                if (e.Data.StartsWith("#00"))
                {
                    // '#' 으로 시작하는 경우 특별 처리
                    UnityEngine.Debug.Log("[SPECIAL] " + e.Data);
                    // 필요하다면 여기서 Unity 이벤트 호출이나 데이터 저장 가능
                    chart = e.Data;
                }
                else
                {
                    UnityEngine.Debug.Log(e.Data);
                }
            };
            pythonProcess.ErrorDataReceived += (sender, e) => {
                if (e.Data != null)
                {
                    LogUpdater.Instance.log += e.Data + "\n";
                    UnityEngine.Debug.LogError(e.Data);
                }
            };
            pythonProcess.Start();
            pythonProcess.BeginOutputReadLine();
            pythonProcess.BeginErrorReadLine();

            UnityEngine.Debug.Log("Python 서버 실행 완료.");
            
        }
        catch (Exception e)
        {
            LogUpdater.Instance.log += e.Data;
            UnityEngine.Debug.LogError("Python 서버 실행 실패: " + e.Message);
        }
        return;
    }
}
