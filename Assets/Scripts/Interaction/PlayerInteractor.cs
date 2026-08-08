using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float interactionDistance;
    [SerializeField] private LayerMask interactionMask = ~0;

    [SerializeField] private InputActionReference interactionRef;

    private void OnEnable()
    {
        interactionRef.action.Enable();
        interactionRef.action.started += PlayerInteracted;
    }

    private void OnDisable()
    {
        interactionRef.action.Disable();
        interactionRef.action.started -= PlayerInteracted;
    }

    private void PlayerInteracted(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            if (!Physics.Raycast(ray, out RaycastHit hitInfo, interactionDistance, interactionMask, QueryTriggerInteraction.Ignore)) return;

            Debug.Log($"Hit: {hitInfo.collider?.name}");
            
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactableObject))
            {
               interactableObject.Interact(); 
            }
                
        }
    }
}
