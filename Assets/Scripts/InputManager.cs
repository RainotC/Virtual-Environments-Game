using UnityEngine;

public class InputManager : MonoBehaviour
{
    public OVRInput.Controller controller;

    public event System.Action OnIndexTriggerPressed;
    public event System.Action OnIndexTriggerReleased;
    public event System.Action OnIndexTriggerHeld;
    public event System.Action OnHandTriggerPressed;

    void Update()
    {
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller) > 0.1f)
        {
            Debug.Log("Held");
            OnIndexTriggerHeld?.Invoke();

        }
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
        {
            Debug.Log("trigger pressed!");
            OnIndexTriggerPressed?.Invoke();
        }
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, controller))
        {
            Debug.Log("trigger released!");
            OnIndexTriggerReleased?.Invoke();
        }
        if(OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, controller))
        {
            Debug.Log("Hand trigger pressed!");
            OnHandTriggerPressed?.Invoke();
        }
    }
}
