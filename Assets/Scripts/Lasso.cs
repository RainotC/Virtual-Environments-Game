using TMPro;
using UnityEngine;

public class Lasso : MonoBehaviour
{
    [SerializeField] private TMP_Text infoText;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Lasso collided with: " + other.gameObject.name);
        if (other.CompareTag("Target"))
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
