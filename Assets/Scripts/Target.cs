using UnityEngine;

public class Target : MonoBehaviour
{
    public GameObject target;
    public void Catch()
    {
        Debug.Log(gameObject.name + " caught!");
        if (GameManager.Instance != null)
        {
            CowAI cowAi = target.GetComponent<CowAI>();
            if (cowAi != null)
            {
                GameManager.Instance.AddPoint(cowAi.pointsToAdd);
            }
            
        }
        Destroy(target);
    }
}
