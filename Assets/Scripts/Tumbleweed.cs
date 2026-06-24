using UnityEngine;

public class Tumbleweed : MonoBehaviour
{
    public Vector3 direction = Vector3.forward;
    public float speed = 2f;
    public float rotationSpeed = 180f;

    private float changeDirectionTimer = 0f;
    private float changeInterval = 10f;

    private Vector3 currentDirection;

    void Start()
    {
        currentDirection = direction.normalized;
    }

    void Update()
    {
        changeDirectionTimer += Time.deltaTime;

        if (changeDirectionTimer >= changeInterval)
        {
            changeDirectionTimer = 0f;

            currentDirection = new Vector3(
                Random.Range(-1f, 1f),
                0f,
                Random.Range(-1f, 1f)
            ).normalized;

            if (currentDirection == Vector3.zero)
                currentDirection = Vector3.forward;
        }

        transform.position += currentDirection * speed * Time.deltaTime;

        if (currentDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime / 360f
            );
        }

        transform.Rotate(Vector3.right * 360f * Time.deltaTime);
    }
}