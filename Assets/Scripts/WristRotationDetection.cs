using UnityEngine;

public class WristRotationDetection : MonoBehaviour
{
    [SerializeField] private Transform controller;
    [SerializeField] private Transform centerEyeAnchor;
    public bool isTwisting { get; private set; }
    public float lastTwistTime { get; private set; }
    public float gracePeriod = 0.3f;


    [Header("Settings")]
    public float minAngularVelocity = 80f;
    public float requiredAngle = 360f;
    public float axisThreshold = 0.7f; // jak bardzo musi by? "w osi"
    public float maxIdleTime = 0.2f;

    private Quaternion lastRotation;
    private float accumulatedTwistY = 0f;
    private float accumulatedTwistZ = 0f;
    private float idleTimer = 0f;

    void Start()
    {
        lastRotation = controller.rotation;
    }

    void Update()
    {
        if (controller.position.y < centerEyeAnchor.position.y)
        {
            isTwisting = false;
            accumulatedTwistY = 0f;
            accumulatedTwistZ = 0f;
            idleTimer = 0f;
            return;
        }

        Quaternion current = controller.rotation;
        Quaternion delta = current * Quaternion.Inverse(lastRotation);

        delta.ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > 180f) angle -= 360f;

        float angularVelocity = Mathf.Abs(angle) / Time.deltaTime;

        Vector3 localAxis = controller.InverseTransformDirection(axis);
        float twistAmountY = localAxis.y;
        float twistAmountZ = localAxis.z;

        bool isMovingCorrectly =
            angularVelocity > minAngularVelocity &&
            (Mathf.Abs(twistAmountY) > axisThreshold || Mathf.Abs(twistAmountZ) > axisThreshold);

        if (isMovingCorrectly)
        {
            accumulatedTwistY += angle * Mathf.Sign(twistAmountY);
            accumulatedTwistZ += angle * Mathf.Sign(twistAmountZ);

            idleTimer = 0f;

            lastTwistTime = Time.time;
        }
        else
        {
            idleTimer += Time.deltaTime;
        }


        if (Mathf.Abs(accumulatedTwistY) >= requiredAngle || Mathf.Abs(accumulatedTwistZ) >= requiredAngle)
        {
            isTwisting = true;
            Debug.Log("Full twist detected!");
            accumulatedTwistY = 0f;
            accumulatedTwistZ = 0f;
        }

        if (Time.time - lastTwistTime > gracePeriod)
        {
            isTwisting = false;
            accumulatedTwistY = 0f;
            accumulatedTwistZ = 0f;
        }

        if (idleTimer > maxIdleTime)
        {
            accumulatedTwistY = 0f;
            accumulatedTwistZ = 0f;
        }

        lastRotation = current;
    }
}