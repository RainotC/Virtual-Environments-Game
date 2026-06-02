using UnityEngine;

public class CowAI : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float runSpeed = 3f;
    public float minWaitTime = 2f;
    public float maxWaitTime = 5f;
    public float moveRadius = 10f;
    public float scareDistance = 5f;

    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool isRunningAway = false;
    private float waitTime;
    private Transform playerTransform;
    private Rigidbody rb;
    private bool stopped = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        waitTime = Random.Range(minWaitTime, maxWaitTime);
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

            Vector3 flatPos = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 flatTarget = new Vector3(targetPosition.x, 0, targetPosition.z);

            if (Vector3.Distance(flatPos, flatTarget) < 0.5f)
            {
                isMoving = false;
                isRunningAway = false;
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
    }

    void MoveTowardsTarget()
    {
        float speed = isRunningAway ? runSpeed : moveSpeed;
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
    }

    void StartFleeing()
    {
        Vector3 directionAwayFromPlayer = (transform.position - playerTransform.position).normalized;
        targetPosition = transform.position + directionAwayFromPlayer * moveRadius;

        isMoving = true;
        isRunningAway = true;
    }
   
    public void StopCow()
    {
        stopped = true;
    }
}