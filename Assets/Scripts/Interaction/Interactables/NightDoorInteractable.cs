using UnityEngine;
using UnityEngine.InputSystem;

public class NightDoorInteractable : MonoBehaviour, IInteractable
{
    private bool waitingForResponse = false;

    public void Interact()
    {
        waitingForResponse = true;
    }

    void Update()
    {
        if (!waitingForResponse) return;

        if (Keyboard.current.yKey.wasPressedThisFrame)
        {
            waitingForResponse = false;
            GameManager.Instance.OpenDoor(true);
        }
        else if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            waitingForResponse = false;
            GameManager.Instance.OpenDoor(false);
        }
    }
}
