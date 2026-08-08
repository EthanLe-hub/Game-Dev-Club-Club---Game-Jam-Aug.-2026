using UnityEngine;
using System.Collections;

public class DoorInteractable : MonoBehaviour, IInteractable
{

    public float openAngle = 90.0f;
    public float openSpeed = 2f;
    public bool locked = false;

    private bool _isOpened = false;
    private Quaternion _openRotation;
    private Quaternion _closedRotation;

    private Coroutine _coroutine;

    private void Start() {
        
        _closedRotation = transform.rotation;
        _openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));

    }

    public void Interact()
    {   
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(ToggleDoor());
    }

    private IEnumerator ToggleDoor()
    {
        Quaternion targetRotation = _isOpened ? _closedRotation : _openRotation;
        _isOpened = !_isOpened;

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);
            yield return null;
        }

        transform.rotation = targetRotation;

    }
}
