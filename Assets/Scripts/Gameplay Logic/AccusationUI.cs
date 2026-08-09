// Ethan Le (8/8/2026):
using TMPro; 
using UnityEngine;
using UnityEngine.UI; 

/** 
 * Script for the End-of-Day Judgement at the end of every night on the submarine.
**/
public class AccusationUI : MonoBehaviour
{
    [SerializeField] GameObject accusationPanel; // Parent panel. 
    [SerializeField] GameObject characterSelectionPanel; // First child panel. 
    [SerializeField] GameObject actionSelectionPanel; // Second child panel. 
    [SerializeField] GameObject confirmationPanel; // Third child panel. 
    [SerializeField] TextMeshProUGUI instructionDisplay; 
    [SerializeField] Button[] characterButtons; // On the first child panel. 
    [SerializeField] Button[] actionButtons; // On the second child panel. 
    [SerializeField] Button yesButton; // On third child panel. 
    [SerializeField] Button noButton; // On third child panel. 
    //[SerializeField] Character[] characterScripts; 

    string firstPanelInstruction = "Who do you want to accuse tonight?"; 

    string previousPanelInstruction; 

    string confirmDoNothing = "Are you sure you want to do nothing tonight?"; 

    string lockUpChosen = "Lock Up"; 
    string ejectChosen = "Eject"; 
    string doNothingChosen = "Do Nothing"; 

    string yesCharChosen = "Yes";
    string noCharChosen = "No"; 

    Character characterSelected; 

    void Awake()
    {
        if (characterButtons != null)
        {
            for (int i = 0; i < characterButtons.Length; i++)
            {
                characterButtons[i].onClick.RemoveAllListeners(); 
            }
        }

        if (actionButtons != null)
        {
            for (int i = 0; i < actionButtons.Length; i++)
            {
                actionButtons[i].onClick.RemoveAllListeners(); 
            }
        }

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners(); 

        // Navigation Officer:
        characterButtons[0].onClick.AddListener(NavOfficerClicked);

        // Cook:
        characterButtons[1].onClick.AddListener(CookClicked); 

        // Engineer:
        characterButtons[2].onClick.AddListener(EngineerClicked); 

        // Doctor/Scientist:
        characterButtons[3].onClick.AddListener(DoctorClicked); 

        // Rich Guy:
        characterButtons[4].onClick.AddListener(RichGuyClicked); 

        // Rich Girl:
        characterButtons[5].onClick.AddListener(RichGirlClicked); 

        /** Action Buttons **/
        actionButtons[0].onClick.AddListener(LockUpClicked); 
        actionButtons[1].onClick.AddListener(EjectClicked); 
        actionButtons[2].onClick.AddListener(GoBackClicked); 
        actionButtons[3].onClick.AddListener(DoNothingClicked); 
    }

    void Start()
    {
        accusationPanel.SetActive(false); // Panel not opened until end of the night. 
    }

    // Called by the GameManager's NightAccusation() function:
    // LockUpClicked() and EjectClicked() will send results of character and action selected to GameManager's MakeAccusation() function: 
    public void OpenAccusationPanel()
    {
        characterSelected = null; // Ensure selected character is always null when it is time to make an accusation. 

        // Ensure the yes and no buttons do not have any listeners yet when it is time to make an accusation. 
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners(); 

        accusationPanel.SetActive(true); 

        instructionDisplay.text = firstPanelInstruction; 

        characterSelectionPanel.SetActive(true); // Only open first child panel. 

        actionSelectionPanel.SetActive(false); // Do not open second child panel yet. 

        confirmationPanel.SetActive(false); // Do not open third child panel yet. 
    }

    //
    /** Button listeners for choosing the different characters to perform an accusation on: **/ 
    // 

    void NavOfficerClicked()
    {
        for (int i = 0; i < GameManager.Instance.characters.Length; i++)
        {
            if (GameManager.Instance.characters[i] is NavigationOfficer)
            {
                characterSelected = GameManager.Instance.characters[i]; 
                break; 
            }
        }

        if (characterSelected == null)
        {
            Debug.Log("Navigation Officer script not found"); 
            return; 
        }

        instructionDisplay.text = "What do you want to do to Leah?"; 

        previousPanelInstruction = instructionDisplay.text; 

        characterSelectionPanel.SetActive(false); // Close first child panel. 

        actionSelectionPanel.SetActive(true); // Open second child panel to confirm action. 

        confirmationPanel.SetActive(false); // Do not open third child panel yet. 
    }

