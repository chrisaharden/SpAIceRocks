using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TMP_Text scoreText;
    public TMP_Text levelText;
    public GameObject gameOverPanel;
    public GameObject creditsPanel;
    public Button creditsButton;

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

    public void ToggleCreditsPanel()
    {
        if (creditsPanel != null)
        {
            bool isActive = !creditsPanel.activeSelf;
            creditsPanel.SetActive(isActive);

            // Bring the panel to the top of the z-order
            if (isActive)
            {
                creditsPanel.transform.SetAsLastSibling();
            }
        }
    }

    void Start()
    {
       creditsButton.onClick.AddListener(ToggleCreditsPanel);
       creditsButton.onClick.Invoke();
    }
}
