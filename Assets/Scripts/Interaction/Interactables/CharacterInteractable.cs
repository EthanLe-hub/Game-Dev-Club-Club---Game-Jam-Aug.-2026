using UnityEngine;

public class CharacterInteractable : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        UIManager.Instance.startDialogue(GetComponentInParent<Character>());
    }
}