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

    private float currentTime;
    private bool isGameOver = false;

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
        currentTime = startTime;
        UpdateTimerUI();
    }

    void Update()
    {
        if (isGameOver) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            GameOver();
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            // Wyœwietl czas w formacie mm:ss lub sekundy
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    // Metoda wywo³ywana przez krowê – dodaje czas
    public void AddTime()
    {
        if (isGameOver) return;
        currentTime += timeToAdd;
        Debug.Log($"Dodano {timeToAdd} sekund! Aktualny czas: {currentTime:F1}");
        UpdateTimerUI();
    }

    private void GameOver()
    {
        isGameOver = true;
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