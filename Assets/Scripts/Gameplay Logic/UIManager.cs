using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] DialogueManager dialoguePanel;
    [SerializeField] Player player;

    [SerializeField] Character test;

    void Awake()
    {
        if (Instance != null) // If instance already exists, destroy newly created copy. 
        {
            Destroy(gameObject);
            return;
        }

        // Set this first instance to be the static instance.
        // Not DontDestroyOnLoad for the same reason as GameManager: this scene never unloads
        // mid-game (judgement is additive), and persisting would break returning to the title screen.
        Instance = this;

        dialoguePanel.HideInspectUI();
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void startDialogue(Character c)
    {
        GameManager.Instance.pause();
        player.isTalking = true;

        // Free the cursor so the Talk/Inspect buttons are clickable. This must happen here:
        // pause() disables the Player script, so its own cursor handling is frozen during dialogue.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        dialoguePanel.gameObject.SetActive(true);
        dialoguePanel.startDialogue(c);
    }

    public void endDialogue()
    {
        dialoguePanel.gameObject.SetActive(false);
        player.isTalking = false;

        // Re-lock the cursor for first-person control (Player.cs takes over again once unpaused):
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameManager.Instance.unpause();
    }
}
