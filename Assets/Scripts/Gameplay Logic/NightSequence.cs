using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;


public class NightSequence : MonoBehaviour
{
    // Assign these in Unity Inspector:
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip knockClip;

    [SerializeField] GameObject textPanel; // Panel holding the narration text below.
    [SerializeField] TextMeshProUGUI narrationText;

    [SerializeField] GameObject doorChoicePanel; // Panel holding the two buttons below.
    [SerializeField] Button openButton;
    [SerializeField] Button keepClosedButton;

    [SerializeField] float delayAfterKnock = 1.5f; // Seconds between the knock sound and the narration text appearing.

    [SerializeField]
    string[] knockLinesImposter =
    {
        "*knock... knock... knock*",
        "The knocking is slow.",
        "Something about it makes your skin crawl."
    };

    [SerializeField]
    string[] knockLinesInnocent =
    {
        "*knock knock knock*",
        "A hurried knocking at your door.",
        "Whoever is out there sounds anxious."
    };

    string[] currentLine;
    int curIndex;
    bool canAdvance = false;
    Action onLinesDone; // Invoked once the player has read through the current lines.

    // True while narration lines are on screen. GameManager checks this so that the Space press
    // advancing the text does not simultaneously count as interacting with the door (Space is
    // also the interact key, and the door is right in front of the player at night):
    public bool IsShowingLines { get; private set; }

    void Awake()
    {
        if (textPanel != null) textPanel.SetActive(false);
        if (doorChoicePanel != null) doorChoicePanel.SetActive(false);

        if (openButton != null) openButton.onClick.AddListener(() => Choose(true));
        if (keepClosedButton != null) keepClosedButton.onClick.AddListener(() => Choose(false));
    }

    void Update()
    {
        if (canAdvance && Keyboard.current.spaceKey.wasPressedThisFrame) next();
    }

    // Called by GameManager once the night transition has fully faded back in:
    public void BeginKnock(bool visitorIsImposter)
    {
        StartCoroutine(KnockRoutine(visitorIsImposter));
    }

    IEnumerator KnockRoutine(bool visitorIsImposter)
    {
        if (audioSource != null && knockClip != null) audioSource.PlayOneShot(knockClip);

        yield return new WaitForSeconds(delayAfterKnock);

        PlayLines(visitorIsImposter ? knockLinesImposter : knockLinesInnocent, null);
    }

    // Called by GameManager when the player interacts with the door:
    public void ShowDoorChoice()
    {
        HideLines(); // Clear the knock narration if it is still up.
        if (doorChoicePanel != null) doorChoicePanel.SetActive(true);
    }

    void Choose(bool open)
    {
        if (doorChoicePanel != null) doorChoicePanel.SetActive(false);
        GameManager.Instance.OpenDoor(open);
    }

    // Shows the innocent visitor's final hint; calls onDone once the player has read it:
    public void ShowFinalHint(string speakerName, string hintLine, Action onDone)
    {
        PlayLines(new[] { speakerName + ": \"" + hintLine + "\"" }, onDone);
    }

    void PlayLines(string[] lines, Action onDone)
    {
        if (textPanel == null || narrationText == null || lines == null || lines.Length == 0)
        {
            onDone?.Invoke(); // UI not wired up yet (or no lines) - skip through so the game flow never stalls.
            return;
        }

        onLinesDone = onDone;
        currentLine = lines;
        curIndex = 0;

        IsShowingLines = true;
        textPanel.SetActive(true);
        narrationText.text = currentLine[curIndex];

        canAdvance = false;
        StartCoroutine(EnableAdvanceAfterFrame());
    }

    void next()
    {
        curIndex++;
        if (curIndex < currentLine.Length)
        {
            narrationText.text = currentLine[curIndex];
            return;
        }

        HideLines();

        Action done = onLinesDone; // Clear before invoking, in case the callback starts new lines.
        onLinesDone = null;
        done?.Invoke();
    }

    void HideLines()
    {
        canAdvance = false;
        IsShowingLines = false;
        if (textPanel != null) textPanel.SetActive(false);
    }

    // Waits one frame before accepting Space, so the key press that triggered these lines does not also advance them:
    IEnumerator EnableAdvanceAfterFrame()
    {
        yield return null;
        canAdvance = true;
    }
}
