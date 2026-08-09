// Ethan Le (8/6/2026):
using System.Collections; // For IEnumerator. 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

/** 
 * Global static script for managing game logic:
**/ 
public class GameManager : MonoBehaviour 
{
    public static GameManager Instance { get; private set; }

    // Variable for determining who is the imposter in this playthrough:
    int imposterIndex;

    // chances the imposter does something, for easily adjustable difficulty
    public float killSelfChance = 0.4f;
    public float killChance = 0.5f;
    public float imposterNightArrivalChance = 2.0f;
    public int maxDays = 5;

    // spawnpoints each day
    public GameObject nightSpawnPoint;
    public GameObject daySpawnPoint;
    public Player player;

    // all 6 characters (excluding captain) who can potentially be imposters or die:
    [SerializeField] Character[] characters;

    public int currentDay = 0;
    private Character currentImposter;
    public Character characterJustKilled; // Character who was just killed (needed to display the appropriate death cutscene image). 
    private int choicesLeft = 3;

    [SerializeField] CutsceneUI cutsceneUI; // Script to show death scene of new character each day.
    public bool isCutsceneActive = false; // True when a death scene is showing at the start of each day. 

    AccusationUI accusationUI; // Script to make accusation for the night. 
    string accusationUITag = "Accusation UI"; 
    int accusationScene = 1; 

    // whether tonight's door visitor is the imposter, rolled at NightStart and resolved by OpenDoor().
    private bool doorVisitorIsImposter;

    // states
    private enum GameState { GameStart, GameEnd, DayStart, DayEnd, NightStart, NightDoor, NightAccusation, NightEnd }
    public enum AccusationType { LockUp, ThrowOverboard, None }


    private GameState currentState;

    // Pause logic
    public bool paused = false;

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

    void Start()
    {
        // Deferred to Start() so every object's Awake() (e.g. Player assigning its
        // own CharacterController) has already run before we start touching them.
        StartGame();

        /** Testing the start of a brand new day after Day 0: **/
        /*
        currentImposter = characters[0];
        ImposterKillsSomeone();
        NightEnd();
        */
        //StartGame(); 

        /** Testing the start of a brand new day after Day 0: **/
        /*
        currentImposter = characters[0]; 
        ImposterKillsSomeone(); 
        NightEnd(); 
        */

        /** Testing the night accusation panel: **/ 
        
        // StartGame();
        // currentImposter = characters[0];  
        // TransitionTo(GameState.NightAccusation); 
        
    }

    void TransitionTo(GameState newState)
    {
        currentState = newState;
        Debug.Log("[GameManager] -> " + newState);
        switch (newState)
        {
            case GameState.GameStart:
                StartGame();
                break;
            case GameState.GameEnd:
                EndGame();
                break;
            case GameState.DayStart:
                DayStart();
                break;
            case GameState.DayEnd:
                DayEnd();
                break;
            case GameState.NightStart:
                NightStart();
                break;
            case GameState.NightDoor:
                NightDoor();
                break;
            case GameState.NightAccusation:
                NightAccusation();
                break;
            case GameState.NightEnd:
                NightEnd();
                break;
        }
    }

    //                              ========
    //                              ========

    //                       MAIN GAME LOOP FUNCTIONS
    
    //                              ========
    //                              ========

    // Pressing "Start" on the title screen should call this function to choose an imposter: 
    void StartGame()
    {

        // resetting daily params
        currentDay = 0;

        imposterIndex = Random.Range(0, characters.Length);
        Debug.Log("Imposter index is: " + imposterIndex);
        SetImposter(imposterIndex);
        TransitionTo(GameState.DayStart);
    }

    void EndGame()
    {
        bool win = currentImposter.isDead;
        Debug.Log("[GameManager] Game over. Win? " + win);
    }

