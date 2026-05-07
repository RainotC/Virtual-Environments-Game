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
            OnIndexTriggerHeld?.Invoke();

        }
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
        {
            OnIndexTriggerPressed?.Invoke();
        }
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, controller))
        {
            OnIndexTriggerReleased?.Invoke();
        }
        if(OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, controller))
        {
            OnHandTriggerPressed?.Invoke();
        }
    }
}
