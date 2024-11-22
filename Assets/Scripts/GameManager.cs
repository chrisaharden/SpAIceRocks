using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static Board board;

    public int itemsLeftToCollect;
    public int coinsEarned;
    public int intialCollectionGoal = 50;
    int collectionGoal;
    public int level;
    private Camera mainCamera;
    public int PlanetNumber = -1; //Starting at -1, since we use GoToNextPlanet() which will increment it to 0. 
    
    
    [Header("Planet Configs")]
    public PlanetConfig[] planetConfigs;
    //public Sprite[] planetaryBackgrounds;
    //public Sprite[] characterSprites;
    public SpriteRenderer backgroundRenderer;
    public SpriteRenderer characterRenderer;
    
    [Header("Economy")]
    public int rocketCost = 10000;
    public int boardClearCredits = 1000;

    [Header("Ship")]
    public ShipMovement shipMovement;

    void Awake()
    {
        Instance = this;
        board = FindFirstObjectByType<Board>();
        mainCamera = Camera.main;
    }

    void Start()
    {
        NewGame();
    }

    void NewGame()
    {
        level = 1;
        itemsLeftToCollect = intialCollectionGoal;
        coinsEarned = 0;
        board.GenerateBoard();
        AudioManager.Instance.PlayBackgroundMusic();
        
        // Initialize all UI text fields
        UIManager.Instance.UpdateCollectionGoal(itemsLeftToCollect);
        UIManager.Instance.UpdateCoinsEarned(coinsEarned);
        UIManager.Instance.UpdateMoves(board.movesRemaining);

        //Starting at -1, since we use GoToNextPlanet() which will increment it to 0. 
        PlanetNumber = -1;
        GoToNextPlanet();
    }

    public void PlayCashRegisterSound()
    {
        AudioManager.Instance.PlayCashRegisterSound();
    }

    public void GoToNextPlanet()
    {

        //Short term bug fix; I need to move the audio index to also be the planet index, but for now I'm setting it to match the planet index
        AudioManager.Instance.currentMusicIndex = PlanetNumber;

        //Update planet index before moving ahead
        PlanetNumber = (PlanetNumber+1) % planetConfigs.Length;

        // Update background & character
        if (backgroundRenderer != null && characterRenderer.sprite != null && PlanetNumber < planetConfigs.Length && planetConfigs.Length > 0)
        {
            backgroundRenderer.sprite = planetConfigs[PlanetNumber].BackgroundImage;
            characterRenderer.sprite = planetConfigs[PlanetNumber].CharacterSprite;
        }
        else
        {
            Debug.LogWarning("Background renderer, character renderer, or planetConfigs not set in GameManager, or planet index is wrong");
        }

        // Update music track index. New music will play when planet dialog is closed
        AudioManager.Instance.UpdateMusicIndex((AudioManager.Instance.currentMusicIndex + 1) % AudioManager.Instance.backgroundMusics.Length);

        // Reset the board to minimum columns
        board.UpdateBoardSize(level = 1);

        // Update PlanetNumber when moving to a new planet
        UIManager.Instance.UpdatePlanet(PlanetNumber);
    }

    public void PurchasePlanet(int planetIndex)
    {
        if (planetIndex >= 0 && planetIndex < planetConfigs.Length)
        {
            // Check if all previous planets are unlocked before allowing purchase
            bool canPurchase = true;
            for (int i = 0; i < planetIndex; i++)
            {
                if (planetConfigs[i].isLocked)
                {
                    canPurchase = false;
                    break;
                }
            }

            PlanetConfig config = planetConfigs[planetIndex];
            if (canPurchase && config.isLocked && coinsEarned >= config.purchasePrice)
            {
                AudioManager.Instance.PlayCashRegisterSound();
                coinsEarned -= config.purchasePrice;
                config.isLocked = false;
                UIManager.Instance.UpdateCoinsEarned(coinsEarned);

                // Animate the ship with the planet's specific position
                shipMovement.MoveShipToX(config.ShipPosX, config.ShipFlyOrJump); 
                
                // Reset the level back to first again
                level = 1;
                GoToNextPlanet();

                // Check if this was the last planet being unlocked
                bool allPlanetsUnlocked = true;
                for (int i = 0; i < planetConfigs.Length; i++)
                {
                    if (planetConfigs[i].isLocked)
                    {
                        allPlanetsUnlocked = false;
                        break;
                    }
                }

                // If all planets were unlocked, lock all planets again
                if (allPlanetsUnlocked && planetConfigs.Length > 0)
                {
                    for (int i = 0; i < planetConfigs.Length; i++)
                    {
                        planetConfigs[i].isLocked = true;
                    }
                }
            }
        }
    }

    public void AddItemsCollected(HashSet<Tile> matchedTiles)
    {
        //collect coins for all items collected
        int totalCoins = 0;
        foreach(Tile tile in matchedTiles)
        {
            totalCoins += tile.config.coinValue;
        }
        coinsEarned += totalCoins;
        UIManager.Instance.UpdateCoinsEarned(coinsEarned);

        itemsLeftToCollect -= matchedTiles.Count;
        if (itemsLeftToCollect <= 0) 
        {
            itemsLeftToCollect = 0;
            LevelWon();
        }
        UIManager.Instance.UpdateCollectionGoal(itemsLeftToCollect);
    }

    public void LevelWon()
    {
        level++;
        itemsLeftToCollect = intialCollectionGoal;
        
        // Award board clear credits
        coinsEarned += boardClearCredits;
        UIManager.Instance.UpdateCoinsEarned(coinsEarned);
        
        UIManager.Instance.ShowBoardCleared();
        
        // Play board cleared sound
        AudioManager.Instance.PlayBoardClearedSound();
        
        board.UpdateBoardSize(level);

        //reset for the next try
        itemsLeftToCollect = intialCollectionGoal;
        board.movesRemaining = 20;
        UIManager.Instance.UpdateCollectionGoal(itemsLeftToCollect);
        UIManager.Instance.UpdateMoves(board.movesRemaining);        
    }

    public void OutOfMoves()
    {
        UIManager.Instance.ShowOutOfMoves();
        
        //reset for the next try
        itemsLeftToCollect = intialCollectionGoal;
        board.movesRemaining = 20;    
        level = 1;
        board.UpdateBoardSize(level);
        UIManager.Instance.UpdateCollectionGoal(itemsLeftToCollect);
        UIManager.Instance.UpdateMoves(board.movesRemaining);   
    }
}