    void CookClicked()
    {
        for (int i = 0; i < GameManager.Instance.characters.Length; i++)
        {
            if (GameManager.Instance.characters[i] is Cook)
            {
                characterSelected = GameManager.Instance.characters[i]; 
                break; 
            }
        }

        if (characterSelected == null)
        {
            Debug.Log("Cook script not found"); 
            return; 
        }

        instructionDisplay.text = "What do you want to do to Andy?"; 

        previousPanelInstruction = instructionDisplay.text;

        characterSelectionPanel.SetActive(false); // Close first child panel. 

        actionSelectionPanel.SetActive(true); // Open second child panel to confirm action.

        confirmationPanel.SetActive(false); // Do not open third child panel yet. 
    }

    void EngineerClicked()
    {
        for (int i = 0; i < GameManager.Instance.characters.Length; i++)
        {
            if (GameManager.Instance.characters[i] is Engineer)
            {
                characterSelected = GameManager.Instance.characters[i]; 
                break; 
            }
        }

        if (characterSelected == null)
        {
            Debug.Log("Engineer script not found"); 
            return; 
        }

        instructionDisplay.text = "What do you want to do to Ramona?"; 

        previousPanelInstruction = instructionDisplay.text;

        characterSelectionPanel.SetActive(false); // Close first child panel. 

        actionSelectionPanel.SetActive(true); // Open second child panel to confirm action.

        confirmationPanel.SetActive(false); // Do not open third child panel yet. 
    }

    void DoctorClicked()
    {
        for (int i = 0; i < GameManager.Instance.characters.Length; i++)
        {
            if (GameManager.Instance.characters[i] is Doctor)
            {
                characterSelected = GameManager.Instance.characters[i]; 
                break; 
            }
        }

        if (characterSelected == null)
        {
            Debug.Log("Doctor script not found"); 
            return; 
        }

        instructionDisplay.text = "What do you want to do to Walter Black?"; 

        previousPanelInstruction = instructionDisplay.text;

        characterSelectionPanel.SetActive(false); // Close first child panel. 

        actionSelectionPanel.SetActive(true); // Open second child panel to confirm action.

        confirmationPanel.SetActive(false); // Do not open third child panel yet. 
    }

    void RichGuyClicked()
    {
        for (int i = 0; i < GameManager.Instance.characters.Length; i++)
        {
            if (GameManager.Instance.characters[i] is RichGuy)
            {
                characterSelected = GameManager.Instance.characters[i]; 
                break; 
            }
        }

        if (characterSelected == null)
        {
            Debug.Log("Rich Guy script not found"); 
            return; 
        }

        instructionDisplay.text = "What do you want to do to Dick (Mr. Richard)?"; 

        previousPanelInstruction = instructionDisplay.text;

        characterSelectionPanel.SetActive(false); // Close first child panel. 

        actionSelectionPanel.SetActive(true); // Open second child panel to confirm action.

        confirmationPanel.SetActive(false); // Do not open third child panel yet. 
    }

    void RichGirlClicked()
    {
        for (int i = 0; i < GameManager.Instance.characters.Length; i++)
        {
            if (GameManager.Instance.characters[i] is RichGirl)
            {
                characterSelected = GameManager.Instance.characters[i]; 
                break; 
            }
        }

        if (characterSelected == null)
        {
            Debug.Log("Rich Girl script not found"); 
            return; 
        }

        instructionDisplay.text = "What do you want to do to Mrs. Moola?"; 

        previousPanelInstruction = instructionDisplay.text;

        characterSelectionPanel.SetActive(false); // Close first child panel. 

        actionSelectionPanel.SetActive(true); // Open second child panel to confirm action.

        confirmationPanel.SetActive(false); // Do not open third child panel yet. 
    }

    //
    /** Button listeners for choosing the specific action of what to do with the character: **/ 
    // 

