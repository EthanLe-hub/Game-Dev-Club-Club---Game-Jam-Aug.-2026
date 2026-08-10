using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    // Set by GameManager.EndGame() right before loading this scene (static so it survives the scene change):
    public static bool PlayerWon = false;

    [SerializeField] GameObject winGraphic;  // Shown when the imposter was caught.
    [SerializeField] GameObject loseGraphic; // Shown when the imposter won.

    [SerializeField] string gameSceneName = "Main";
    [SerializeField] string titleSceneName = "TitleScreen";

    void Start()
    {
        // Cursor must be usable here (it is locked during gameplay):
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Show only the graphic matching the outcome:
        if (winGraphic != null) winGraphic.SetActive(PlayerWon);
        if (loseGraphic != null) loseGraphic.SetActive(!PlayerWon);
    }

    // Starts a completely fresh run (Main scene reloads, GameManager re-rolls the imposter):
    public void OnRetryPressed()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnTitlePressed()
    {
        SceneManager.LoadScene(titleSceneName);
    }

    // Does nothing in the editor, quits in a build:
    public void OnQuitPressed()
    {
        Application.Quit();
    }
}
