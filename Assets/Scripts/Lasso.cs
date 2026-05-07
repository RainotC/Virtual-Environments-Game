using System;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;

public class Lasso : MonoBehaviour
{

    [SerializeField] private GameObject lassoProjectile;
    [SerializeField] private Transform lassoAnchor; //propably shouldn't use this
    [SerializeField] private InputManager lassoControllerInputManager;

    [Header("Lasso rings")]
    [SerializeField] private Transform ringCorrect;
    [SerializeField] private Transform ringWrong;

    [Header("Throw settings")]
    public float lassoingDistance = 0.5f;
    public float thrownDistance = 2;
    public float throwLift = 1.5f;

    public float ropeLength = 0.5f;

    public float throwForce = 2f;
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
    private int velocityBufferSize = 3;

    private void Start()
    {
        lastPos = lassoAnchor.position;
        wristRotationDetection = GetComponent<WristRotationDetection>();

        projectileRB = lassoProjectile.GetComponent<Rigidbody>();
        joint = lassoProjectile.GetComponent<SpringJoint>();

        joint.maxDistance = lassoingDistance;
        joint.minDistance = lassoingDistance;


        lassoControllerInputManager.OnIndexTriggerHeld += () =>
        {
            if (!isThrown && wristRotationDetection.isTwisting)
            {
                IsLassoing = true;
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


        Vector3 currentPos = lassoAnchor.position;
        Vector3 velocity = (currentPos - lastPos) / Time.deltaTime;

        isThrown = true;
        IsLassoing = false;
        joint.maxDistance = thrownDistance;
        joint.minDistance = thrownDistance;
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
        projectileRB.linearVelocity = lassoAnchor.forward;

    }

    private void ResetLasso()
    {
        joint.maxDistance = lassoingDistance;
        joint.minDistance = lassoingDistance;
        projectileRB.linearVelocity = Vector3.zero;
        projectileRB.angularVelocity = Vector3.zero;


        lassoProjectile.transform.position = lassoAnchor.position;
        //lassoProjectile.transform.rotation = lassoAnchor.rotation;
        
        isThrown = false;
        IsLassoing = false;
        velocityBuffer.Clear();
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
        if (isThrown) return;
        if (value)
        {
            ringCorrect.gameObject.SetActive(true);
            ringWrong.gameObject.SetActive(false);
        }
        else
        {
            ringCorrect.gameObject.SetActive(false);
            ringWrong.gameObject.SetActive(true);
        }
    }
}