    void DayStart() {

        if (currentDay == 0) choicesLeft = 10;
        else choicesLeft = 3;

        Debug.Log("Day Start");
        TeleportPlayer(daySpawnPoint.transform.position);

        if (currentDay == 0) // If it is intro day (Day 0), simply open up to gameplay from black screen (no deaths on Day 0):
        {
            cutsceneUI.StartOfGame(); 
        }

        else // If it is after the intro day (after Day 0), then show death cutscenes (no deaths on Day 0):
        {
            if (characterJustKilled != null)
            {
                Debug.Log(characterJustKilled.name + " death scene is showing");
                cutsceneUI.ShowDeathScene(characterJustKilled.deathSprite); // Send cutscene image to display in CutsceneUI.cs script.
            }
            else
            {
                Debug.Log("No deaths tonight!");
                cutsceneUI.CloseCutscene(); // No death cutscene to display.
            }
        }

        Debug.Log("[GameManager] Day " + currentDay + " start, choicesLeft = " + choicesLeft);

        PlaceAliveCharacters();

    }

    void DayEnd()
    {
        TransitionTo(GameState.NightStart);
    }

    void NightStart()
    {
        TeleportPlayer(nightSpawnPoint.transform.position);
        Debug.Log("[GameManager] imposterNightArrivalChance = " + imposterNightArrivalChance);
        
        doorVisitorIsImposter = Random.value < imposterNightArrivalChance;
        Debug.Log("[GameManager] Door visitor is imposter? " + doorVisitorIsImposter);
        TransitionTo(GameState.NightDoor);
    }

    void NightDoor()
    {
        // waits for OpenDoor() to be called by the door interaction.
    }

    // called when the player chooses to open (or not open) the door at night.
    public void OpenDoor(bool open)
    {
        if (currentState != GameState.NightDoor) return;

        Debug.Log("[GameManager] Player opened door? " + open);

        if (open)
        {
            if (doorVisitorIsImposter)
            {
                KillPlayer();
                return;
            }

            GiveClue();
        }

        ImposterKillsSomeone();
        // TransitionTo(GameState.NightAccusation);
        TransitionTo(GameState.NightEnd);
    }

    void NightAccusation()
    {
        // waits for MakeAccusation() to be called by the accusation UI.

        StartCoroutine(NightAccusationRoutine());
    }

    IEnumerator NightAccusationRoutine()
    {
        // Uses LoadSceneMode.Additive to keep the main scene alive while the accusation Scene is on (needed so the AccusationUI.cs can retrieve the same characters' scripts in memory):
        AsyncOperation asyncLoading = SceneManager.LoadSceneAsync(accusationScene, LoadSceneMode.Additive); 

        while (!asyncLoading.isDone)
        {
            yield return null; 
        }

        GameObject accusationUIObj = GameObject.FindGameObjectWithTag(accusationUITag); 

        if (accusationUIObj != null)
        {
            accusationUI = accusationUIObj.GetComponent<AccusationUI>(); 
        }

        if (accusationUI != null)
        {
            Debug.Log("ACCUSATION PANEL OPENING"); 
            accusationUI.OpenAccusationPanel(); // Begin accusation choice. 
        }
    }

    // called when the player locks someone up, throws someone overboard, or does nothing.
    public void MakeAccusation(Character target, AccusationType type)
    {
        if (currentState != GameState.NightAccusation) return;

        Debug.Log("[GameManager] Accusation: " + type + " on " + (target != null ? target.name : "nobody"));

        switch (type)
        {
            case AccusationType.LockUp:
                LockUp(target);
                break;
            case AccusationType.ThrowOverboard:
                ThrowOverboard(target);
                break;
            case AccusationType.None:
                break;
        }

        // Unload the accusation Scene before transitioning:
        SceneManager.UnloadSceneAsync(accusationScene); 

        TransitionTo(GameState.NightEnd);
    }

    void NightEnd()
    {   
        currentDay += 1;
        if (currentDay > maxDays) TransitionTo(GameState.GameEnd);
        else TransitionTo(GameState.DayStart);
    }

    //                              ========
    //                              ========

    //                          HELPER FUNCTIONS

    //                              ========
    //                              ========


