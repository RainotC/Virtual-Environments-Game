using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Lasso : MonoBehaviour
{

    [SerializeField] private GameObject lassoPhysicalBall;
    [SerializeField] private GameObject lassoProjectile; //shit you throw, with 000 transform
    [SerializeField] private Transform lassoAnchor; //propably shouldn't use this
    [SerializeField] private GameObject lassoProjectileParent;
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
    public float throwForce = 1f;
    public float unsuccessfulThrowForce = 0.5f;
    public float throwSpeed = 3f;
    private AnimationCurve throwCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Rope colors")]
    public Material lassoingMaterial;
    public Material notLassoingMaterial;
    public Material neutralMaterial;
    public Material needsResetMaterial;
    public Renderer ropeVisual;

    private bool isThrown = false;
    private Rigidbody ballRB;
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
    private bool canReset = false;

    private Vector3 lastPos;
    Queue<Vector3> velocityBuffer = new Queue<Vector3>();
    private int velocityBufferSize = 9;//maybe we should also take some after throw?????

    private void Start()
    {
        lastPos = lassoAnchor.position;
        wristRotationDetection = GetComponent<WristRotationDetection>();

        ballRB = lassoPhysicalBall.GetComponent<Rigidbody>();
        if (ballRB == null)
        {
            Debug.Log("Rigidbody not found ");
        }
        else
        {
            Debug.Log("Rigidbody found ");
            Debug.Log("Rigidbody mass: " + ballRB.mass);
        }
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
                ropeVisual.material = needsResetMaterial;
                ThrowLasso();
                Debug.Log("Throwing lasso!");
            }
            else if(!isThrown)
            {
                ropeVisual.material = needsResetMaterial;
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
        ballRB.linearVelocity = Vector3.zero;
        ballRB.angularVelocity = Vector3.zero;

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
        lassoProjectile.transform.SetParent(null);
        RaycastHitTarget();
        //if (!hitTarget)
        //{
        //    lassoProjectile.transform.SetParent(null);
        //    lassoProjectile.AddComponent<Rigidbody>();



        //    Vector3 currentPos = lassoAnchor.position;
        //    Vector3 velocity = (currentPos - lastPos) / Time.deltaTime;
        //    //ballRB.linearVelocity = Vector3.zero;
        //    //ballRB.angularVelocity = Vector3.zero;

        //    //joint.maxDistance = succesfullThrowRopeLength;
        //    //joint.minDistance = succesfullThrowRopeLength;

        //    Vector3 throwVelocity = GetAverageVelocity();
        //    throwVelocity.y = 5.0f;

        //    throwVelocity.y = Mathf.Max(throwVelocity.y, throwLift);
        //    //ballRB.linearVelocity = throwVelocity * throwForce;
        //    ////rb.linearVelocity = lassoAnchor.forward * throwForce; //to trzeba zmienić na wyczytywanie wektora

        //    //Debug.Log("Lasso thrown!");
            
        //    lassoProjectile.GetComponent<Rigidbody>().linearVelocity = throwVelocity * throwForce;

        //    canReset = true;
        //}
        isThrown = true;
        IsLassoing = false;
    }

    private void UnsuccessfulLasso()
    {
        Debug.Log("Lasso throw unsuccessful!");
        isThrown = true;
        IsLassoing = false;
        joint.maxDistance = unsucessfullThrowRopeLength;
        joint.minDistance = lassoingDistance;
        ballRB.linearVelocity = lassoAnchor.forward * unsuccessfulThrowForce;
        canReset = true;
    }

    private void ResetLasso()
    {
        if (!canReset) return;
        lassoProjectile.transform.SetParent(lassoProjectileParent.transform, false);
        joint.maxDistance = lassoingDistance;
        joint.minDistance = lassoingDistance;
        ballRB.linearVelocity = Vector3.zero;
        ballRB.angularVelocity = Vector3.zero;


        lassoPhysicalBall.transform.position = lassoAnchor.position;
        lassoProjectile.transform.localPosition = Vector3.zero;

        isThrown = false;
        ringCorrect.gameObject.SetActive(false); // maybe should do this in some function 
        ringWrong.gameObject.SetActive(true);

        velocityBuffer.Clear();

        ropeVisual.material = neutralMaterial;
        //joint.connectedBody = lassoAnchor;
        Debug.Log("Lasso reset!");
        canReset = false;
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
        }
        else
        {
            Debug.Log("OnLassoingChanged: Not Lassoing");
            ringCorrect.gameObject.SetActive(false);
            ringWrong.gameObject.SetActive(true);
        }
    }


    private void RaycastHitTarget()
    {
        Vector3 velocity = GetAverageVelocity();
        Vector3 flatVelocity = new Vector3(velocity.x, 0f, velocity.z);
        Vector3 dir = flatVelocity.normalized;

        Ray ray = new Ray(lassoAnchor.transform.position, flatVelocity.normalized);
        Debug.Log("Raycasting with velocity: " + flatVelocity.magnitude);
        if (Physics.Raycast(ray, out RaycastHit hit, succesfullThrowRopeLength, layerMask: LayerMask.GetMask("Target")))
        {
            Debug.Log("Trafiono w: " + hit.collider.name);
            GameObject caughtObj = hit.collider.gameObject;
            if (caughtObj.GetComponent<CowAI>() != null)
            {
                caughtObj.GetComponent<CowAI>().StopCow();
            }
            StartCoroutine(MoveToPoint(caughtObj.transform.position, caughtObj));
        }
        else
        {
            Vector3 fallbackPoint = ray.origin + dir * (succesfullThrowRopeLength-0.5f); // idk czy to 0.5 coś da pozytywnego
            fallbackPoint.y = 0;
            Debug.Log("Nie trafiono, używam punktu fallback: " + fallbackPoint);

            StartCoroutine(MoveToPoint(fallbackPoint));
        }
    }

    IEnumerator MoveToPoint(Vector3 target, GameObject targetObject=null)
    {
        while (Vector3.Distance(lassoProjectile.transform.position, target) > 0.1f)
        {
            lassoProjectile.transform.position = Vector3.MoveTowards(
                lassoProjectile.transform.position,
                target,
                throwSpeed * Time.deltaTime
            );

            yield return null;
        }
        if(targetObject != null)
        {
            targetObject.GetComponent<Target>().Catch();
        }
        canReset = true;
    }


    //IEnumerator MoveToTargetPoint(Vector3 targetPoint, GameObject obj)
    //{

    //    while (Vector3.Distance(lassoProjectile.transform.position, obj.transform.position) > 0.1f)
    //    {
    //        // Make the target kinematic to prevent physics interference
    //        lassoProjectile.transform.position = Vector3.MoveTowards(
    //            lassoProjectile.transform.position,
    //            obj.transform.position,
    //            speed * Time.deltaTime
    //        );

    //        yield return null;
    //    }

    //    obj.GetComponent<Target>().Catch();
    //    canReset = true;
    //}

    //IEnumerator MoveToPoint(Vector3 targetPoint)
    //{

    //    while (Vector3.Distance(lassoProjectile.transform.position, targetPoint) > 0.1f)
    //    {
    //        lassoProjectile.transform.position = Vector3.MoveTowards(
    //            lassoProjectile.transform.position,
    //            targetPoint,
    //            speed * Time.deltaTime
    //        );

    //        yield return null;
    //    }

    //    canReset = true;
    //}
}