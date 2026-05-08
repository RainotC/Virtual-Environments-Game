using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public TextMeshProUGUI scoreText;
    public int currentScore = 0;

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
        UpdateScoreUI();
    }

    public void AddPoints(int points)
    {
        currentScore += points;
        UpdateScoreUI();
        Debug.Log("TWOJ PUNKYT" + currentScore);
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "AKTUALNE PUNKTY: " + currentScore;
        }
    }
}