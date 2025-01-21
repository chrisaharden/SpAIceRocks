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


    [Header("Modal Backgrounds")]
    public GameObject modalBackground;
    public GameObject bottomMenuBackground;

    [Header("Main Screen")]
    public TMP_Text coinsEarnedText;
    public TMP_Text collectionGoalText;
    public TMP_Text movesRemainingText;
    public TMP_Text planetText;
    public Button exitButton;
    public Button buyItemsButton;
    public Button buyToolsButton; 
    public Button creditsButton;
    public Button buyPlanetsButton;
    public TMP_Text CharacterDialogText;


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
    public Button[] buyItemButtons;    // Type_05 to Type_10
    public TMP_Text[] coinValueTexts;  // CoinValue_05 to CoinValue_10

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

        //bottomMenuBackground.blocksRaycasts = true;
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

        // Update the planet info text
        CharacterDialogText.text = planetText.text + ": " + GameManager.Instance.planetConfigs[planetNumber].GetCurrentCharacterDialogue()+"Buy rights to mine "+ GameManager.board.tileConfigs[planetNumber+(int)TileConfig.TileType.Type_05].info +" while you are here.";

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
                buttonText.text = $"Buy: {config.purchasePrice} Coins";
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
                AudioManager.Instance.PlayBackgroundMusic();
            }
            else
            {
                ShowPanel(buyPlanetsPanel);
                AudioManager.Instance.PlayShopBackgroundMusic();

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
                
                // Update each buy item button (Type_05 to Type_10)
                for (int i = (int)TileConfig.TileType.Type_05; i <= (int)TileConfig.TileType.Type_10; i++)
                {
                    int arrayIndex = i - (int)TileConfig.TileType.Type_05; // Adjust for array indexing
                    UpdateBuyItemButton(buyItemButtons[arrayIndex], coinValueTexts[arrayIndex], configs[i], coins);
                }

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

        // Disable all item buttons by default
        if (buyItemButtons != null)
        {
            foreach (Button button in buyItemButtons)
            {
                if (button != null) button.interactable = false;
            }
        }

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

        // Set up buy item button listeners using array
        if (buyItemButtons != null)
        {
            for (int i = 0; i < buyItemButtons.Length; i++)
            {
                int tileIndex = i + 5; // Adjust for tile indexing (5-9)
                if (buyItemButtons[i] != null)
                {
                    buyItemButtons[i].onClick.AddListener(() => PurchaseTile(tileIndex));
                }
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

        // Set up tool button listeners
        if (ToolButtons != null)
        {
            for (int i = 0; i < ToolButtons.Length; i++)
            {
                int toolIndex = i; // Capture the index for the lambda
                if (ToolButtons[i] != null)
                {
                    ToolButtons[i].onClick.AddListener(() => PurchaseTool(toolIndex));
                }
            }
        }
    }
}
