using System.Net;
using TMPro;
using UnityEngine;

public class Lasso : MonoBehaviour
{
    [SerializeField] private TMP_Text infoText;

    [SerializeField] private WristRotationDetection wristRotationDetection;
    [SerializeField] private GameObject lassoProjectile;
    [SerializeField] private Transform lassoAnchor;
    [SerializeField] private InputManager lassoControllerInputManager;

    public float lassoingDistance = 0.5f;
    public float thrownDistance = 2f;

    public float ropeLength = 0.5f;

    public Transform hand; //propably should use this
    public float throwForce = 4f;
    private bool isThrown = false;
    private Rigidbody rb;
    private SpringJoint joint;
    private bool isLassoing = false;


    private void Start()
    {

        rb = lassoProjectile.GetComponent<Rigidbody>();
        joint = lassoProjectile.GetComponent<SpringJoint>();

        joint.maxDistance = lassoingDistance;
        joint.minDistance = lassoingDistance;


        lassoControllerInputManager.OnTriggerHeld += () =>
        {
            if (!isThrown && wristRotationDetection.isTwisting)
            {
                isLassoing = true;
                Debug.Log("Lassoing...");
            }
            else
            {
                isLassoing = false;
            }
        };
        lassoControllerInputManager.OnTriggerReleased += () =>
        {
            if (isLassoing)
            {
                ThrowLasso();
                isLassoing = false;
            }
        };

    }
    private void ThrowLasso()
    {
        joint.maxDistance = thrownDistance;
        joint.minDistance = thrownDistance;

        rb.linearVelocity = hand.forward * throwForce;


        isThrown = true;
        Debug.Log("Lasso thrown!");

    }

}