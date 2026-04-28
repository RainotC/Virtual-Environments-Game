using UnityEngine;

public class HorseController : MonoBehaviour
{
    public Transform halter;

    public Transform playerRig;
    private float lastHorseYaw;

    [Header("Speed")]
    public float minSpeed = 0f;
    public float maxSpeed = 8f;
    public float acceleration = 2f;

    [Header("Turning")]
    public float maxTurnRate = 50f;
    public float turnSmoothness = 3f;

    [Header("Input Limits")]
    public float maxForwardInput = 0.5f;
    public float maxSideInput = 0.5f;
    public float deadZone = 0.05f;

    private float currentSpeed;
    private float currentTurnRate;

    public bool isReinsGrabbed = false;

    public void OnGrab()

    {
        isReinsGrabbed = true;
    }

    public void OnRelease()
    {
        isReinsGrabbed = false;
    }

    void Update()
    {
        if (!isReinsGrabbed)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, acceleration * Time.deltaTime);
            currentTurnRate = Mathf.Lerp(currentTurnRate, 0f, turnSmoothness * Time.deltaTime);
        }
        else
        {
            HandleSpeed();
            HandleTurning();
        }

        MoveHorse();
    }

    void Start()
    {
        lastHorseYaw = transform.eulerAngles.y;
    }

    void LateUpdate()
    {
        RotatePlayerWithHorse();
    }

    void RotatePlayerWithHorse()
    {
        float currentYaw = transform.eulerAngles.y;

        float deltaYaw = Mathf.DeltaAngle(lastHorseYaw, currentYaw);

        playerRig.Rotate(0f, deltaYaw, 0f);

        lastHorseYaw = currentYaw;
    }

    void HandleSpeed()
    {
        float zInput = Mathf.Clamp(halter.localPosition.z, -maxForwardInput, maxForwardInput);

        if (Mathf.Abs(zInput) < deadZone)
            zInput = 0f;

        float normalized = Mathf.InverseLerp(-maxForwardInput, maxForwardInput, zInput);
        float targetSpeed = Mathf.Lerp(minSpeed, maxSpeed, normalized);

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
    }

    void HandleTurning()
    {
        float xInput = Mathf.Clamp(halter.localPosition.x, -maxSideInput, maxSideInput);

        if (Mathf.Abs(xInput) < deadZone)
            xInput = 0f;

        float targetTurnRate = (xInput / maxSideInput) * maxTurnRate;

        currentTurnRate = Mathf.Lerp(currentTurnRate, targetTurnRate, turnSmoothness * Time.deltaTime);
    }

    void MoveHorse()
    {
        transform.position += transform.forward * currentSpeed * Time.deltaTime;
        transform.Rotate(0f, currentTurnRate * Time.deltaTime, 0f);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);

        Vector3 velocity = transform.forward * currentSpeed;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + velocity);

        Quaternion predictedRotation = Quaternion.Euler(0f, currentTurnRate * 0.5f, 0f);
        Vector3 predictedDir = predictedRotation * transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + predictedDir * 2f);
    }
}
