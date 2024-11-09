using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Modal Background")]
    public GameObject modalBackground;

    [Header("Main Screen")]
    public TMP_Text coinsEarnedText;
    public TMP_Text collectionGoalText;
    public TMP_Text movesRemainingText;
    public TMP_Text planetText;
    public Button exitButton;
    public Button buyItemsButton;
    public Button buyToolsButton; 
    public Button rocketButton;
    public Button creditsButton;

    [Header("Out Of Moves Panel")]
    public GameObject OutOfMovesPanel;
    public Button OutOfMovesButton;

    [Header("Board Cleared Panel")]
    public GameObject boardClearedPanel;
    public Button boardClearedButton;
    public TMP_Text boardClearCreditsText;

    [Header("Credits Panel")]
    public GameObject creditsPanel;

    [Header("Rocket Panel")]
    public GameObject buyRocketPanel;
    public Button confirmBuyRocketButton;
    public TMP_Text rocketCostText;

    [Header("Items Panel")]
    public GameObject buyItemsPanel;
    public Button buyItem05Button;    // Type_05
    public TMP_Text CoinValue_05;
    public Button buyItem06Button;    // Type_06
    public TMP_Text CoinValue_06;
    public Button buyItem07Button;    // Type_07
    public TMP_Text CoinValue_07;
    public Button buyItem08Button;    // Type_08 
    public TMP_Text CoinValue_08;
    public Button buyItem09Button;    // Type_09
    public TMP_Text CoinValue_09;

    [Header("Tools Panel")]
    public GameObject buyToolsPanel; // Panel for Tool shop
    public Button[] ToolButtons; // Array of buttons to buy Tools (0: TOOL_COLUMN_CLEARER, 1: TOOL_ROW_CLEARER, 2: TOOL_PLUS_CLEARER)
    public TMP_Text[] ToolLabels; // Array of texts to show Tool prices

    [Header("Exit Panel")]
    public GameObject exitConfirmPanel;
    public Button confirmExitButton;
    public Button cancelExitButton;

    private System.Collections.Generic.Stack<GameObject> activeModalPanels = new System.Collections.Generic.Stack<GameObject>();

    void Awake()
    {
        Instance = this;
        
        if (modalBackground != null)
        {
            modalBackground.SetActive(false);
            
            Image modalImage = modalBackground.GetComponent<Image>();
            if (modalImage != null)
            {
                Color modalColor = modalImage.color;
                modalColor.a = 0.5f;
                modalImage.color = modalColor;
                modalImage.raycastTarget = true;
            }
        }
    }

    private void ShowPanel(GameObject panel)
    {
        HideAllPanels();

        if (panel != null)
        {
            if (activeModalPanels.Count == 0)
            {
                modalBackground?.SetActive(true);
            }

            panel.SetActive(true);
            
            if (modalBackground != null)
            {
                modalBackground.transform.SetAsLastSibling();
            }
            panel.transform.SetAsLastSibling();
            
            activeModalPanels.Push(panel);

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
                    
                    if (activeModalPanels.Count == 0)
                    {
                        modalBackground?.SetActive(false);
                    }
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
        if (confirmBuyRocketButton != null)
        {
            int rocketCost = GameManager.Instance.rocketCost;
            confirmBuyRocketButton.interactable = coins >= rocketCost;
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
    public void UpdatePlanet(int planetNumber)
    {
        planetText.text = $"{planetNumber}";
    }

    public void ShowOutOfMoves()
    {
        ShowPanel(OutOfMovesPanel);
    }

    public void CloseOutOfMovesPanel()
    {
        HidePanel(OutOfMovesPanel);
    }

    private void UpdateBuyItemButton(Button button, TMP_Text valueText, TileConfig config, int coins)
    {
        if (button != null)
        {
            // Only enable if config's PlanetNumber matches current PlanetNumber and other conditions are met
            button.interactable = config.isLocked && 
                                coins >= config.purchasePrice && 
                                config.PlanetNumber == GameManager.Instance.PlanetNumber;
            
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                if (config.PlanetNumber != GameManager.Instance.PlanetNumber)
                {
                    buttonText.text = $"Available on Planet {config.PlanetNumber}";
                }
                else
                {
                    buttonText.text = config.isLocked ? $"Unlock: {config.purchasePrice} Coins" : "Unlocked";
                }
            }
        }
        if (valueText != null)
        {
            valueText.text = $"Earns: {config.coinValue} Coins";
        }
    }

    public void UpdateToolButtons(Button button, TMP_Text valueText, TileConfig config, int coins)
    {
        if (button != null)
        {
            bool isUnlocked = !config.isLocked;
            button.interactable = !isUnlocked && coins >= config.purchasePrice;
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = !isUnlocked ? $"Buy: {config.purchasePrice} Coins" : "Unlocked";
            }
        }
        if (valueText != null)
        {
            valueText.text = config.info;
        }
    }

    public void PurchaseTile(int tileIndex)
    {
        TileConfig config = GameManager.board.tileConfigs[tileIndex];
        if (config.isLocked && 
            GameManager.Instance.coinsEarned >= config.purchasePrice && 
            config.PlanetNumber == GameManager.Instance.PlanetNumber)
        {
            GameManager.Instance.PlayCashRegisterSound();
            
            GameManager.Instance.coinsEarned -= config.purchasePrice;
            UpdateCoinsEarned(GameManager.Instance.coinsEarned);

            GameManager.board.UnlockTileType(tileIndex);

            ToggleItemsPanel();
        }
    }

    public void PurchaseTool(int toolIndex)
    {
        GameManager.board.UnlockTool(toolIndex);
        ToggleToolsPanel(); // Refresh the panel to update button states
    }

    public void ShowBoardCleared(GameObject ToolReward = null)
    {
        if (boardClearedPanel != null)
        {
            ShowPanel(boardClearedPanel);
            if (boardClearCreditsText != null)
            {
                boardClearCreditsText.text = $"Bonus: {GameManager.Instance.boardClearCredits} Coins!";
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

    public void ToggleRocketPanel()
    {
        if (buyRocketPanel != null)
        {
            if (buyRocketPanel.activeSelf)
            {
                HidePanel(buyRocketPanel);
            }
            else
            {
                ShowPanel(buyRocketPanel);
                if (rocketCostText != null)
                {
                    rocketCostText.text = $"Cost: {GameManager.Instance.rocketCost} Coins";
                }
            }
        }
    }

    public void ToggleToolsPanel()
    {
        if (buyToolsPanel != null)
        {
            if (buyToolsPanel.activeSelf)
            {
                HidePanel(buyToolsPanel);
            }
            else
            {
                ShowPanel(buyToolsPanel);

                // Update each tool button (TOOL_COLUMN_CLEARER, TOOL_ROW_CLEARER, TOOL_PLUS_CLEARER)
                for (int i = 0; i < ToolButtons.Length && i < 3; i++)
                {
                    TileConfig config = GameManager.board.toolConfigs[i];
                    int coins = GameManager.Instance.coinsEarned;
                    UpdateToolButtons(ToolButtons[i], ToolLabels[i], config, coins);
                }
            }
        }
    }

    public void ToggleItemsPanel()
    {
        if (buyItemsPanel != null)
        {
            if (buyItemsPanel.activeSelf)
            {
                HidePanel(buyItemsPanel);
            }
            else
            {
                ShowPanel(buyItemsPanel);
                            
                TileConfig[] configs = GameManager.board.tileConfigs;
                int coins = GameManager.Instance.coinsEarned;
                
                UpdateBuyItemButton(buyItem05Button, CoinValue_05, configs[5], coins);
                UpdateBuyItemButton(buyItem06Button, CoinValue_06, configs[6], coins);
                UpdateBuyItemButton(buyItem07Button, CoinValue_07, configs[7], coins);
                UpdateBuyItemButton(buyItem08Button, CoinValue_08, configs[8], coins);
                UpdateBuyItemButton(buyItem09Button, CoinValue_09, configs[9], coins);
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

    public void HideAllPanels()
    {
        if (OutOfMovesPanel != null) OutOfMovesPanel.SetActive(false);
        if (boardClearedPanel != null) boardClearedPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (exitConfirmPanel != null) exitConfirmPanel.SetActive(false);
        if (buyRocketPanel != null) buyRocketPanel.SetActive(false);
        if (buyItemsPanel != null) buyItemsPanel.SetActive(false);
        if (buyToolsPanel != null) buyToolsPanel.SetActive(false);
        if (modalBackground != null) modalBackground.SetActive(false);
    }

    void Start()
    {
        HideAllPanels();

        if (buyItem05Button != null) buyItem05Button.interactable = false;
        if (buyItem06Button != null) buyItem06Button.interactable = false;
        if (buyItem07Button != null) buyItem07Button.interactable = false;
        if (buyItem08Button != null) buyItem08Button.interactable = false;
        if (buyItem09Button != null) buyItem09Button.interactable = false;

        UpdateBuyRocketButtonState(GameManager.Instance.coinsEarned);
        UpdatePlanet(GameManager.Instance.PlanetNumber);

        creditsButton.onClick.AddListener(ToggleCreditsPanel);
        if (OutOfMovesButton != null)
        {
            OutOfMovesButton.onClick.AddListener(CloseOutOfMovesPanel);
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
            rocketButton.onClick.AddListener(ToggleRocketPanel);
        }
        if (confirmBuyRocketButton != null)
        {
            confirmBuyRocketButton.onClick.AddListener(() => GameManager.Instance.ConfirmRocketPurchase());
        }
        if (buyItemsButton != null)
        {
            buyItemsButton.onClick.AddListener(ToggleItemsPanel);
        }
        if (buyToolsButton != null)
        {
            buyToolsButton.onClick.AddListener(ToggleToolsPanel);
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

        // Set up Tool button listeners (TOOL_COLUMN_CLEARER, TOOL_ROW_CLEARER, TOOL_PLUS_CLEARER)
        for (int i = 0; i < ToolButtons.Length && i < 3; i++)
        {
            int toolIndex = i; // Capture the index for the lambda
            if (ToolButtons[i] != null)
            {
                ToolButtons[i].onClick.AddListener(() => PurchaseTool(toolIndex));
            }
        }
    }
}
