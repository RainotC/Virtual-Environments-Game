using System.Net;
using TMPro;
using UnityEngine;

public class Lasso : MonoBehaviour
{
    [SerializeField] private TMP_Text infoText;

    [SerializeField] private WristRotationDetection wristRotationDetection;
    [SerializeField] private GameObject lassoProjectile;
    [SerializeField] private Transform lassoAnchor;

    public float lassoingDistance = 0.5f;
    public float thrownDistance = 2f;

    public float ropeLength = 0.5f;

    public Transform hand;
    public float throwForce = 4f;
    private bool isThrown = false;
    private Rigidbody rb;
    private SpringJoint joint;

    private void Start()
    {
        rb = lassoProjectile.GetComponent<Rigidbody>();
        joint = lassoProjectile.GetComponent<SpringJoint>();

        joint.maxDistance = lassoingDistance;
        joint.minDistance = lassoingDistance;

    }

    private void Update()
    {

        if (wristRotationDetection.isTwisting)
        {
            //better to have lassoing on button pressed and throw on release
            infoText.text = "Lassoing";
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                Debug.Log("Trigger pressed while twisting!");
                ThrowLasso();
            }
        }
        else
        {
            infoText.text = "Idle";
        }
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