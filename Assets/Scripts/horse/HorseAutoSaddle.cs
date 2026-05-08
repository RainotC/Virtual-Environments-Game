using UnityEngine;

public class AutoMountHorse : MonoBehaviour
{
    [Header("References")]
    public HorseController horseController;
    public OVRCameraRig playerRig;
    public SaddleBehaviour saddle;

    [Header("Mount Position")]
    public Transform mountPoint;

    private bool hasMounted = false;

    void Start()
    {
        Invoke("MountPlayer", 0.1f);
    }

    void MountPlayer()
    {
        if (hasMounted) return;
        //if (playerRig == null)
        //{
        //    Debug.LogError("nie ma OVRCameraRig");
        //    return;
        //}

        //if (horseController == null)
        //{
        //    Debug.LogError("nie ma HorseController");
        //    return;
        //}

        horseController.saddle = saddle;
        if (saddle != null)
        {
            saddle.isMounted = true;
        }
    }
}