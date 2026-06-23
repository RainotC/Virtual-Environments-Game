using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // opcjonalnie do restartu lub koñca gry

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Timer Settings")]
    public float startTime = 60f;          // startowy czas w sekundach
    public float timeToAdd = 10f;           // ile sekund dodaje krowa
    public TextMeshProUGUI timerText;       // UI do wyœwietlania czasu
    public TextMeshProUGUI scoreText;       // UI do wyœwietlania wyniku

    private float currentTime;
    private bool isGameOver = false;
    private int score = 0; // Zmienna do przechowywania wyniku


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        score = 0;
        currentTime = startTime;
        UpdateTimerUI();
        UpdateScoreUI();
    }

    void Update()
    {
        if (isGameOver) return;

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
            // Wyœwietl czas w formacie mm:ss lub sekundy
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            timerText.text = string.Format("Time left: {0:00}:{1:00}", minutes, seconds);
        }
    }

    // Metoda wywo³ywana przez krowê – dodaje czas
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
        timerText.text = "Game over!";
        
        Debug.Log("KONIEC CZASU – GAME OVER");
        // Tutaj mo¿esz dodaæ: zatrzymanie gry, ³adowanie ekranu koñca itp.
        // np. SceneManager.LoadScene("GameOverScene");
    }

    // Opcjonalnie – metoda do rêcznego dodawania dowolnej iloœci sekund
    public void AddCustomTime(float seconds)
    {
        if (isGameOver) return;
        currentTime += seconds;
        UpdateTimerUI();
    }
}