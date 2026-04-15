using UnityEngine;

public class HorseController : MonoBehaviour
{
    public Transform halter;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float turnSpeed = 60f;
    [SerializeField] private float maxTurnAngle = 0.5f;
    [SerializeField] private bool isReinsGrabbed = false;
    [SerializeField] float currentSpeed = 0f;
    void Update()
    {
        if (!isReinsGrabbed)
            return;

        MoveForward();
        Turn();
    }

    void MoveForward()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    void Turn()
    {
        float input = halter.localPosition.z;
        input = Mathf.Clamp(input, -maxTurnAngle, maxTurnAngle);

        float turn = input * turnSpeed * Time.deltaTime;

        transform.Rotate(0f, turn, 0f);
    }

    public void OnGrab()
    {
        isReinsGrabbed = true;
    }

    public void OnRelease()
    {
        isReinsGrabbed = false;
    }
}