    void KillPlayer()
    {
        Debug.Log("The imposter got you.");
        TransitionTo(GameState.GameEnd);
    }

    void GiveClue()
    {
        Debug.Log("A crewmate gives you a clue about the imposter.");
    }

    // imposter either kills its own host (and possesses a different alive character), kills someone else, or does not kill.
    void ImposterKillsSomeone()
    {
        if (currentImposter == null || 
            currentImposter.isDead || 
            currentImposter.isLockedUp) return;

        bool killsHost = Random.value < killSelfChance;
        bool killsCrewmate = Random.value < killChance; 

        if (killsHost) PossessNewHost();
        else if (killsCrewmate) KillRandomCrewmate();
        else // If imposter does not kill anyone that night, do nothing except setting "characterJustKilled" to null (so the same death scene does not play again the next day).
        {
            characterJustKilled = null; 
            return; 
        }
    }

    // imposter kills its current host body and jumps into a different alive character.
    void PossessNewHost()
    {
        currentImposter.isDead = true;
        currentImposter.isImposter = false;
        characterJustKilled = currentImposter; // Imposter killed its current host body, so that was the character who was just killed. 

        List<Character> candidates = GetAliveExcept(currentImposter);
        if (candidates.Count == 0)
        {
            TransitionTo(GameState.GameEnd);
            return;
        }

        Character newHost = candidates[Random.Range(0, candidates.Count)];
        newHost.isImposter = true;
        currentImposter = newHost;

        Debug.Log("The imposter killed its host and is now " + newHost.name);
    }

    void KillRandomCrewmate()
    {
        List<Character> candidates = GetAliveExcept(currentImposter);
        if (candidates.Count == 0) return;

        Character victim = candidates[Random.Range(0, candidates.Count)];
        victim.isDead = true;
        characterJustKilled = victim; // Random victim was killed, so assign it as the character who was just killed. 

        Debug.Log(victim.name + " was killed during the night.");
    }

    List<Character> GetAliveExcept(Character exclude)
    {
        List<Character> result = new List<Character>();
        foreach (Character character in characters)
        {
            if (!character.isDead && character != exclude)
                result.Add(character);
        }
        return result;
    }

    // setting characters to be imposter
    void SetImposter(int imposter)
    {
        characters[imposter].isImposter = true;
        currentImposter = characters[imposter];

        foreach (Character character in characters)
        {
            Debug.Log(character.name + " imposter? " + character.isImposter);
        }
    }

    // places alive characters in their positions throughout boat
    void PlaceAliveCharacters()
    {
        foreach (Character character in characters)
        {
            if (character.isDead) continue;
            if (character.isLockedUp) continue;

            character.PlaceAtRandomSpawn();
        }
    }

    public void SpendChoice()
    {
        if (currentState != GameState.DayStart) return;
        if (choicesLeft <= 0) return;

        choicesLeft--;

        if (choicesLeft <= 0)
            TransitionTo(GameState.DayEnd);
    }

    public void EndDayEarly()
    {
        TransitionTo(GameState.DayEnd);
    }

    void LockUp(Character character)
    {
        character.isLockedUp = true;
    }

    void ThrowOverboard(Character character)
    {
        character.isDead = true;
    }

    // Helper function to close death cutscene once player presses Spacebar key (called in Player.cs):
    public void HelperCloseCutscene()
    {
        cutsceneUI.CloseCutscene(); 
    }


    void TeleportPlayer(Vector3 pos)
    {
        player.playerController.enabled = false;
        player.transform.position = pos;
        player.playerController.enabled = true;
    }

    public void pause()
    {
        paused = true;
        Time.timeScale = 0f;

        foreach (MonoBehaviour script in player.GetComponents<MonoBehaviour>())
        {
            script.enabled = false;
        }
    }

    public void unpause()
    {
        paused = false;
        Time.timeScale = 1f;

        foreach (MonoBehaviour script in player.GetComponents<MonoBehaviour>())
        {
            script.enabled = true;
        }
    }
}
