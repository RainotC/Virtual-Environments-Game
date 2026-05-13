using UnityEngine;

public class HorseController : MonoBehaviour
{
    [Header("Visual Animation")]
    public Transform horseVisual; //MACIEJ
    public float bobSpeed = 6f; //MACIEJ
    public float bobAmount = 0.03f; //MACIEJ
    public float tiltAmount = 2f; //MACIEJ
    private Vector3 visualStartPos; //MACIEJ
    private Quaternion visualStartRot; //MACIEJ

    public Transform halter;

    public Transform playerRig;
    private float lastHorseYaw;
    public SaddleBehaviour saddle;

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
        AnimateHorseVisual(); //MACIEJ
        if (!isReinsGrabbed || saddle.isMounted == false)
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
        visualStartPos = horseVisual.localPosition; //MACIEJ
        visualStartRot = horseVisual.localRotation; //MACIEJ
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
    void AnimateHorseVisual() //MACIEJ
    {
        if (currentSpeed < 0.1f)
        {
            horseVisual.localPosition = Vector3.Lerp(
                horseVisual.localPosition,
                visualStartPos,
                Time.deltaTime * 5f);

            horseVisual.localRotation = Quaternion.Lerp(
                horseVisual.localRotation,
                visualStartRot,
                Time.deltaTime * 5f);

            return;
        }

        float sin = Mathf.Sin(Time.time * bobSpeed);

        Vector3 targetPos =
            visualStartPos + Vector3.up * (sin * bobAmount);

        horseVisual.localPosition = targetPos;

        Quaternion targetRot =
            visualStartRot * Quaternion.Euler(sin * tiltAmount, 0f, 0f);

        horseVisual.localRotation = targetRot;
    }
}
