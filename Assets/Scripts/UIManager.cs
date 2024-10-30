using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TMP_Text coinsEarnedText;
    public TMP_Text collectionGoalText;
    public TMP_Text movesRemainingText;
    public GameObject timesUpPanel;
    public GameObject boardClearedPanel;
    public GameObject creditsPanel;
    public GameObject exitConfirmPanel;
    public GameObject buyRocketPanel;
    public Button creditsButton;
    public Button timesUpButton;
    public Button boardClearedButton;
    public Button exitButton;
    public Button confirmExitButton;
    public Button cancelExitButton;
    public Button buyRocketButton;
    public Button confirmBuyRocketButton;
    public Button cancelBuyRocketButton;

    [Header("Robot Reward")]
    public Image robotRewardImage;
    public TMP_Text robotRewardText;

    void Awake()
    {
        Instance = this;
    }

    public void UpdateCoinsEarned(int coins)
    {
        coinsEarnedText.text = $"{coins}";
        UpdateBuyRocketButtonState(coins);
    }

    private void UpdateBuyRocketButtonState(int coins)
    {
        if (buyRocketButton != null)
        {
            int rocketCost = GameManager.Instance.rocketCost;
            buyRocketButton.interactable = coins >= rocketCost;
        }
    }

    public void UpdateMoves(int moves)
    {
        movesRemainingText.text = $"{moves}";
    }

    public void UpdateCollectionGoal(int goal)
    {
        collectionGoalText.text = $"{goal} Minerals";
    }

    public void ShowtimesUp()
    {
        timesUpPanel.SetActive(true);
    }

    public void CloseTimesUpPanel()
    {
        if (timesUpPanel != null)
        {
            timesUpPanel.SetActive(false);
        }
    }

    public void ShowBuyRocketPanel()
    {
        if (buyRocketPanel != null)
        {
            buyRocketPanel.SetActive(true);
            buyRocketPanel.transform.SetAsLastSibling();
        }
    }

    public void HideBuyRocketPanel()
    {
        if (buyRocketPanel != null)
        {
            buyRocketPanel.SetActive(false);
        }
    }

    public void ShowBoardCleared(GameObject robotReward = null)
    {
        if (boardClearedPanel != null)
        {
            bool isActive = !boardClearedPanel.activeSelf;
            boardClearedPanel.SetActive(isActive);

            // Show robot reward if provided
            if (isActive && robotReward != null && robotRewardImage != null)
            {
                // Get the sprite from the robot prefab's SpriteRenderer
                SpriteRenderer robotSprite = robotReward.GetComponent<SpriteRenderer>();
                if (robotSprite != null)
                {
                    robotRewardImage.sprite = robotSprite.sprite;
                    robotRewardImage.gameObject.SetActive(true);
                }
                
                if (robotRewardText != null)
                {
                    robotRewardText.text = "Cherry Bomb R17";
                    robotRewardText.gameObject.SetActive(true);
                }
            }

            // Bring the panel to the top of the z-order
            if (isActive)
            {
                boardClearedPanel.transform.SetAsLastSibling();
            }
        }
    }

    public void CloseBoardClearedPanel()
    {
        if (boardClearedPanel != null)
        {
            boardClearedPanel.SetActive(false);
        }
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

    public void ShowExitConfirmation()
    {
        if (exitConfirmPanel != null)
        {
            exitConfirmPanel.SetActive(true);
            exitConfirmPanel.transform.SetAsLastSibling();
        }
    }

    public void CloseExitConfirmation()
    {
        if (exitConfirmPanel != null)
        {
            exitConfirmPanel.SetActive(false);
        }
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    void Start()
    {
        // Hide all panels on launch
        if (timesUpPanel != null) timesUpPanel.SetActive(false);
        if (boardClearedPanel != null) boardClearedPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (exitConfirmPanel != null) exitConfirmPanel.SetActive(false);
        if (buyRocketPanel != null) buyRocketPanel.SetActive(false);

        // Set the state of the buy button
        UpdateBuyRocketButtonState(GameManager.Instance.coinsEarned);

        // Set up button listeners
        creditsButton.onClick.AddListener(ToggleCreditsPanel);
        if (timesUpButton != null)
        {
            timesUpButton.onClick.AddListener(CloseTimesUpPanel);
        }
        if (boardClearedButton != null)
        {
            boardClearedButton.onClick.AddListener(CloseBoardClearedPanel);
        }
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ShowExitConfirmation);
        }
        if (confirmExitButton != null)
        {
            confirmExitButton.onClick.AddListener(ExitGame);
        }
        if (cancelExitButton != null)
        {
            cancelExitButton.onClick.AddListener(CloseExitConfirmation);
        }
        if (buyRocketButton != null)
        {
            buyRocketButton.onClick.AddListener(() => GameManager.Instance.TryPurchaseRocket());
        }
        if (confirmBuyRocketButton != null)
        {
            confirmBuyRocketButton.onClick.AddListener(() => GameManager.Instance.ConfirmRocketPurchase());
        }
        if (cancelBuyRocketButton != null)
        {
            cancelBuyRocketButton.onClick.AddListener(HideBuyRocketPanel);
        }
    }
}
