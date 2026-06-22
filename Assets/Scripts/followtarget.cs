using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    public Rigidbody rb;

    public float speed = 15f;

    void LateUpdate()
    {
        Vector3 dir = target.position - rb.position;

        rb.linearVelocity = dir * speed;
    }
}