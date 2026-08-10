using UnityEngine;

public class NightDoorInteractable : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        // Opens the open/keep-closed choice UI (GameManager ignores this outside the
        // NightDoor state or once tonight's door choice has already been made):
        GameManager.Instance.DoorClicked();
    }
}
