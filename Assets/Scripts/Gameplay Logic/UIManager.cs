using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] Dialogue dialoguePanel;
    [SerializeField] Player player;

    [SerializeField] Character test;

    bool panelOpen = false;

    void Awake()
    {
        if (Instance != null) // If instance already exists, destroy newly created copy. 
        {
            Destroy(gameObject);
            return;
        }

        // Otherwise, set this first instance to be the static instance:
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void startDialogue(Character c)
    {
        panelOpen = true;
        player.isTalking = true;
        dialoguePanel.gameObject.SetActive(true);
        dialoguePanel.startDialogue(c);
    }

    public void endDialogue()
    {
        dialoguePanel.endDialogue();
        dialoguePanel.gameObject.SetActive(false);
        player.isTalking = false;
        panelOpen = false;
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && !panelOpen)
        {
            startDialogue(test);
        }
    }
}
