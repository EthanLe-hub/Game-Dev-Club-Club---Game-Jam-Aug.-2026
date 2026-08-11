using UnityEngine;
using UnityEngine.SceneManagement;


public class TitleScreenUI : MonoBehaviour
{
    [SerializeField] string gameSceneName = "Main"; // Scene loaded when Start is pressed.
    [SerializeField] GameObject creditsPanel; 

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        creditsPanel.SetActive(false); 
    }

    public void OnStartPressed()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnQuitPressed()
    {
        Application.Quit();
    }

    public void OnCreditsPressed()
    {
        creditsPanel.SetActive(true); 
    }

    public void OnCreditsClosedPressed()
    {
        creditsPanel.SetActive(false); 
    }
}
