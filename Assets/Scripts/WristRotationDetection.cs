using UnityEngine;

public class WristRotationDetection : MonoBehaviour
{
    [SerializeField] private Transform controller;
    [SerializeField] private Transform centerEyeAnchor;
    public bool isTwisting { get; private set; }
    public float lastTwistTime { get; private set; }
    public float gracePeriod = 0.5f;


    [Header("Settings")]
    public float minAngularVelocity = 80f;
    public float requiredAngle = 360f;
    //public float axisThreshold = 0.7f; // jak bardzo musi byæ "w osi"
    public float maxIdleTime = 0.2f;

    private Quaternion lastRotation;
    private float accumulatedTwistY = 0f;
    private float accumulatedTwistX = 0f;
    private float idleTimer = 0f;

    void Start()
    {
        lastRotation = controller.rotation;
    }

    void Update()
    {
        Debug.Log("Is twisting: " + isTwisting);
        float length = 0.2f;

        Debug.DrawRay(controller.position, controller.forward * length, Color.blue);
        Debug.DrawRay(controller.position, controller.right * length, Color.red);
        Debug.DrawRay(controller.position, controller.up * length, Color.green);


        if (controller.position.y < centerEyeAnchor.position.y)
        {
            isTwisting = false;
            accumulatedTwistY = 0f;
            accumulatedTwistX = 0f;
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
        float twistAmountX = localAxis.x;

        bool isMovingCorrectly = angularVelocity > minAngularVelocity;
            //&&
            //(Mathf.Abs(twistAmountY) > axisThreshold || Mathf.Abs(twistAmountX) > axisThreshold);

        if (isMovingCorrectly)
        {
            accumulatedTwistY += angle * Mathf.Sign(twistAmountY);
            accumulatedTwistX += angle * Mathf.Sign(twistAmountX);

            idleTimer = 0f;

            lastTwistTime = Time.time;
        }
        else
        {
            idleTimer += Time.deltaTime;
        }


        if (Mathf.Abs(accumulatedTwistY) >= requiredAngle || Mathf.Abs(accumulatedTwistX) >= requiredAngle)
        {
            Debug.Log("Full twist detected!");

            isTwisting = true;
            accumulatedTwistY = 0f;
            accumulatedTwistX = 0f;
        }

        if (Time.time - lastTwistTime > gracePeriod)
        {
            isTwisting = false;
            accumulatedTwistY = 0f;
            accumulatedTwistX = 0f;
        }

        if (idleTimer > maxIdleTime)
        {
            accumulatedTwistY = 0f;
            accumulatedTwistX = 0f;
        }

        lastRotation = current;
    }
}
