using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Splash Screen")]
    public GameObject splashScreenPanel;
    public float splashScreenDuration = 3f;


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
    public Button buyPlanetsButton;

    [Header("Out Of Moves Panel")]
    public GameObject OutOfMovesPanel;
    public Button OutOfMovesButton;

    [Header("Board Cleared Panel")]
    public GameObject boardClearedPanel;
    public TMP_Text boardClearCreditsText;

    [Header("Credits Panel")]
    public GameObject creditsPanel;

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

    [Header("Planets Panel")]
    public GameObject buyPlanetsPanel;
    public Button[] buyPlanetButtons; // Array of buttons to buy planets
    public TMP_Text[] planetLabels; // Array of texts to show planet info

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
        PlanetConfig config = GameManager.Instance.planetConfigs[planetNumber];
        planetText.text = config.info;
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
                    string planetName = GameManager.Instance.planetConfigs[config.PlanetNumber].info;
                    buttonText.text = $"Available on {planetName}";
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

    public void UpdateBuyPlanetButton(Button button, TMP_Text label, PlanetConfig config, int coins, bool isNextSequentialPlanet)
    {
        if (button != null)
        {
            // Only enable the button if it's the next sequential planet to unlock
            button.interactable = config.isLocked && 
                                   coins >= config.purchasePrice && 
                                   isNextSequentialPlanet;
            
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = config.isLocked ? $"Buy: {config.purchasePrice} Coins" : "Unlocked";
            }
        }
        if (label != null)
        {
            label.text = config.info;
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

    public void PurchasePlanet(int planetIndex)
    {
        GameManager.Instance.PurchasePlanet(planetIndex);
        //TogglePlanetsPanel(); // Refresh the panel to update button states
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
            
            // Start coroutine to auto-hide the panel after 3 seconds
            StartCoroutine(AutoHideBoardClearedPanel());
        }
    }

    private IEnumerator AutoHideBoardClearedPanel()
    {
        yield return new WaitForSeconds(1.5f);
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

    public void TogglePlanetsPanel()
    {
        if (buyPlanetsPanel != null)
        {
            if (buyPlanetsPanel.activeSelf)
            {
                HidePanel(buyPlanetsPanel);
                GameManager.Instance.RestorePreviousBackgroundMusic();
            }
            else
            {
                ShowPanel(buyPlanetsPanel);
                GameManager.Instance.PlayShopBackgroundMusic();

                // Find the first locked planet to determine the next sequential planet
                int nextPlanetIndex = -1;
                for (int i = 0; i < GameManager.Instance.planetConfigs.Length; i++)
                {
                    if (GameManager.Instance.planetConfigs[i].isLocked)
                    {
                        nextPlanetIndex = i;
                        break;
                    }
                }

                // Update each planet button
                for (int i = 0; i < buyPlanetButtons.Length && i < GameManager.Instance.planetConfigs.Length; i++)
                {
                    PlanetConfig config = GameManager.Instance.planetConfigs[i];
                    int coins = GameManager.Instance.coinsEarned;
                    
                    // Only the next sequential planet can be purchased
                    bool isNextSequentialPlanet = (i == nextPlanetIndex);
                    
                    UpdateBuyPlanetButton(buyPlanetButtons[i], planetLabels[i], config, coins, isNextSequentialPlanet);
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
        if (splashScreenPanel != null) splashScreenPanel.SetActive(false);
        if (OutOfMovesPanel != null) OutOfMovesPanel.SetActive(false);
        if (boardClearedPanel != null) boardClearedPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (exitConfirmPanel != null) exitConfirmPanel.SetActive(false);
        if (buyItemsPanel != null) buyItemsPanel.SetActive(false);
        if (buyToolsPanel != null) buyToolsPanel.SetActive(false);
        if (buyPlanetsPanel != null) buyPlanetsPanel.SetActive(false);
        if (modalBackground != null) modalBackground.SetActive(false);
    }

    void Start()
    {
        // Show splash screen at the start of the game
        if (splashScreenPanel != null)
        {
            StartCoroutine(ShowSplashScreen());
        }
        else
        {
            InitializeUI();
        }
    }

    private IEnumerator ShowSplashScreen()
    {
        // Activate splash screen
        splashScreenPanel.SetActive(true);

        // Wait for specified duration
        yield return new WaitForSeconds(splashScreenDuration);

        // Deactivate splash screen
        splashScreenPanel.SetActive(false);

        // Initialize rest of the UI
        InitializeUI();
    }

    private void InitializeUI()
    {
        HideAllPanels();

        if (buyItem05Button != null) buyItem05Button.interactable = false;
        if (buyItem06Button != null) buyItem06Button.interactable = false;
        if (buyItem07Button != null) buyItem07Button.interactable = false;
        if (buyItem08Button != null) buyItem08Button.interactable = false;
        if (buyItem09Button != null) buyItem09Button.interactable = false;

        // Set all planet buttons to not interactable by default
        if (buyPlanetButtons != null)
        {
            foreach (Button button in buyPlanetButtons)
            {
                if (button != null) button.interactable = false;
            }
        }


        UpdatePlanet(GameManager.Instance.PlanetNumber);

        creditsButton.onClick.AddListener(ToggleCreditsPanel);
        if (OutOfMovesButton != null)
        {
            OutOfMovesButton.onClick.AddListener(CloseOutOfMovesPanel);
        }
        // Removed boardClearedButton listener
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
        if (buyItemsButton != null)
        {
            buyItemsButton.onClick.AddListener(ToggleItemsPanel);
        }
        if (buyToolsButton != null)
        {
            buyToolsButton.onClick.AddListener(ToggleToolsPanel);
        }
        if (buyPlanetsButton != null)
        {
            buyPlanetsButton.onClick.AddListener(TogglePlanetsPanel);
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

        // Set up planet button listeners
        if (buyPlanetButtons != null)
        {
            for (int i = 0; i < buyPlanetButtons.Length; i++)
            {
                int planetIndex = i; // Capture the index for the lambda
                if (buyPlanetButtons[i] != null)
                {
                    buyPlanetButtons[i].onClick.AddListener(() => PurchasePlanet(planetIndex));
                }
            }
        }
    }
}