    void LockUpClicked()
    {
        if (characterSelected == null)
        {
            Debug.Log("Character was not selected"); 
            return; 
        }

        instructionDisplay.text = "Are you sure you want to lock up the " + characterSelected.name + "?"; 

        characterSelectionPanel.SetActive(false); // Close first child panel. 

        actionSelectionPanel.SetActive(false); // Close second child panel.

        confirmationPanel.SetActive(true); // Open third child panel to confirm action. 

        /** Confirmation Buttons (Yes and No have unique listeners in different functions): **/
        yesButton.onClick.AddListener(() => YesClicked(lockUpChosen));  
        noButton.onClick.AddListener(() => NoClicked(yesCharChosen)); 
    }

    void EjectClicked()
    {
        if (characterSelected == null)
        {
            Debug.Log("Character was not selected"); 
            return; 
        }

        instructionDisplay.text = "Are you sure you want to throw the " + characterSelected.name + " overboard?"; 

        characterSelectionPanel.SetActive(false); // Close first child panel. 

        actionSelectionPanel.SetActive(false); // Close second child panel.

        confirmationPanel.SetActive(true); // Open third child panel to confirm action. 

        /** Confirmation Buttons (Yes and No have unique listeners in different functions): **/
        yesButton.onClick.AddListener(() => YesClicked(ejectChosen));  
        noButton.onClick.AddListener(() => NoClicked(yesCharChosen)); 
    }

    void GoBackClicked()
    {
        if (characterSelected != null) // Going back to first child panel (character selection) means to unchoose the selected character.
        {
            characterSelected = null; 
        }

        // Reset unique Yes and No listeners:
        yesButton.onClick.RemoveAllListeners(); 
        noButton.onClick.RemoveAllListeners(); 

        instructionDisplay.text = firstPanelInstruction;

        characterSelectionPanel.SetActive(true); // Go back to first child panel. 

        actionSelectionPanel.SetActive(false); // Close second child panel.

        confirmationPanel.SetActive(false); // Do not open third child panel yet. 
    }

    void DoNothingClicked()
    {
        instructionDisplay.text = confirmDoNothing; 

        characterSelectionPanel.SetActive(false); // Close first child panel. 

        actionSelectionPanel.SetActive(false); // Close second child panel.

        confirmationPanel.SetActive(true); // Open third child panel to confirm action. 

        /** Confirmation Buttons (Yes and No have unique listeners in different functions): **/
        yesButton.onClick.AddListener(() => YesClicked(doNothingChosen));  
        noButton.onClick.AddListener(() => NoClicked(noCharChosen)); 
    }

    void YesClicked(string actionType)
    {
        switch (actionType)
        {
            case "Lock Up":
                // Lock up the selected character (send accusation results over to GameManager's MakeAccusation() function): 
                GameManager.Instance.MakeAccusation(characterSelected, GameManager.AccusationType.LockUp); 
                break;
            case "Eject":
                // Eject the selected character (send accusation results over to GameManager's MakeAccusation() function): 
                GameManager.Instance.MakeAccusation(characterSelected, GameManager.AccusationType.ThrowOverboard); 
                break; 
            case "Do Nothing":
                // Do nothing for the night (send accusation results over to GameManager's MakeAccusation() function): 
                GameManager.Instance.MakeAccusation(characterSelected, GameManager.AccusationType.None); 
                break; 
            default:
                break; 
        }

        // Reset children panels to default active state, and close the parent panel:
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners(); 
        characterSelectionPanel.SetActive(true); 
        actionSelectionPanel.SetActive(true); 
        confirmationPanel.SetActive(true); 
        accusationPanel.SetActive(false); 
    }

    void NoClicked(string wasCharacterSelected)
    { 
        // Reset unique Yes listener:
        yesButton.onClick.RemoveAllListeners(); 
        noButton.onClick.RemoveAllListeners(); 

        switch (wasCharacterSelected)
        {
            case "Yes":
                instructionDisplay.text = previousPanelInstruction; 
                characterSelectionPanel.SetActive(false); // Do not open first child panel. 
                actionSelectionPanel.SetActive(true); // Re-open second child panel.
                confirmationPanel.SetActive(false); // Close third child panel.
                break;
            case "No":
                instructionDisplay.text = firstPanelInstruction; 
                characterSelectionPanel.SetActive(true); // Re-open first child panel. 
                actionSelectionPanel.SetActive(false); // Do not open second child panel.
                confirmationPanel.SetActive(false); // Close third child panel.
                break; 
            default:
                break; 
        } 
    }
}