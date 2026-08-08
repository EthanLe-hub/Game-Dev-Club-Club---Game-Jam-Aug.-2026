// Ethan Le (8/8/2026):
using System.Collections; // For IEnumerator. 
using UnityEngine; 
using UnityEngine.UI; 

/** 
 * Script to handle death scene that plays at the start of each new day.
**/ 
public class CutsceneUI : MonoBehaviour
{
    // Assign these in Unity Inspector: 
    [SerializeField] GameObject panelUI; 
    [SerializeField] Image cutsceneImage; 
    [SerializeField] CanvasGroup fadeCanvasGroup; 

    float fadeDuration = 1f; // 1 second for the fade. 
    
    bool isTransitioning = false; // True when death image is loading in. 
    float timePassed; // Keeps track of how long it has been in the transition for (gets compared against fadeDuration to know when fade is done). 

    void Start()
    {
        panelUI.SetActive(false); 
    }

    public void ShowDeathScene(Sprite cutsceneSprite)
    {
        if (isTransitioning) // Prevent transition from triggering twice at the same time. 
        {
            return; 
        }

        cutsceneImage.sprite = cutsceneSprite;

        // startingAlpha = 1 (completely colored), endingAlpha = 0 (completely uncolored / invisible). 
        StartCoroutine(StartTransitionToDeathScene()); 
    }

    public void CloseCutscene()
    {
        if (isTransitioning) // Prevent transition from triggering twice at the same time. 
        {
            return; 
        }

        // startingAlpha = 1 (completely colored), endingAlpha = 0 (completely uncolored / invisible). 
        StartCoroutine(StartTransitionToNewDay());
    }

    IEnumerator StartTransitionToDeathScene()
    {
        isTransitioning = true; // Mark as true so we do not accidentally have multiple transitions playing at once. 
        GameManager.Instance.isCutsceneActive = true; // Death cutscene is playing, so turn flag on to disable player movement.

        // (1) Do not allow any UI interactions during transition: 
        fadeCanvasGroup.blocksRaycasts = true; 

        // (2) Fade to black (0 = invisible, 1 = completely colored):
        yield return StartCoroutine(Fading(0, 1)); 

        // (3) Enable the cutscene panel while screen is black:
        panelUI.SetActive(true); 

        // (4) Slowly fade out into the death scene panel: 
        yield return StartCoroutine(Fading(1, 0)); 

        // (5) Allow UI interactions since transition is now complete: 
        fadeCanvasGroup.blocksRaycasts = false; 
        isTransitioning = false; // Transition is now done. 
    }

    IEnumerator StartTransitionToNewDay()
    {
        isTransitioning = true; // Mark as true so we do not accidentally have multiple transitions playing at once. 

        // (1) Do not allow any UI interactions during transition: 
        fadeCanvasGroup.blocksRaycasts = true; 

        // (2) Fade to black (0 = invisible, 1 = completely colored):
        yield return StartCoroutine(Fading(0, 1)); 

        // (3) Enable the cutscene panel while screen is black:
        panelUI.SetActive(false); 

        // (4) Slowly fade out into the death scene panel: 
        yield return StartCoroutine(Fading(1, 0)); 

        // (5) Allow UI interactions since transition is now complete: 
        fadeCanvasGroup.blocksRaycasts = false; 
        isTransitioning = false; // Transition is now done. 
        GameManager.Instance.isCutsceneActive = false; // Death cutscene is over, so turn flag off to re-enable player movement again. 
    }

    // Function that fades the screen in and out:
    IEnumerator Fading(float startingAlpha, float endingAlpha)
    {
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