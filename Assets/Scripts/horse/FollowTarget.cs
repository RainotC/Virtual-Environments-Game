using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target; // The target I mean positionlock graable
    public Rigidbody rb;

    public float speed = 15f;

    void FixedUpdate()
    {
        Vector3 dir = target.position - rb.position;

        rb.linearVelocity = dir * speed;
    }
}