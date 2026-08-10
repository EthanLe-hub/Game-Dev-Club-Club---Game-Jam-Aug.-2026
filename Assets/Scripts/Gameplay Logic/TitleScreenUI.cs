using UnityEngine;
using UnityEngine.SceneManagement;


public class TitleScreenUI : MonoBehaviour
{
    [SerializeField] string gameSceneName = "Main"; // Scene loaded when Start is pressed.

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnStartPressed()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnQuitPressed()
    {
        Application.Quit();
    }
}
