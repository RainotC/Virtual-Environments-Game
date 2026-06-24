using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TutorialTarget : MonoBehaviour
{
    public GameObject target;
    public TMP_Text tutorialText;
    public string congratulationsText = "Congratulations!";
    public string normalGameSceneName = "Game";

    [Header("Tutorial 1")]
    public string tutorial2SceneName = "LassoTutorial2";
    public float tutorial1Delay = 10f;

    [Header("Tutorial 2")]
    public int catchesNeeded = 3;
    public float countdownSeconds = 30f;

    [Header("Debug")]
    public bool useDebugTimer = false;
    public float debugTimerSeconds = 5f;

    private static int tutorial2CatchCount = 0;
    private bool alreadyCaught = false;
    public GameObject hitSmokePrefab;
    public void TutorialCatch()
    {
        if (alreadyCaught) return;
        alreadyCaught = true;
        if (hitSmokePrefab != null)
        {
            GameObject fx = Instantiate(
                hitSmokePrefab,
                target.transform.position,
                Quaternion.identity
            );

            Destroy(fx, 1f);
        }

        if (tutorialText != null)
            tutorialText.text = congratulationsText;

        StartCoroutine(DebugSkipTutorial());

        StartCoroutine(ChangeSceneAfterDelay(tutorial2SceneName, tutorial1Delay));
        HideTarget();
    }

    public void Tutorial2Catch()
    {
        if (alreadyCaught) return;
        alreadyCaught = true;

        tutorial2CatchCount++;
        if (hitSmokePrefab != null)
        {
            GameObject fx = Instantiate(
                hitSmokePrefab,
                target.transform.position,
                Quaternion.identity
            );

            Destroy(fx, 1f);
        }

        if (tutorial2CatchCount >= catchesNeeded)
        {
            tutorial2CatchCount = 0;
            StartCoroutine(CongratulationsCountdownAndScene());
        }
        HideTarget();
    }

    private void AddCowPoints()
    {
        Debug.Log(gameObject.name + " caught!");

        if (GameManager.Instance == null || target == null) return;

        CowAI cowAi = target.GetComponent<CowAI>();

        if (cowAi != null)
            GameManager.Instance.AddPoint(cowAi.pointsToAdd);
    }

    private IEnumerator ChangeSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator CongratulationsCountdownAndScene()
    {
        float timeLeft = countdownSeconds;

        while (timeLeft > 0)
        {
            if (tutorialText != null)
                tutorialText.text = congratulationsText + "\nGame starts in " + Mathf.CeilToInt(timeLeft);

            yield return new WaitForSeconds(1f);
            timeLeft--;
        }

        SceneManager.LoadScene(normalGameSceneName);
    }
    private IEnumerator DebugSkipTutorial()
    {
        float timeLeft = debugTimerSeconds;

        while (timeLeft > 0)
        {
            if (tutorialText != null)
                tutorialText.text = $"Congratulations! Loading Tutorial 2 in {Mathf.CeilToInt(timeLeft)}";

            yield return new WaitForSeconds(1f);
            timeLeft--;
        }

        SceneManager.LoadScene(tutorial2SceneName);
    }

    private void HideTarget()
    {
        if (target == null) return;

        foreach (Renderer renderer in target.GetComponentsInChildren<Renderer>())
            renderer.enabled = false;

        foreach (Collider collider in target.GetComponentsInChildren<Collider>())
            collider.enabled = false;
    }
}