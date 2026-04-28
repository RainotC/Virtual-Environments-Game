using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private Material caughtMaterial;
    public void Catch()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material = caughtMaterial;
    }
}
