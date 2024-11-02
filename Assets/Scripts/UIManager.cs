using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Main Screen")]
    public TMP_Text coinsEarnedText;
    public TMP_Text collectionGoalText;
    public TMP_Text movesRemainingText;
    public Button exitButton;
    public Button buyItemsButton;
    public Button rocketButton;
    public Button creditsButton;


    [Header("Times Up Panel")]
    public GameObject timesUpPanel;
    public Button timesUpButton;


    [Header("Board Cleared Panel")]
    public GameObject boardClearedPanel;
    public Button boardClearedButton;


    [Header("Credits Panel")]
    public GameObject creditsPanel;


    [Header("Rocket Panel")]
    public GameObject buyRocketPanel;
    public Button confirmBuyRocketButton;
    public Button cancelBuyRocketButton;


    [Header("Items Panel")]
    public GameObject buyItemsPanel;
    public Button buyItem05Button;    // Type_05
    public Button buyItem06Button;    // Type_06
    public Button buyItem07Button;    // Type_07
    public Button buyItem08Button;    // Type_08
    public Button buyItem09Button;    // Type_09
    public Button cancelBuyItemButton;


    [Header("Robot Reward")]
    public Image robotRewardImage;
    public TMP_Text robotRewardText;


    [Header("Exit Panel")]
    public GameObject exitConfirmPanel;
    public Button confirmExitButton;
    public Button cancelExitButton;
   

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
        if (rocketButton != null)
        {
            int rocketCost = GameManager.Instance.rocketCost;
            rocketButton.interactable = coins >= rocketCost;
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

    private void UpdateBuyItemButton(Button button, TileConfig config, int coins)
    {
        if (button != null)
        {
            button.interactable = config.isLocked && coins >= config.purchasePrice;
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = config.isLocked ? $"{config.purchasePrice} Coins" : "Unlocked";
            }
        }
    }

    public void ShowBuyItemsPanel()
    {
        if (buyItemsPanel != null)
        {
            buyItemsPanel.SetActive(true);
            buyItemsPanel.transform.SetAsLastSibling();

            // Update button states based on tile configurations
            TileConfig[] configs = GameManager.board.tileConfigs;
            int coins = GameManager.Instance.coinsEarned;
            
            UpdateBuyItemButton(buyItem05Button, configs[5], coins);
            UpdateBuyItemButton(buyItem06Button, configs[6], coins);
            UpdateBuyItemButton(buyItem07Button, configs[7], coins);
            UpdateBuyItemButton(buyItem08Button, configs[8], coins);
            UpdateBuyItemButton(buyItem09Button, configs[9], coins);
        }
    }

    public void HideBuyItemsPanel()
    {
        if (buyItemsPanel != null)
        {
            buyItemsPanel.SetActive(false);
        }
    }

    public void PurchaseTile(int tileIndex)
    {
        TileConfig config = GameManager.board.tileConfigs[tileIndex];
        if (config.isLocked && GameManager.Instance.coinsEarned >= config.purchasePrice)
        {
            // Deduct coins
            GameManager.Instance.coinsEarned -= config.purchasePrice;
            UpdateCoinsEarned(GameManager.Instance.coinsEarned);

            // Unlock the tile type
            GameManager.board.UnlockTileType(tileIndex);

            // Hide the panel
            HideBuyItemsPanel();
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
        if (buyItemsPanel != null) buyItemsPanel.SetActive(false);

        // Disable buy item buttons by default
        if (buyItem05Button != null) buyItem05Button.interactable = false;
        if (buyItem06Button != null) buyItem06Button.interactable = false;
        if (buyItem07Button != null) buyItem07Button.interactable = false;
        if (buyItem08Button != null) buyItem08Button.interactable = false;
        if (buyItem09Button != null) buyItem09Button.interactable = false;

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
        if (rocketButton != null)
        {
            rocketButton.onClick.AddListener(ShowBuyRocketPanel);
        }
        if (confirmBuyRocketButton != null)
        {
            confirmBuyRocketButton.onClick.AddListener(() => GameManager.Instance.ConfirmRocketPurchase());
        }
        if (cancelBuyRocketButton != null)
        {
            cancelBuyRocketButton.onClick.AddListener(HideBuyRocketPanel);
        }
        if (buyItemsButton != null)
        {
            buyItemsButton.onClick.AddListener(ShowBuyItemsPanel);
        }
        if (cancelBuyItemButton != null)
        {
            cancelBuyItemButton.onClick.AddListener(HideBuyItemsPanel);
        }

        // Set up buy item button listeners
        if (buyItem05Button != null)
            buyItem05Button.onClick.AddListener(() => PurchaseTile(5));
        if (buyItem06Button != null)
            buyItem06Button.onClick.AddListener(() => PurchaseTile(6));
        if (buyItem07Button != null)
            buyItem07Button.onClick.AddListener(() => PurchaseTile(7));
        if (buyItem08Button != null)
            buyItem08Button.onClick.AddListener(() => PurchaseTile(8));
        if (buyItem09Button != null)
            buyItem09Button.onClick.AddListener(() => PurchaseTile(9));
    }
}
