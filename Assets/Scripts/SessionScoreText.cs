using UnityEngine;
using TMPro;

public class SessionScoreText : MonoBehaviour
{
    public TMP_Text scoreText;
    public string prefix = "";

    private void Update()
    {
        if (SessionScore.Instance == null || scoreText == null)
            return;

        scoreText.text = prefix + SessionScore.Instance.Score;
    }
}