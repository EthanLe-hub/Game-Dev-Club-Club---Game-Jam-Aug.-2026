using UnityEngine;

/* Interactable class. Add any logic to a new script in the Interact method and
 player will run it when the interact button is pressed.

Naming case for children should be: [name]Interactable
*/
public interface IInteractable
{
    void Interact();
}
