using UnityEngine;

public class CaptainsRoomInteractable : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        GameManager.Instance.EndDayEarly();
    }
}
