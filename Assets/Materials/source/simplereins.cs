using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Simplereins : MonoBehaviour
{
    public Transform previousBone;
    public Transform nextBone;
    public bool isFixed = false;
    public bool isMidPoint = false;
    public Transform midPointController;
    public float mass = 0.5f;
    public float spring = 1000f;
    public float damper = 50f;
    public float angleLimit = 45f;
    private Rigidbody rb;
    private ConfigurableJoint jointToPrevious;
    private ConfigurableJoint jointToNext;

    private void Start()
    {
        InitializePhysics();
        CreateJoints();
    }

    private void Update()
    {
        if (isMidPoint && midPointController != null)
        {
            transform.position = midPointController.position;
            transform.rotation = midPointController.rotation;
        }
    }

    private void InitializePhysics()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.mass = mass;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 1f;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        if (isFixed)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        else if (isMidPoint)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        else
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // Sprawdź czy ma Collider
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            CapsuleCollider capsule = gameObject.AddComponent<CapsuleCollider>();
            capsule.radius = 0.05f;
            capsule.height = 0.3f;
            capsule.direction = 2;
        }
    }

    private void CreateJoints()
    {
        if (previousBone != null)
        {
            Rigidbody prevRB = previousBone.GetComponent<Rigidbody>();
            if (prevRB != null)
            {
                jointToPrevious = CreateJoint(prevRB, true);
            }
        }
        if (nextBone != null)
        {
            Rigidbody nextRB = nextBone.GetComponent<Rigidbody>();
            if (nextRB != null)
            {
                jointToNext = CreateJoint(nextRB, false);
            }
        }
    }

    private ConfigurableJoint CreateJoint(Rigidbody connectedBody, bool isPrevious)
    {
        ConfigurableJoint joint = gameObject.AddComponent<ConfigurableJoint>();
        joint.connectedBody = connectedBody;
        joint.autoConfigureConnectedAnchor = false;

        if (isPrevious)
        {
            joint.anchor = Vector3.back * 0.15f;
            joint.connectedAnchor = Vector3.forward * 0.15f;
        }
        else
        {
            joint.anchor = Vector3.forward * 0.15f;
            joint.connectedAnchor = Vector3.back * 0.15f;
        }
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;

        joint.angularXMotion = ConfigurableJointMotion.Limited;
        joint.angularYMotion = ConfigurableJointMotion.Limited;
        joint.angularZMotion = ConfigurableJointMotion.Limited;

        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = angleLimit;
        joint.lowAngularXLimit = limit;
        joint.highAngularXLimit = limit;
        joint.angularYLimit = limit;
        joint.angularZLimit = limit;

        joint.linearLimit = new SoftJointLimit { limit = 0.01f };
        joint.xDrive = new JointDrive { positionSpring = spring, positionDamper = damper, maximumForce = float.MaxValue };
        joint.yDrive = new JointDrive { positionSpring = spring, positionDamper = damper, maximumForce = float.MaxValue };
        joint.zDrive = new JointDrive { positionSpring = spring, positionDamper = damper, maximumForce = float.MaxValue };

        joint.angularXDrive = new JointDrive { positionSpring = spring * 0.1f, positionDamper = damper * 0.1f, maximumForce = float.MaxValue };
        joint.angularYZDrive = new JointDrive { positionSpring = spring * 0.1f, positionDamper = damper * 0.1f, maximumForce = float.MaxValue };

        return joint;
    }

    public void SetPosition(Vector3 position)
    {
        if (rb != null)
        {
            if (rb.isKinematic)
            {
                transform.position = position;
            }
            else
            {
                rb.MovePosition(position);
            }
        }
    }

    public void SetRotation(Quaternion rotation)
    {
        if (rb != null)
        {
            if (rb.isKinematic)
            {
                transform.rotation = rotation;
            }
            else
            {
                rb.MoveRotation(rotation);
            }
        }
    }
    

    private void OnValidate()
    {
        if (previousBone == transform || nextBone == transform)
        {
            Debug.LogError($"Kość {gameObject.name} nie może być połączona sama ze sobą!");
            previousBone = null;
            nextBone = null;
        }

        if (previousBone == nextBone && previousBone != null)
        {
            Debug.LogWarning($"Kość {gameObject.name} ma tego samego poprzednika i następcę!");
        }
        if (isMidPoint)
        {
            Rigidbody rbCheck = GetComponent<Rigidbody>();
            if (rbCheck != null)
            {
                rbCheck.useGravity = false;
                rbCheck.isKinematic = true;
            }
        }
    }

    public void SetMidPointController(Transform controller)
    {
        midPointController = controller;
        isMidPoint = true;
        Rigidbody rbCheck = GetComponent<Rigidbody>();
        if (rbCheck != null)
        {
            rbCheck.useGravity = false;
            rbCheck.isKinematic = true;
        }
    }
}