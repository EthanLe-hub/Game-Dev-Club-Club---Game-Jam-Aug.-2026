// Ethan Le (8/6/2026):
using UnityEngine; 
using UnityEngine.Events; 

/**
 * Superclass script for all 6 imposter-potential character scripts in the game.
**/
public class Character : MonoBehaviour 
{
    public bool isImposter = false;

    //[SerializeField] string[] regularTexts; // Fill in via Unity Inspector with character's appropriate dialogue (when not imposter). 
    //[SerializeField] string[] imposterTexts; // Fill in via Unity Inspector with character's dialogue as imposter. 
    public DialogueParent dialogue;

    public Sprite deathSprite; // Fill in via Unity Inspector with the cutscene image to show when this character dies.

    // Unity Events for other scripts to subscribe to: 
    // Subclass scripts will subscribe to these events and add listeners to them in their own scripts:
    [HideInInspector] public UnityEvent OnThrowingOut = new UnityEvent(); 
    [HideInInspector] public UnityEvent OnLockingUp = new UnityEvent();

    public bool isDead; 
    public bool isLockedUp;

    // Function called when player selects character to throw out of the submarine:
    public void ThrowOut()
    {
        OnThrowingOut?.Invoke(); // Trigger the event, which the subclass listens for.
    }

    // Function called when player selects character to lock up in quarantine:
    public void LockUp()
    {
        OnLockingUp?.Invoke(); // Trigger the event, which the subclass listens for. 
    }
}