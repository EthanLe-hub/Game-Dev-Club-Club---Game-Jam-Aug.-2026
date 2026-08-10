using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    // What the dialogue panel is currently doing:
    private enum State { Idle, Menu, Lines, Inspect }
    private State state = State.Idle;

    private Character speaking;
    private DialogueParent dialogueFile;

    private string[] currentLine;
    private int curIndex;
    private bool canAdvance = false;

    [SerializeField] public Button button; // Prefab used for the Talk / Inspect menu buttons.

    [SerializeField] GameObject textObject;
    [SerializeField] TextMeshProUGUI textMP;
    [SerializeField] GameObject options;

    // INSPECT UI (popup graphic + one line of text)
    [SerializeField] GameObject inspectPanel;
    [SerializeField] RawImage inspectImage; // RawImage (not Image) because inspect graphics are Texture2D assets.
    [SerializeField] TextMeshProUGUI inspectText;

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && canAdvance)
        {
            if (state == State.Lines) next();
            else if (state == State.Inspect) endDialogue();
        }

        // esc always exits.
        if (Keyboard.current.escapeKey.wasPressedThisFrame) endDialogue();
    }

    public void HideInspectUI()
    {
        if (inspectPanel != null) inspectPanel.SetActive(false);
    }

    public void startDialogue(Character c)
    {
        HideInspectUI(); // In case the popup was left visible (e.g. still active in the editor scene).

        speaking = c;
        dialogueFile = speaking.dialogue;

        // intro line for day 0
        if (GameManager.Instance.currentDay == 0)
        {
            GameManager.Instance.SpendChoice();
            PlayLines(dialogueFile.day0);
            return;
        }

        ShowMenu();
    }

    // talking / inspecting options

    private void ShowMenu()
    {
        state = State.Menu;
        canAdvance = false;

        textObject.SetActive(false);
        ClearOptionButtons();

        Button talkButton = Instantiate(button, options.transform);
        talkButton.GetComponentInChildren<TextMeshProUGUI>().text = "Talk";
        talkButton.onClick.AddListener(TalkChosen);

        Button inspectButton = Instantiate(button, options.transform);
        inspectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Inspect";
        inspectButton.onClick.AddListener(InspectChosen);

        options.SetActive(true);
    }

    private void TalkChosen()
    {
        GameManager.Instance.SpendChoice(); // Talking costs one of the day's choices.

        List<string> lines = new List<string>();

        // React to last night's death first (if anyone died and this character has a reaction for them):
        Character dead = GameManager.Instance.characterJustKilled;
        if (dead != null &&
            dialogueFile.deathDialogue[speaking.isImposter ? 1 : 0].TryGetValue(dead.GetType().Name, out string[] deathLines))
        {
            lines.AddRange(deathLines);
        }

        // Then the character's own dialogue: imposter act, a clue, or a fallback if no clue applies:
        if (speaking.isImposter) lines.AddRange(dialogueFile.talkLinesImposter);
        else if (GameManager.Instance.ImposterIsActive) lines.Add(BuildClueLine());
        else lines.AddRange(dialogueFile.talkLinesFallback);

        PlayLines(lines.ToArray());
    }

    private void InspectChosen()
    {
        GameManager.Instance.SpendChoice(); // Inspecting costs one of the day's choices.

        ClearOptionButtons();
        options.SetActive(false);
        textObject.SetActive(false);

        string line = speaking.isImposter ? dialogueFile.inspectLineImposter : dialogueFile.inspectLineInnocent;
        Texture2D graphic = speaking.isImposter ? speaking.inspectSpriteImposter : speaking.inspectSpriteInnocent;

        if (inspectPanel != null)
        {
            inspectPanel.SetActive(true);
            inspectImage.texture = graphic;
            inspectImage.enabled = graphic != null;
            inspectText.text = line;

            state = State.Inspect;
            canAdvance = false;
            StartCoroutine(EnableDialogueAfterFrame());
        }
        else
        {
            // Popup UI not wired up yet - show the line as regular dialogue so inspect still works:
            PlayLines(new[] { line });
        }
    }

    // fills a random clue template: {name} = suspect, {deadCharacter} = last night's victim.
    private string BuildClueLine()
    {
        Character dead = GameManager.Instance.characterJustKilled;

        // only adding if someone died.
        List<string> usable = new List<string>();
        foreach (string template in dialogueFile.talkClueTemplates)
        {
            if (dead == null && template.Contains("{deadCharacter}")) continue;
            usable.Add(template);
        }

        if (usable.Count == 0) return dialogueFile.talkLinesFallback[0];

        // pick a random suspect.
        Character suspect = GameManager.Instance.GetClueSuspect(speaking);
        string line = usable[Random.Range(0, usable.Count)];
        line = line.Replace("{name}", suspect.DisplayName);
        if (dead != null) line = line.Replace("{deadCharacter}", dead.DisplayName);

        return line;
    }

    // playing lines

    private void PlayLines(string[] lines)
    {
        state = State.Lines;

        ClearOptionButtons();
        options.SetActive(false);
        textObject.SetActive(true);

        currentLine = lines;
        curIndex = 0;
        textMP.text = currentLine[curIndex];

        canAdvance = false;
        StartCoroutine(EnableDialogueAfterFrame());
    }

    private void next()
    {
        curIndex++;
        if (curIndex < currentLine.Length) textMP.text = currentLine[curIndex];
        else endDialogue();
    }

    private IEnumerator EnableDialogueAfterFrame()
    {
        yield return null;
        canAdvance = true;
    }

    // removal

    public void endDialogue()
    {
        state = State.Idle;
        canAdvance = false;

        ClearOptionButtons();
        if (inspectPanel != null) inspectPanel.SetActive(false);

        UIManager.Instance.endDialogue();
    }

    private void ClearOptionButtons()
    {
        for (int i = options.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(options.transform.GetChild(i).gameObject);
        }
    }
}
