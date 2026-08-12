// Ethan Le (8/8/2026):
using System; // For Action.
using System.Collections; // For IEnumerator.
using UnityEngine;
using UnityEngine.InputSystem; // For Keyboard (player-death screen dismissal).
using UnityEngine.UI;
using TMPro; // For the player-death message text.

/** 
 * Script to handle death scene that plays at the start of each new day.
**/ 
public class CutsceneUI : MonoBehaviour
{
    // Assign these in Unity Inspector: 
    [SerializeField] GameObject panelUI;
    [SerializeField] Image cutsceneImage;
    [SerializeField] CanvasGroup fadeCanvasGroup;
    [SerializeField] TextMeshProUGUI deathText; // "You died" message shown over the black screen (place it above the Black Screen in the canvas hierarchy).
    [SerializeField] GameObject dialoguePanel; // Holds the "deathText" component as its grandchild. 

    float fadeDuration = 1f; // 1 second for the fade. 
    float inBetweenWait = 1f;
    
    bool isTransitioning = false; // True when death image is loading in. 
    float timePassed; // Keeps track of how long it has been in the transition for (gets compared against fadeDuration to know when fade is done). 

    // Awake (not Start) so this init is guaranteed to run before GameManager.Start() kicks off the first transition:
    void Awake()
    {
        panelUI.SetActive(false);
        cutsceneImage.enabled = false;
    }

    public void ShowDeathScene(Sprite cutsceneSprite, Action onTeleport)
    {
        if (isTransitioning) // Prevent transition from triggering twice at the same time.
        {
            return;
        }

        cutsceneImage.sprite = cutsceneSprite;
        cutsceneImage.enabled = true; // Make the death sprite visible for this transition.

        StartCoroutine(StartTransitionToDeathScene(onTeleport));
    }

    public void CloseCutscene(Action onTeleport)
    {
        if (isTransitioning) // Prevent transition from triggering twice at the same time.
        {
            return;
        }

        StartCoroutine(StartTransitionToNewDay(onTeleport));
    }

    // Called on Day 0 (to open the game from black screen):
    public void StartOfGame(Action onTeleport)
    {
        if (isTransitioning) // Prevent transition from triggering twice at the same time.
        {
            return;
        }

        StartCoroutine(StartTransitionAtGameStart(onTeleport));
    }

    IEnumerator StartTransitionToDeathScene(Action onTeleport)
    {
        isTransitioning = true; 
        GameManager.Instance.isCutsceneActive = true; 

        fadeCanvasGroup.blocksRaycasts = true;
        panelUI.SetActive(true);
        cutsceneImage.enabled = true;

        yield return StartCoroutine(Fading(1));

        onTeleport?.Invoke();

        isTransitioning = false;
    }

    IEnumerator StartTransitionToNewDay(Action onTeleport)
    {
        isTransitioning = true; // Mark as true so we do not accidentally have multiple transitions playing at once.
        panelUI.SetActive(true);

        // (1) Do not allow any UI interactions during transition:
        fadeCanvasGroup.blocksRaycasts = true;

        // (2) Fade to black (or stay black if we are dismissing a death scene that is already showing):
        yield return StartCoroutine(Fading(1));

        // (3) Screen is fully covered, so it is safe to teleport the player, then hold before revealing:
        onTeleport?.Invoke();
        yield return new WaitForSeconds(inBetweenWait);

        // (4) Slowly fade out into the new day:
        yield return StartCoroutine(Fading(0));

        // (5) Now that nothing is visible, retire the death sprite so it cannot bleed into later transitions:
        cutsceneImage.enabled = false;
        panelUI.SetActive(false);
        fadeCanvasGroup.blocksRaycasts = false;
        isTransitioning = false; // Transition is now done.
        GameManager.Instance.isCutsceneActive = false; // Death cutscene is over, so turn flag off to re-enable player movement again.
    }

