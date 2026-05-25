using UnityEngine;

public class LassoLine : MonoBehaviour
{

    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;

    [SerializeField] float thickness = 0.02f;

    void LateUpdate()
    {
        Vector3 start = pointA.position;
        Vector3 end = pointB.position;

        // Midpoint
        transform.position = (start + end) / 2f;

        // Direction
        Vector3 dir = end - start;
        float distance = dir.magnitude;

        // Rotate Y-axis cylinder to point at target
        transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(90, 0, 0);

        // Scale: Y is length for cylinder
        transform.localScale = new Vector3(thickness, distance / 2f, thickness);
    }
}