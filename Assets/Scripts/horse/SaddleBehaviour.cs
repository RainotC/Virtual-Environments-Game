using UnityEngine;

public class SaddleBehaviour : MonoBehaviour
{
    public Transform mountPoint;
    public Transform ovrRig;

    public Behaviour locomotor;

    public bool isMounted = false;

    void Update()
    {
        if (isMounted)
        {
            ovrRig.position = mountPoint.position;
        }
        else //to delete
        {
            if (OVRInput.GetDown(OVRInput.Button.Start))
            {
                isMounted = true;
                Debug.LogWarning("Unmounted");
            }
        }
    }

    public void OnGrab() //to delete
    {
        ovrRig.position = mountPoint.position;
        ovrRig.rotation = mountPoint.rotation;
        isMounted = true;
        locomotor.enabled = isMounted;
        Debug.LogWarning("Saddle mounted");

    }

}
