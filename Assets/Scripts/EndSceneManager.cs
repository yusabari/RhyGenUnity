using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndSceneManager : MonoBehaviour
{
    public Button RetryButton;
    public Button ExitButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (RetryButton != null)
        {
            RetryButton.onClick.AddListener(OnClickRetry);
            ExitButton.onClick.AddListener(OnClickExit);
        }
    }

    void OnClickRetry()
    {
        SceneManager.LoadScene(1);
    }

    void OnClickExit()
    {
        UnityEngine.Application.Quit();
    }
}
