using TMPro;
using UnityEngine;

public class LassoProjectile : MonoBehaviour
{
    [SerializeField] private TMP_Text infoText;
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Lasso collided with: " + other.gameObject.name);
        if (other.gameObject.CompareTag("Target"))
        {
            OnTargetCatch();
        }
    }
    public void OnTargetCatch()
    {
        Debug.Log("Target caught!");
        infoText.text = "Catched";

    }
}
