using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Modal Background")]
    public GameObject modalBackground; // Add this in Unity - full screen transparent black image

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

    private System.Collections.Generic.Stack<GameObject> activeModalPanels = new System.Collections.Generic.Stack<GameObject>();

    void Awake()
    {
        Instance = this;
        
        // Ensure modal background is ready
        if (modalBackground != null)
        {
            modalBackground.SetActive(false);
            
            // Set up modal background to block raycasts but be invisible
            Image modalImage = modalBackground.GetComponent<Image>();
            if (modalImage != null)
            {
                Color modalColor = modalImage.color;
                modalColor.a = 0.5f; // Semi-transparent black
                modalImage.color = modalColor;
                modalImage.raycastTarget = true;
            }
        }
    }

    private void ShowPanel(GameObject panel)
    {
        if (panel != null)
        {
            // Show modal background if this is the first panel
            if (activeModalPanels.Count == 0)
            {
                modalBackground?.SetActive(true);
            }

            panel.SetActive(true);
            
            // Ensure proper layering
            if (modalBackground != null)
            {
                modalBackground.transform.SetAsLastSibling();
            }
            panel.transform.SetAsLastSibling();
            
            activeModalPanels.Push(panel);

            // Ensure the panel blocks raycasts
            CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = panel.AddComponent<CanvasGroup>();
            }
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }
    }

    private void HidePanel(GameObject panel)
    {
        if (panel != null && panel.activeSelf)
        {
            panel.SetActive(false);
            
            if (activeModalPanels.Count > 0)
            {
                GameObject topPanel = activeModalPanels.Peek();
                if (topPanel == panel)
                {
                    activeModalPanels.Pop();
                    
                    // Hide modal background if no panels are active
                    if (activeModalPanels.Count == 0)
                    {
                        modalBackground?.SetActive(false);
                    }
                    // Otherwise, ensure the new top panel is properly layered
                    else if (activeModalPanels.Count > 0)
                    {
                        GameObject newTopPanel = activeModalPanels.Peek();
                        if (modalBackground != null)
                        {
                            modalBackground.transform.SetAsLastSibling();
                        }
                        newTopPanel.transform.SetAsLastSibling();
                    }
                }
            }
        }
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
        ShowPanel(timesUpPanel);
    }

    public void CloseTimesUpPanel()
    {
        HidePanel(timesUpPanel);
    }

    public void ShowBuyRocketPanel()
    {
        ShowPanel(buyRocketPanel);
    }

    public void HideBuyRocketPanel()
    {
        HidePanel(buyRocketPanel);
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
            ShowPanel(buyItemsPanel);

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
        HidePanel(buyItemsPanel);
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
            ShowPanel(boardClearedPanel);

            // Show robot reward if provided
            if (robotReward != null && robotRewardImage != null)
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
        }
    }

    public void CloseBoardClearedPanel()
    {
        HidePanel(boardClearedPanel);
    }

    public void ToggleCreditsPanel()
    {
        if (creditsPanel != null)
        {
            if (creditsPanel.activeSelf)
            {
                HidePanel(creditsPanel);
            }
            else
            {
                ShowPanel(creditsPanel);
            }
        }
    }

    public void ShowExitConfirmation()
    {
        ShowPanel(exitConfirmPanel);
    }

    public void CloseExitConfirmation()
    {
        HidePanel(exitConfirmPanel);
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
        if (modalBackground != null) modalBackground.SetActive(false);

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
