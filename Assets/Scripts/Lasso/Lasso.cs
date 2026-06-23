using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HandRotationDetection))]
public class Lasso : MonoBehaviour
{
    public enum LassoState
    {
        Down,
        Up,
        Lassoing
    }

    private const int VelocityBufferSize = 9;

    [SerializeField] private GameObject lassoPhysicalBall; //The object player spins when lassoing
    [SerializeField] private GameObject lassoProjectile; //The object player throws
    [SerializeField] private Transform lassoAnchor; 
    [SerializeField] private GameObject lassoProjectileParent;
    [SerializeField] private InputManager lassoControllerInputManager;

    [Header("Lasso rings")]
    [SerializeField] private Transform ringLassoing;
    [SerializeField] private Transform ringUp;
    [SerializeField] private Transform ringDown;

    [Header("Throw settings")] //need cleanup
    [SerializeField] private float lassoingDistance = 0.5f;
    [SerializeField] private float successfulThrowRopeLength = 2.5f;
    [SerializeField] private float unsucessfulThrowRopeLength = 1f;
    [SerializeField] private float throwSpeed = 3f;


    private bool isThrown;
    private Rigidbody ballRB;
    private SpringJoint joint;

    private HandRotationDetection handRotationDetection;

    private Vector3 lastPos;
    private readonly Queue<Vector3> velocityBuffer = new(); //used for calculating throw direction
    private LassoState currentState;

    public LassoState State
    {
        get => currentState;
        set 
        {
            if (currentState == value)
                return;
            
            currentState = value;
            UpdateVisualLassoState();
        }
    }

    private void UpdateVisualLassoState()
    {
        ringLassoing.gameObject.SetActive(State == LassoState.Lassoing);
        ringUp.gameObject.SetActive(State == LassoState.Up);
        ringDown.gameObject.SetActive(State == LassoState.Down);
    }


    private void Awake()
    {
        handRotationDetection = GetComponent<HandRotationDetection>();

        ballRB = lassoPhysicalBall.GetComponent<Rigidbody>();

        if (ballRB == null)
        {
            Debug.LogWarning("Rigidbody not found ");
        }
        
        joint = lassoPhysicalBall.GetComponent<SpringJoint>();
        if (joint == null)
        {
            Debug.LogWarning("SpringJoint not found ");
        }
        else
        {
            //Max and min the same so the ball is on a fixed distance from the anchor
            joint.maxDistance = lassoingDistance;
            joint.minDistance = lassoingDistance;
        }
    }

    private void OnEnable()
    {
        //Reset ball velocity, because it gets high when teleporting player in the begining
        ballRB.linearVelocity = Vector3.zero;
        ballRB.angularVelocity = Vector3.zero;
        lastPos = lassoAnchor.position;

        lassoControllerInputManager.OnIndexTriggerHeld += HandleTriggerHeld;
        lassoControllerInputManager.OnIndexTriggerReleased += HandleTriggerReleased;

    }

    private void OnDisable()
    {
        lassoControllerInputManager.OnIndexTriggerHeld -= HandleTriggerHeld;
        lassoControllerInputManager.OnIndexTriggerReleased -= HandleTriggerReleased;
    }

    private void HandleTriggerHeld()
    {
        if (!isThrown && handRotationDetection.isTwisting)
        {
            State = LassoState.Lassoing;
        }
        else
        {
            State = LassoState.Up;
        }
    }

    private void HandleTriggerReleased()
    {
        if (State == LassoState.Lassoing)
        {
            ThrowLasso();
        }
        else if(State == LassoState.Up && !isThrown)
        {
            UnsuccessfulLassoThrow();
        }
    }


    private void Update()
    {
        if (State != LassoState.Lassoing)
            return;
        
        Vector3 currentPos = lassoAnchor.position;
        Vector3 velocity = (currentPos - lastPos) / Time.deltaTime;
        velocityBuffer.Enqueue(velocity);
        if (velocityBuffer.Count > VelocityBufferSize)
        {
            velocityBuffer.Dequeue();
        }
        lastPos = currentPos;
        
    }


    private void ThrowLasso()
    {
        lassoProjectile.transform.SetParent(null);
        isThrown = true;
        TryCatchTarget();
    }


    private void TryCatchTarget()
    {
        Vector3 velocity = GetAverageVelocity();
        Vector3 flatVelocity = new Vector3(velocity.x, 0f, velocity.z);
        Vector3 dir = flatVelocity.normalized;

        Ray ray = new Ray(lassoAnchor.transform.position, dir);
        if (Physics.Raycast(ray, out RaycastHit hit, successfulThrowRopeLength, layerMask: LayerMask.GetMask("Target")))
        {
            GameObject caughtObj = hit.collider.gameObject;
            CowAI cowAI = caughtObj.GetComponent<CowAI>();
            if (cowAI != null)
            {
                cowAI.StopCow();
            }
            StartCoroutine(MoveToPoint(caughtObj.transform.position, caughtObj));
        }
        else
        {
            Vector3 fallbackPoint = ray.origin + dir * (successfulThrowRopeLength);
            fallbackPoint.y = 0;
            StartCoroutine(MoveToPoint(fallbackPoint));
        }
    }

    private void UnsuccessfulLassoThrow()
    {

        lassoProjectile.transform.SetParent(null);
        isThrown = true;

        Vector3 dir = lassoAnchor.forward;
        Vector3 fallbackPoint = lassoAnchor.transform.position + dir * unsucessfulThrowRopeLength;
        fallbackPoint.y = 0;
        StartCoroutine(MoveToPoint(fallbackPoint));
    }


    private IEnumerator MoveToPoint(Vector3 target, GameObject targetObject = null)
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
        if (targetObject != null)
        {
            Target targetScript = targetObject.GetComponent<Target>();
            if(targetScript != null)
            {
                targetScript.Catch();
            }
        }
        ResetLasso();
    }


    private void ResetLasso()
    {
        lassoProjectile.transform.SetParent(lassoProjectileParent.transform, false);

        ballRB.linearVelocity = Vector3.zero;
        ballRB.angularVelocity = Vector3.zero;


        lassoPhysicalBall.transform.position = lassoAnchor.position;
        lassoProjectile.transform.localPosition = Vector3.zero;

        isThrown = false;
        State = LassoState.Down;
        velocityBuffer.Clear();

    }


    private Vector3 GetAverageVelocity()
    {
        if (velocityBuffer.Count == 0)
            return Vector3.forward;

        Vector3 sum = Vector3.zero;
        foreach (var v in velocityBuffer)
            sum += v;

        return sum / velocityBuffer.Count;
    }
}