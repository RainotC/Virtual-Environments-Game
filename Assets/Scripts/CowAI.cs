using UnityEngine;

public class CowAI : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float runSpeed = 3f;
    public float minWaitTime = 3f;
    public float maxWaitTime = 8f;
    public float moveRadius = 10f;
    public float scareDistance = 5f;
    public int pointsToAdd = 1;


    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool isRunningAway = false;
    private float waitTime;
    private Transform playerTransform;
    private Rigidbody rb;
    private bool stopped = false;

    private Animator anim; //ANIMACJA
    private float currentSpeed = 0f; //ANIMACJA


    private float moveTimer = 0f; //niewchodza w sciane
    public float moveDuration = 4f;//niewchodza w sciane
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        waitTime = Random.Range(minWaitTime, maxWaitTime);

        //////////////////////////////////////////////////
        anim = GetComponent<Animator>();                   //ANIMACJA
        if (anim != null)                                  //ANIMACJA
        {                                                  //ANIMACJA
            int randomIdle = Random.Range(0, 2);           //ANIMACJA
            anim.SetInteger("IdleStateIndex", randomIdle); //ANIMACJA
        }                                                  //ANIMACJA
        ///////////////////////////////////////////////////
    }

    void Update()
    {
        if (stopped) return;
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer < scareDistance && !isRunningAway)
            {
                StartFleeing();
            }
        }

        if (isMoving)
        {
            MoveTowardsTarget();

            moveTimer -= Time.deltaTime; //niewchodza w sciane
            if (moveTimer <= 0f) //niewchodza w sciane
            {
                // ANIMACJA //
                if (anim != null)           //ANIMACJA
                {                           //ANIMACJA
                    int randomIdle = Random.Range(0, 2);           //ANIMACJA
                    anim.SetInteger("IdleStateIndex", randomIdle); //ANIMACJA 
                }
                // ANIMACJA //

                isMoving = false;
                isRunningAway = false;
                currentSpeed = 0f;
                waitTime = Random.Range(minWaitTime, maxWaitTime);
            }
        }
        else
        {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0f)
            {
                SetNewRandomTarget();
            }
        }

        // ANIMACJA //
        if (anim != null)
        {
            anim.SetFloat("Speed", currentSpeed);
            anim.SetBool("IsRunning", isRunningAway);
        }
        // ANIMACJA //
    }

    void MoveTowardsTarget()
    {
        float speed = isRunningAway ? runSpeed : moveSpeed;
        currentSpeed = speed;
        Vector3 direction = (targetPosition - transform.position);
        direction.y = 0;

        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            transform.position += direction.normalized * speed * Time.deltaTime;
        }
    }

    void SetNewRandomTarget()
    {
        Vector2 randomPoint = Random.insideUnitCircle * moveRadius;
        targetPosition = transform.position + new Vector3(randomPoint.x, 0, randomPoint.y);
        isMoving = true;
        moveTimer = moveDuration; //niewchodza w sciane
    }

    void StartFleeing()
    {
        Vector3 directionAwayFromPlayer = (transform.position - playerTransform.position).normalized;
        targetPosition = transform.position + directionAwayFromPlayer * moveRadius;

        isMoving = true;
        isRunningAway = true;
        moveTimer = moveDuration; //niewchodza w sciane
    }

    public void StopCow()
    {
        stopped = true;
    }
}