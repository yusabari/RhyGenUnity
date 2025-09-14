using UnityEngine;
using UnityEngine.UI;

public class LogUpdater : MonoBehaviour
{
    public static LogUpdater Instance;
    public string log = "";
    public Text logText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        logText.text = log;
    }
}
