using UnityEngine;
using UnityEngine.UIElements;
public class Tumbleweed : MonoBehaviour
{
    public Vector3 direction = Vector3.forward;
    public float speed = 2f;
    public float rotationSpeed = 180f;

    void Update()
    {
        transform.position += direction.normalized * speed * Time.deltaTime;
        transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
    }
}