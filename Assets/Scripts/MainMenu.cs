using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("PRZYCISK WCISNIEWTY ");
        if (Application.CanStreamedLevelBeLoaded("LevelDesign"))
        {
            Debug.Log("£adowanie sceny: LevelDesign");
            SceneManager.LoadScene("LevelDesign");
        }
    }
}