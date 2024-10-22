using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TMP_Text scoreText;
    public TMP_Text levelText;
    public GameObject gameOverPanel;

    void Awake()
    {
        Instance = this;
    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    public void UpdateLevel(int level)
    {
        levelText.text = $"Level: {level}";
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
    }
}
