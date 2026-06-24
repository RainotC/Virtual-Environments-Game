using UnityEngine;

public class AttachReinsToLeftHand : MonoBehaviour
{
    public Transform leftHandAnchor;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;

    private void LateUpdate()
    {
        if (leftHandAnchor == null) return;

        transform.position = leftHandAnchor.position + leftHandAnchor.TransformDirection(positionOffset);
        transform.rotation = leftHandAnchor.rotation * Quaternion.Euler(rotationOffset);
    }
}