using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    private Character speaking;
    private DialogueParent dialogueFile;

    private Dictionary<DialogueParent.Section, string[]> dict;
    private string[] currentLine;
    private int day;
    private int curIndex;
    private bool canAdvance = false;

    [SerializeField] public Button button;

    GameObject textObject;
    TextMeshProUGUI textMP;
    GameObject options;

    void Awake()
    {
        textObject = transform.GetChild(2).gameObject;
        textMP = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        options = transform.GetChild(3).gameObject;
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && canAdvance) next();
        if (Keyboard.current.escapeKey.wasPressedThisFrame) endDialogue();
    }

    public void startDialogue(Character c)
    {
        textObject.SetActive(true);
        options.SetActive(false);

        speaking = c;
        dialogueFile = speaking.dialogue;
        day = GameManager.Instance.currentDay;
        switch (day)
        {
            case 0:
                dict = null;
                currentLine = dialogueFile.day0;
                curIndex = 0;
                textMP.text = currentLine[curIndex];
                StartCoroutine(EnableDialogueAfterFrame());
                return;
            case 1:
                dict = dialogueFile.day1[c.isImposter ? 1 : 0];
                break;
            default:
                Debug.LogError("Day index outside of bounds: " + day);
                break;
        }

        string deadCharacter = GameManager.Instance.characterJustKilled.GetType().Name;
        dialogueFile.questions[day - 1, 0] = "What are your throughts on " + deadCharacter + "'s death?";
        dict[DialogueParent.Section.Q1] = dialogueFile.deathDialogue[c.isImposter ? 1 : 0][deadCharacter];

        currentLine = dict[DialogueParent.Section.Intro];
        curIndex = 0;
        textMP.text = currentLine[curIndex];


        StartCoroutine(EnableDialogueAfterFrame());
    }

    private IEnumerator EnableDialogueAfterFrame()
    {
        yield return null;
        canAdvance = true;
    }

    public void endDialogue()
    {
        canAdvance = false;

        for (int i = options.transform.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(options.transform.GetChild(i).gameObject);
        }

        UIManager.Instance.endDialogue();
    }


    private void next()
    {
        curIndex++;
        if (curIndex < currentLine.Length)
        {
            textMP.text = currentLine[curIndex];
        }
        else if (dict == null)
        {
            endDialogue();
        }
        else if (options.transform.childCount == 0)
        {
            for (int i=0; i<3; i++)
            {
                int index = i;

                Button b = Instantiate(button, options.transform);
                b.GetComponentInChildren<TextMeshProUGUI>().text = dialogueFile.questions[day - 1, i];
                b.onClick.AddListener(() => ButtonClicked(index));

                textObject.SetActive(false);
                options.SetActive(true);
            }
        }
    }

    private void ButtonClicked(int index)
    {
        switch (index)
        {
            case 0:
                currentLine = dict[DialogueParent.Section.Q1];
                break;
            case 1:
                currentLine = dict[DialogueParent.Section.Q2];
                break;
            case 2:
                currentLine = dict[DialogueParent.Section.Q3];
                break;
            default:
                Debug.LogError("Question Index out of Bounds: " + index);
                break;
        }

        curIndex = 0;
        textMP.text = currentLine[curIndex];

        for (int i = options.transform.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(options.transform.GetChild(i).gameObject);
        }

        textObject.SetActive(true);
        options.SetActive(false);
    }
}
