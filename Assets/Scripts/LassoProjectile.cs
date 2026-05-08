using TMPro;
using UnityEngine;

public class LassoProjectile : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        CowAI cow = other.gameObject.GetComponent<CowAI>();
        if (cow != null)
        {
            cow.Catch();
        }
    }
    public void OnTargetCatch()
    {
        Debug.Log("Target caught!");

    }
}
