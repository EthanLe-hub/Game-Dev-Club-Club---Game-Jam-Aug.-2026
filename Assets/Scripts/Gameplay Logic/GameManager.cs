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
    public float clueAccuracy = 0.7f; // chance that a clue line names the real imposter (otherwise a random other living character)
    public float finalHintAccuracy = 0.9f; // accuracy of the hint given by an innocent night visitor (reward for risking the door)
    public int maxDays = 5;

    // spawnpoints each day
    public GameObject nightSpawnPoint;
    public GameObject daySpawnPoint;
    public Player player;

    // all 6 characters (excluding captain) who can potentially be imposters or die:
    [SerializeField] public Character[] characters;

    public int currentDay = 0;
    private Character currentImposter;
    private Character lockedUpCharacter; 

    // Class name of the character the imposter is currently possessing (e.g. "Engineer"), or null if none.
    // Dialogue clue dictionaries are keyed by this name.
    public string CurrentImposterName => currentImposter != null ? currentImposter.GetType().Name : null;

    // False once the imposter can no longer act (dead or locked up) - dialogue should use fallback lines then.
    public bool ImposterIsActive => currentImposter != null && !currentImposter.isDead && !currentImposter.isLockedUp;

    
    public Character characterJustKilled; // Character who was just killed (needed to display the appropriate death cutscene image). 
    private int choicesLeft = 3;

    [SerializeField] CutsceneUI cutsceneUI; // Script to show death scene of new character each day.
    public bool isCutsceneActive = false; // True when a death scene is showing at the start of each day. 

    AccusationUI accusationUI; // Script to make accusation for the night.
    string accusationUITag = "Accusation UI";
    string accusationScene = "End of Day Judgement"; // Loaded by NAME, so reordering Build Settings (e.g. adding the title scene) cannot break it.
    string gameOverScene = "GameOver"; // Scene loaded (replacing Main) when the game ends.
    public bool isAccusing = false; // True when making an accusation for the night.


    [SerializeField] NightSequence nightSequence; // Night door UI: knock narration, open/keep-closed choice, final hint.
    public bool isAtDoor = false; // True while the open/keep-closed choice is on screen (frees the cursor in Player.cs).
    bool doorResolved; // True once tonight's door choice has been made (prevents answering the door twice).

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

        // Set this first instance to be the static instance.
        // NOTE: deliberately NOT DontDestroyOnLoad - the judgement scene loads additively (this scene
        // never unloads mid-game), and a persistent manager would carry dead references to destroyed
        // scene objects when returning to the title screen or restarting.
        Instance = this;
    }

    // Clear the singleton when this scene unloads (e.g. going back to the title screen),
    // so the next Main-scene load can register its fresh GameManager:
    void OnDestroy()
    {
        if (Instance == this) Instance = null;
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
        // Win = the imposter is dead (thrown overboard or killed). Everything else - the player
        // opening the door to the imposter, or the days running out - counts as a loss:
        bool win = currentImposter != null && currentImposter.isDead;
        Debug.Log("[GameManager] Game over. Win? " + win);

        GameOverUI.PlayerWon = win; // Static handoff - this GameManager is destroyed when the scene switches.
        SceneManager.LoadScene(gameOverScene);
    }

    void DayStart() {

        if (currentDay == 0) choicesLeft = 10;
        else choicesLeft = 3;

        Debug.Log("Day Start");

        if (SoundManager.Instance != null) SoundManager.Instance.PlayDayMusic();

        if (currentDay == 0) // If it is intro day (Day 0), simply open up to gameplay from black screen (no deaths on Day 0):
        {
            cutsceneUI.StartOfGame(() => TeleportPlayer(daySpawnPoint.transform.position));
        }

        else // If it is after the intro day (after Day 0), then show death cutscenes (no deaths on Day 0):
        {
            if (characterJustKilled != null)
            {
                Debug.Log(characterJustKilled.name + " death scene is showing");
                cutsceneUI.ShowDeathScene(characterJustKilled.deathSprite, () => TeleportPlayer(daySpawnPoint.transform.position));
            }
            else
            {
                Debug.Log("No deaths tonight!");
                cutsceneUI.CloseCutscene(() => TeleportPlayer(daySpawnPoint.transform.position));
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
        Debug.Log("[GameManager] imposterNightArrivalChance = " + imposterNightArrivalChance);

        doorVisitorIsImposter = Random.value < imposterNightArrivalChance;
        doorResolved = false; // New night, new door choice.

        if (SoundManager.Instance != null) SoundManager.Instance.PlayNightMusic();
        Debug.Log("[GameManager] Door visitor is imposter? " + doorVisitorIsImposter);

        // Fade to black, teleport to the bedroom, fade back in - then the visitor knocks:
        cutsceneUI.GoToNight(
            () => TeleportPlayer(nightSpawnPoint.transform.position),
            () => { if (nightSequence != null) nightSequence.BeginKnock(doorVisitorIsImposter); });

        TransitionTo(GameState.NightDoor);
    }

    void NightDoor()
    {
        // waits for OpenDoor() to be called by the door interaction.
    }

    // called by NightDoorInteractable when the player clicks the door - opens the open/keep-closed choice.
    public void DoorClicked()
    {
        if (currentState != GameState.NightDoor || doorResolved) return;

        // Ignore door interaction while narration is on screen - the Space press that advances
        // the text is also the interact key, and must not open the door choice mid-reading:
        if (nightSequence != null && nightSequence.IsShowingLines) return;

        if (nightSequence == null)
        {
            // Night UI not wired up in this scene - resolve as "kept closed" so the night can still finish:
            Debug.LogError("[GameManager] nightSequence is not assigned in the Inspector! Treating the door as kept closed.");
            OpenDoor(false);
            return;
        }

        nightSequence.ShowDoorChoice();
        isAtDoor = true; // Only after the UI actually opened, so a failure above can never leave this stuck on.
    }

    // called when the player chooses to open (or not open) the door at night.
    public void OpenDoor(bool open)
    {
        if (currentState != GameState.NightDoor || doorResolved) return;

        doorResolved = true;
        isAtDoor = false;
        Debug.Log("[GameManager] Player opened door? " + open);

        if (!open)
        {
            cutsceneUI.GoToNight(FinishNight);
            return;
        }

        if (doorVisitorIsImposter)
        {
            cutsceneUI.ShowPlayerDeath("You open the door...\n\nIt was the imposter.\n\nYou died.",
                () => TransitionTo(GameState.GameEnd));
            return;
        }

        // Opened the door to an innocent visitor: they give one final, more accurate hint.
        // Then back to bed and on to judgement:
        Character hintGiver = GetRandomAliveInnocent();
        if (hintGiver != null && nightSequence != null)
        {
            string hint = BuildClueLine(hintGiver, finalHintAccuracy);
            nightSequence.ShowFinalHint(hintGiver.DisplayName, hint, () => cutsceneUI.GoToNight(FinishNight));
            return;
        }

        cutsceneUI.GoToNight(FinishNight);
    }

    // Shared tail of the night: the imposter acts, then the accusation begins (runs while the screen is black).
    void FinishNight()
    {
        ImposterKillsSomeone();
        TransitionTo(GameState.NightAccusation);
    }

    void NightAccusation()
    {
        StartCoroutine(NightAccusationRoutine());
    }

    IEnumerator NightAccusationRoutine()
    {
        isAccusing = true; 

        if (lockedUpCharacter != null)
        {
            lockedUpCharacter.gameObject.SetActive(true); 
            lockedUpCharacter.isLockedUp = false;
            lockedUpCharacter = null; 
        }

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
        isAccusing = false; 

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


    // imposter either kills its own host (and possesses a different alive character), kills someone else, or does not kill.
    void ImposterKillsSomeone()
    {
        if (currentImposter == null ||
            currentImposter.isDead ||
            currentImposter.isLockedUp)
        {
            characterJustKilled = null; // Imposter can't act tonight, so no new death - do not let a stale value replay yesterday's cutscene.
            return;
        }

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
        victim.gameObject.SetActive(false); // De-activate victim. 
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

    // Picks who a clue line should name (fills the {name} placeholder in talkClueTemplates):
    // 'accuracy' chance of the real imposter; otherwise a random other living character.
    // (Daytime clues pass clueAccuracy, the night-door final hint passes finalHintAccuracy.)
    // The speaker is excluded so characters never implicate themselves.
    public Character GetClueSuspect(Character speaker, float accuracy)
    {
        if (ImposterIsActive && Random.value < accuracy) return currentImposter;

        List<Character> candidates = new List<Character>();
        foreach (Character character in characters)
        {
            if (character.isDead || character == currentImposter || character == speaker) continue;
            candidates.Add(character);
        }

        if (candidates.Count == 0) return currentImposter; // Nobody else left to falsely accuse.
        return candidates[Random.Range(0, candidates.Count)];
    }

    // Builds a filled-in clue line spoken by 'speaker', naming a suspect chosen with the given accuracy.
    public string BuildClueLine(Character speaker, float accuracy)
    {
        Character suspect = GetClueSuspect(speaker, accuracy);

        // Templates mentioning {deadCharacter} only make sense if someone died last night:
        List<string> usable = new List<string>();
        string[] templates = speaker.dialogue != null ? speaker.dialogue.talkClueTemplates : null;
        if (templates != null)
        {
            foreach (string template in templates)
            {
                if (characterJustKilled == null && template.Contains("{deadCharacter}")) continue;
                usable.Add(template);
            }
        }

        // No usable templates (e.g. dialogue asset not written yet) - fall back to a generic clue:
        if (usable.Count == 0) return "I saw " + suspect.DisplayName + " acting strange earlier... watch them, Captain.";

        string line = usable[Random.Range(0, usable.Count)];
        line = line.Replace("{name}", suspect.DisplayName);
        if (characterJustKilled != null) line = line.Replace("{deadCharacter}", characterJustKilled.DisplayName);

        return line;
    }

    // A random living, free character who is not the imposter (the night-door hint giver). Null if none left.
    Character GetRandomAliveInnocent()
    {
        List<Character> candidates = new List<Character>();
        foreach (Character character in characters)
        {
            if (character.isDead || character.isLockedUp || character == currentImposter) continue;
            candidates.Add(character);
        }

        if (candidates.Count == 0) return null;
        return candidates[Random.Range(0, candidates.Count)];
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
        lockedUpCharacter = character; 
        lockedUpCharacter.gameObject.SetActive(false); 
    }

    void ThrowOverboard(Character character)
    {
        character.isDead = true;
        character.gameObject.SetActive(false); 
    }

    // Helper function to close death cutscene once player presses Spacebar key (called in Player.cs):
    public void HelperCloseCutscene()
    {
        cutsceneUI.CloseCutscene(null);
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
