using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Timer Settings")]
    public float startTime = 60f;
    public float timeToAdd = 10f;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;

    private float currentTime;
    private bool isGameOver = false;
    [SerializeField]private int score = 0;

    [Header("Scene Switching")]
    public string gameOverSceneName = "MainMenu";

    public int Score => score;

    void Awake()
    {
        
    }

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        score = 0;
        currentTime = startTime;
        UpdateTimerUI();
        UpdateScoreUI();
    }

    void Update()
    {
        if (isGameOver)
        {
            if (OVRInput.GetDown(OVRInput.Button.Two))
            {
                SceneManager.LoadScene(gameOverSceneName);
            }

            return;
        }

        currentTime -= Time.deltaTime;
        UpdateTimerUI();

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            GameOver();
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            timerText.text = string.Format("Time left: {0:00}:{1:00}", minutes, seconds);
        }
    }
    public void AddTime()
    {
        if (isGameOver) return;
        currentTime += timeToAdd;
        UpdateTimerUI();
    }


    public void AddPoint(int point)
    {
        if (isGameOver) return;
        score += point;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
    private void GameOver()
    {
        isGameOver = true;
        timerText.text = "Game over! \n Press B on the controller to return to the main menu.";

        if (SessionScore.Instance != null)
        {
            SessionScore.Instance.SetScore(score);
        }

        Debug.Log("KONIEC CZASU – GAME OVER");
    }

    public void AddCustomTime(float seconds)
    {
        if (isGameOver) return;
        currentTime += seconds;
        UpdateTimerUI();
    }
}