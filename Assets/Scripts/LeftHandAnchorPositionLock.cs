using UnityEngine;

public class LeftHandAnchorPositionLock : MonoBehaviour
{
    public Transform hand;

    void LateUpdate()
    {
        transform.position = hand.position;
        transform.rotation = hand.rotation;
    }
}