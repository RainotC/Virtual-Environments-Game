using UnityEngine;

public class SaddleBehaviour : MonoBehaviour
{
    public Transform mountPoint;
    public Transform ovrRig;

    public Behaviour locomotor;

    public bool isMounted = false;

    void Update()
    {
        if (isMounted) return;

        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            isMounted = true;
            Debug.LogWarning("Unmounted");
        }
    }

    public void OnGrab()
    {
        ovrRig.position = mountPoint.position;
        ovrRig.rotation = mountPoint.rotation;
        isMounted = true;
        locomotor.enabled = isMounted;
        Debug.LogWarning("Saddle mounted");

    }

    void LateUpdate()
    {
        if (isMounted)
        {
            ovrRig.position = mountPoint.position;
        }
    }
}
