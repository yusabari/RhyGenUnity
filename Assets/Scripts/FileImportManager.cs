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
    public Button openFileButton; // �ν����Ϳ��� ��ư ����
    public InputField BPMInput;
    private Process pythonProcess;

    [Header("����")]
    public string pythonExeName = "python";       // ���� ȯ�濡 �°� python ���� ���� �̸�
    public string pythonServerRelativePath = "SERVER/model.py"; // StreamingAssets �Ǵ� ���� ���� ���


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
            new ExtensionFilter("��� ����", "*"),
        };

        string[] paths = StandaloneFileBrowser.OpenFilePanel("���� ����", "", extensions, false);

        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            bpm = double.Parse(BPMInput.text);
            filePath = paths[0];
            UnityEngine.Debug.Log("������ ����: " + filePath);

            StartPythonScript(filePath, bpm);
        }
    }

    void StartPythonScript(string filename, double bpm)
    {
        string serverPath = Path.Combine(Application.streamingAssetsPath, pythonServerRelativePath);

        if (!File.Exists(serverPath))
        {
            UnityEngine.Debug.LogError("Python ���� ��ũ��Ʈ�� ã�� �� �����ϴ�: " + serverPath);
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
                    // '#' ���� �����ϴ� ��� Ư�� ó��
                    UnityEngine.Debug.Log("[SPECIAL] " + e.Data);
                    // �ʿ��ϴٸ� ���⼭ Unity �̺�Ʈ ȣ���̳� ������ ���� ����
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

            UnityEngine.Debug.Log("Python ���� ���� �Ϸ�.");
            
        }
        catch (Exception e)
        {
            LogUpdater.Instance.log += e.Data;
            UnityEngine.Debug.LogError("Python ���� ���� ����: " + e.Message);
        }
        return;
    }
}
