using UnityEngine;

public class Target : MonoBehaviour
{
    public GameObject target;
    public GameObject hitSmokePrefab;

    public void Catch()
    {
        //Debug.Log(gameObject.name + " caught!");

        if (GameManager.Instance != null)
        {
            Debug.Log("Adding time: " + GameManager.Instance.timeToAdd);
            CowAI cowAi = target.GetComponent<CowAI>();
            if (cowAi != null)
            {
                Debug.Log("Adding points: " + cowAi.pointsToAdd);
                GameManager.Instance.AddPoint(cowAi.pointsToAdd);
                
            }
            if (GameManager.Instance.timeToAdd > 0)
            {
                GameManager.Instance.AddTime();
            }
        }
        if (hitSmokePrefab != null)
        {
            GameObject fx = Instantiate(
                hitSmokePrefab,
                target.transform.position,
                Quaternion.identity
            );

            Destroy(fx, 1f);
        }

        Destroy(target);
    }
}