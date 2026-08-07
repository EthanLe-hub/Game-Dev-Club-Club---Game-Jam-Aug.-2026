using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;

public class Dialogue : MonoBehaviour
{
    private Character speaking;
    private DialogueParent dialogueFile;

    private Dictionary<DialogueParent.Section, string[]> dict;
    private string[] currentLine;
    private int day;
    private int curIndex;

    GameObject textObject;
    TextMeshProUGUI textMP;
    GameObject options;

    void Awake()
    {
        textObject = transform.GetChild(1).gameObject;
        textMP = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        options = transform.GetChild(2).gameObject;
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            next();
        }
    }

    public void startDialogue(Character c)
    {
        if (c == null) Debug.Log("Character c is null");
        if (c.dialogue == null) Debug.Log("Dialogue file is null");


        textObject.SetActive(true);
        options.SetActive(false);

        speaking = c;
        dialogueFile = speaking.dialogue;
        day = GameManager.Instance.day;
        switch (day)
        {
            case 1:
                dict = dialogueFile.day1;
                break;
            default:
                Debug.LogError("Day index outside of bounds: " + day);
                break;
        }

        currentLine = dict[DialogueParent.Section.Intro];
        curIndex = 0;
        textMP.text = currentLine[curIndex];
    }
    
    private void next()
    {
        curIndex++;
        if (curIndex < currentLine.Length)
        {
            textMP.text = currentLine[curIndex];
        }
        else
        {
            UIManager.Instance.endDialogue();
        }
    }
}
