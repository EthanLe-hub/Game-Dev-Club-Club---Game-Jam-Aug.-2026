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

        // Otherwise, set this first instance to be the static instance:
        Instance = this;
        DontDestroyOnLoad(gameObject);

        dialoguePanel.HideInspectUI();
    }

    public void startDialogue(Character c)
    {
        GameManager.Instance.pause();
        player.isTalking = true;
        dialoguePanel.gameObject.SetActive(true);
        dialoguePanel.startDialogue(c);
    }

    public void endDialogue()
    {
        dialoguePanel.gameObject.SetActive(false);
        player.isTalking = false;
        GameManager.Instance.unpause();
    }
}