    IEnumerator StartTransitionAtGameStart(Action onTeleport)
    {
        isTransitioning = true;
        fadeCanvasGroup.blocksRaycasts = true;

        // Cover the screen in black immediately (no fade-in at game start), teleport while hidden, then hold:
        panelUI.SetActive(true);
        fadeCanvasGroup.alpha = 1;
        onTeleport?.Invoke();
        yield return new WaitForSeconds(inBetweenWait);

        // Fade out of black into the first day:
        yield return StartCoroutine(Fading(0));

        panelUI.SetActive(false);
        fadeCanvasGroup.blocksRaycasts = false;
        isTransitioning = false;
    }

    // Called at the start of NightStart() (fades to black, runs onBlackScreen while hidden, then fades back in).
    // onComplete fires after the fade back in finishes (e.g. to start the door knock).
    public void GoToNight(Action onBlackScreen, Action onComplete = null)
    {
        if (isTransitioning) return;
        StartCoroutine(StartTransitionToNight(onBlackScreen, onComplete));
    }

    IEnumerator StartTransitionToNight(Action onBlackScreen, Action onComplete)
    {
        isTransitioning = true;
        fadeCanvasGroup.blocksRaycasts = true;

        cutsceneImage.enabled = false;
        panelUI.SetActive(true);
        yield return StartCoroutine(Fading(1));

        // signal for gamemanager to follow
        onBlackScreen?.Invoke();

        yield return new WaitForSeconds(inBetweenWait);
        yield return StartCoroutine(Fading(0));

        panelUI.SetActive(false);
        fadeCanvasGroup.blocksRaycasts = false;
        isTransitioning = false;

        onComplete?.Invoke();
    }

    // Shows the "you died" screen when the player opens the door to the imposter:
    // fades to black, shows the message, then waits for Space before handing control back (via onDismissed).
    public void ShowPlayerDeath(string message, Action onDismissed)
    {
        if (isTransitioning) return;
        StartCoroutine(PlayerDeathRoutine(message, onDismissed));
    }

    IEnumerator PlayerDeathRoutine(string message, Action onDismissed)
    {
        isTransitioning = true;
        GameManager.Instance.isCutsceneActive = true; // Freeze player movement.
        fadeCanvasGroup.blocksRaycasts = true;

        cutsceneImage.enabled = false; // Pure black screen with text only.
        panelUI.SetActive(true);

        if (deathText != null)
        {
            deathText.text = message;
            deathText.gameObject.SetActive(true); // Fades in together with the black screen.
            dialoguePanel.gameObject.SetActive(true); // Allows the "deathText" to show up onscreen. 
        }

        yield return StartCoroutine(Fading(1));

        // Wait for Space. isTransitioning stays true the whole time, which also blocks
        // Player.cs's cutscene-dismiss path (HelperCloseCutscene -> CloseCutscene) from
        // starting a new-day transition on the same key press.
        yield return null; // Do not count a press from this same frame.
        while (!Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            yield return null;
        }

        // The game is over - deliberately leave the screen black (and isTransitioning true,
        // so no other transition can play over the game-over state):
        onDismissed?.Invoke();
    }

    // Function that fades the screen to endingAlpha, starting from whatever alpha is currently showing
    // (fading from the current value instead of a hardcoded start prevents a one-frame flash when the
    // screen is already faded, e.g. dismissing the death scene while the canvas is already at alpha 1):
    IEnumerator Fading(float endingAlpha)
    {
        float startingAlpha = fadeCanvasGroup.alpha;

        if (Mathf.Approximately(startingAlpha, endingAlpha)) yield break; // Already at the target, no need to wait out the timer.

        timePassed = 0f; // Begin timer.

        while (timePassed < fadeDuration) // Continue fade-in or fade-out until we reach the desired fadeDuration:
        {
            timePassed += Time.deltaTime; // Increment the time that has passed.

            // Shift the alpha smoothly (from startingAlpha to endingAlpha):
            fadeCanvasGroup.alpha = Mathf.Lerp(startingAlpha, endingAlpha, timePassed / fadeDuration);

            yield return null; // Keeps the loop going (do not exit out of this function yet).
        }

        fadeCanvasGroup.alpha = endingAlpha; // Once the desired fadeDuration is achieved, have the black screen be completely uncolored.
    }
}