using System.Net;
using TMPro;
using UnityEngine;

public class Lasso : MonoBehaviour
{

    [SerializeField] private GameObject lassoProjectile;
    [SerializeField] private Transform lassoAnchor; //propably shouldn't use this
    [SerializeField] private InputManager lassoControllerInputManager;

    public float lassoingDistance = 0.5f;
    public float thrownDistance = 2f;

    public float ropeLength = 0.5f;

    public float throwForce = 4f;
    private bool isThrown = false;
    private Rigidbody rb;
    private SpringJoint joint;
    private bool isLassoing = false;
    private WristRotationDetection wristRotationDetection;



    private void Start()
    {
        wristRotationDetection = GetComponent<WristRotationDetection>();
        rb = lassoProjectile.GetComponent<Rigidbody>();
        joint = lassoProjectile.GetComponent<SpringJoint>();

        joint.maxDistance = lassoingDistance;
        joint.minDistance = lassoingDistance;


        lassoControllerInputManager.OnIndexTriggerHeld += () =>
        {
            if (!isThrown && wristRotationDetection.isTwisting)
            {
                isLassoing = true;
            }
            else
            {
                isLassoing = false;
            }
        };
        lassoControllerInputManager.OnIndexTriggerReleased += () =>
        {
            if (isLassoing)
            {
                ThrowLasso();
            }
            else if(!isThrown)
            {
                UnsuccessfulLasso();
            }
        };
        lassoControllerInputManager.OnHandTriggerPressed += () =>
        {
            if (isThrown)
            {
                ResetLasso();
            }
        };

    }
    private void ThrowLasso()
    {
        isLassoing = false;
        joint.maxDistance = thrownDistance;
        joint.minDistance = thrownDistance;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.linearVelocity = lassoAnchor.forward * throwForce;


        isThrown = true;
        Debug.Log("Lasso thrown!");

    }

    private void UnsuccessfulLasso()
    {
        Debug.Log("Lasso throw unsuccessful!");
        isThrown = true;
        rb.linearVelocity = lassoAnchor.forward;

    }

    private void ResetLasso()
    {
        joint.maxDistance = lassoingDistance;
        joint.minDistance = lassoingDistance;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        lassoProjectile.transform.position = lassoAnchor.position;
        lassoProjectile.transform.rotation = lassoAnchor.rotation;
        isThrown = false;
        isLassoing = false;
        Debug.Log("Lasso reset!");
    }

}