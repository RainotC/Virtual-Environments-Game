using System;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;

public class Lasso : MonoBehaviour
{

    [SerializeField] private GameObject lassoPhysicalBall;
    [SerializeField] private GameObject lassoProjectile;
    [SerializeField] private Transform lassoAnchor; //propably shouldn't use this
    [SerializeField] private InputManager lassoControllerInputManager;

    [Header("Lasso rings")]
    [SerializeField] private Transform ringCorrect;
    [SerializeField] private Transform ringWrong;

    [Header("Throw settings")]
    public float lassoingDistance = 0.5f;
    public float succesfullThrowRopeLength = 2.5f;
    public float unsucessfullThrowRopeLength = 1f;
    public float throwLift = 1.5f; //does it work?
    public float ropeLength = 0.5f;
    public float throwForce = 1.5f;
    public float unsuccessfulThrowForce = 0.5f;

    [Header("Rope colors")]
    public Material lassoingMaterial;
    public Material notLassoingMaterial;
    public Material neutralMaterial;
    public Renderer ropeVisual;

    private bool isThrown = false;
    private Rigidbody projectileRB;
    private SpringJoint joint;

    public bool IsLassoing
    {
        get { return isLassoing; }
        set
        {
            if (isLassoing != value)
            {
                isLassoing = value;
                OnLassoingChanged(value);
            }
        }
    }
    private bool isLassoing = false;
    private WristRotationDetection wristRotationDetection;

    private Vector3 lastPos;
    Queue<Vector3> velocityBuffer = new Queue<Vector3>();
    private int velocityBufferSize = 10;

    private void Start()
    {
        lastPos = lassoAnchor.position;
        wristRotationDetection = GetComponent<WristRotationDetection>();

        projectileRB = lassoPhysicalBall.GetComponent<Rigidbody>();
        joint = lassoPhysicalBall.GetComponent<SpringJoint>();

        joint.maxDistance = lassoingDistance;
        joint.minDistance = lassoingDistance;

        lassoControllerInputManager.OnIndexTriggerPressed += () =>
        {
            ropeVisual.material = notLassoingMaterial;
        };
        lassoControllerInputManager.OnIndexTriggerHeld += () =>
        {
            Debug.Log("Index trigger held. Wrist twisting:");
            
            if (!isThrown && wristRotationDetection.isTwisting)
            {
                IsLassoing = true;
                Debug.Log("Started lassoing!");
            }
            else
            {
                IsLassoing = false;
            }
        };
        lassoControllerInputManager.OnIndexTriggerReleased += () =>
        {
            if (IsLassoing)
            {
                ThrowLasso();
                Debug.Log("Throwing lasso!");
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


    private void Update()
    {
        if (IsLassoing)
        {
            //Adding to velocity buffer current velocity
            Vector3 currentPos = lassoAnchor.position;
            Vector3 velocity = (currentPos - lastPos) / Time.deltaTime;
            velocityBuffer.Enqueue(velocity);
            if (velocityBuffer.Count > velocityBufferSize)
            {
                velocityBuffer.Dequeue();
            }
            lastPos = currentPos;
        }
    }


    private void ThrowLasso()
    {

        lassoProjectile.SetActive(true);
        Vector3 currentPos = lassoAnchor.position;
        Vector3 velocity = (currentPos - lastPos) / Time.deltaTime;

        isThrown = true;
        IsLassoing = false;
        joint.maxDistance = succesfullThrowRopeLength;
        joint.minDistance = succesfullThrowRopeLength;
        projectileRB.linearVelocity = Vector3.zero;
        projectileRB.angularVelocity = Vector3.zero;

        Vector3 throwVelocity = GetAverageVelocity();
        //throwVelocity.y = 0; //ignoring vertical, so it want just fall to ground 

        throwVelocity.y = Mathf.Max(throwVelocity.y, throwLift);
        projectileRB.linearVelocity = throwVelocity * throwForce;
        //rb.linearVelocity = lassoAnchor.forward * throwForce; //to trzeba zmieniæ na wyczytywanie wektora

        Debug.Log("Lasso thrown!");

    }

    private void UnsuccessfulLasso()
    {
        Debug.Log("Lasso throw unsuccessful!");
        isThrown = true;
        IsLassoing = false;
        joint.maxDistance = unsucessfullThrowRopeLength;
        joint.minDistance = lassoingDistance;
        projectileRB.linearVelocity = lassoAnchor.forward * unsuccessfulThrowForce;

    }

    private void ResetLasso()
    {
        lassoProjectile.SetActive(true);
        joint.maxDistance = lassoingDistance;
        joint.minDistance = lassoingDistance;
        projectileRB.linearVelocity = Vector3.zero;
        projectileRB.angularVelocity = Vector3.zero;


        lassoPhysicalBall.transform.position = lassoAnchor.position;
        
        isThrown = false;
        ringCorrect.gameObject.SetActive(false); // maybe should do this in some function 
        ringWrong.gameObject.SetActive(true);

        velocityBuffer.Clear();

        ropeVisual.material = neutralMaterial;

        Debug.Log("Lasso reset!");
    }


    public Vector3 GetAverageVelocity()
    {
        Vector3 sum = Vector3.zero;

        foreach (var v in velocityBuffer)
            sum += v;

        return sum / velocityBuffer.Count;
    }

    private void OnLassoingChanged(bool value)
    {
        Debug.Log("OnLassoingChanged");
        if (isThrown) return;
        if (value)
        {
            Debug.Log("OnLassoingChanged: Lassoing");
            ringCorrect.gameObject.SetActive(true);
            ringWrong.gameObject.SetActive(false);
            ropeVisual.material = lassoingMaterial;
        }
        else
        {
            Debug.Log("OnLassoingChanged: Not Lassoing");
            ringCorrect.gameObject.SetActive(false);
            ringWrong.gameObject.SetActive(true);
            ropeVisual.material = notLassoingMaterial;
        }
    }
}