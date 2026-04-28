using UnityEngine;

public class InputManager : MonoBehaviour
{
    public OVRInput.Controller controller;

    public event System.Action OnTriggerPressed;
    public event System.Action OnTriggerReleased;
    public event System.Action OnTriggerHeld;
    void Update()
    {
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller) > 0.1f)
        {
            Debug.Log("Held");
            OnTriggerHeld?.Invoke();

        }
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
        {
            Debug.Log("trigger pressed!");
            OnTriggerPressed?.Invoke();
        }
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, controller))
        {
            Debug.Log("trigger released!");
            OnTriggerReleased?.Invoke();
        }
    }
}
