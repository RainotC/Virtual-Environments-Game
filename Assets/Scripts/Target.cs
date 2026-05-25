using UnityEngine;

public class Target : MonoBehaviour
{
    public void Catch()
    {
        Debug.Log(gameObject.name + " caught!");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddTime();
        }
        Destroy(gameObject);
    }
}
