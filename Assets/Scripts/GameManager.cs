using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlanetConfig
{
    public bool isLocked;
    public int purchasePrice;
    public string info = "";
    public int planetNumber;
    public float ShipPosX; 
    public bool ShipFlyOrJump; // True for fly, false for jump 
}

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
    public int PlanetNumber = 1; 
    
    [Header("Audio")]
    private AudioSource backgroundAudio;
    private AudioSource sfxAudio; // New audio source for sound effects
    public AudioClip[] backgroundMusics; // Array of background music tracks
    public AudioClip shopBackgroundMusic; // Background music for the shop panel
    public AudioClip cashRegisterSound; 
    public AudioClip boardClearedSound;
    private int currentMusicIndex = 0;
    private int previousMusicIndex = 0; // Store the music index before entering shop

    [Header("Backgrounds and Characters")]
    public Sprite[] planetaryBackgrounds;
    public Sprite[] characterSprites;
    private int currentBackgroundIndex = 0;
    private int currentCharacterIndex = 0;
    public SpriteRenderer backgroundRenderer;
    public SpriteRenderer characterRenderer;
    
    [Header("Economy")]
    public int rocketCost = 10000;
    public int boardClearCredits = 1000;

    [Header("Planets")]
    public PlanetConfig[] planetConfigs;
    public ShipMovement shipMovement;

    void Awake()
    {
        Instance = this;
        board = FindFirstObjectByType<Board>();
        mainCamera = Camera.main;
        
        // Setup background music audio source
        backgroundAudio = gameObject.AddComponent<AudioSource>();
        backgroundAudio.loop = true;
        backgroundAudio.volume = 0.015f;
        
        // Setup SFX audio source
        sfxAudio = gameObject.AddComponent<AudioSource>();
        sfxAudio.loop = false;
        sfxAudio.volume = 1f;
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
        PlayBackgroundMusic();
        
        // Initialize all UI text fields
        UIManager.Instance.UpdateCollectionGoal(itemsLeftToCollect);
        UIManager.Instance.UpdateCoinsEarned(coinsEarned);
        UIManager.Instance.UpdateMoves(board.movesRemaining);

        // Set initial background and character
        if (backgroundRenderer != null && planetaryBackgrounds.Length > 0)
        {
            backgroundRenderer.sprite = planetaryBackgrounds[currentBackgroundIndex];
        }
        if (characterRenderer != null && characterSprites.Length > 0)
        {
            characterRenderer.sprite = characterSprites[currentCharacterIndex];
        }
    }

    public void PlayCashRegisterSound()
    {
        if (sfxAudio != null && cashRegisterSound != null)
        {
            sfxAudio.PlayOneShot(cashRegisterSound);
        }
    }

    public void GoToNextPlanet()
    {
        // Update background
        currentBackgroundIndex = (currentBackgroundIndex + 1) % planetaryBackgrounds.Length;
        if (backgroundRenderer != null)
        {
            backgroundRenderer.sprite = planetaryBackgrounds[currentBackgroundIndex];
        }

        // Update character
        currentCharacterIndex = (currentCharacterIndex + 1) % characterSprites.Length;
        if (characterRenderer != null)
        {
            characterRenderer.sprite = characterSprites[currentCharacterIndex];
        }

        // Update music track index.  New music will play  when planet dialog is closed
        if (backgroundMusics != null && backgroundMusics.Length > 0)
        {
            currentMusicIndex = (currentMusicIndex + 1) % backgroundMusics.Length;
            //PlayBackgroundMusic();
        }

        // Update PlanetNumber when moving to a new planet
        PlanetNumber = (PlanetNumber+1) % planetConfigs.Length;
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
                PlayCashRegisterSound();
                coinsEarned -= config.purchasePrice;
                config.isLocked = false;
                UIManager.Instance.UpdateCoinsEarned(coinsEarned);
                //UIManager.Instance.TogglePlanetsPanel();

                // Animate the ship with the planet's specific position
                shipMovement.MoveShipToX(config.ShipPosX, config.ShipFlyOrJump); 
                
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

                // If all planets were unlocked, lock the first planet and make it available for sale
                if (allPlanetsUnlocked && planetConfigs.Length > 0)
                {
                    planetConfigs[0].isLocked = true;
                }
            }
        }
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusics != null && backgroundMusics.Length > 0)
        {
            AudioClip nextTrack = backgroundMusics[currentMusicIndex];
            if (backgroundAudio.clip != nextTrack)
            {
                backgroundAudio.clip = nextTrack;
                backgroundAudio.Play();
            }
        }
    }

    public void PlayShopBackgroundMusic()
    {
        if (shopBackgroundMusic != null && backgroundAudio != null)
        {
            previousMusicIndex = currentMusicIndex;
            backgroundAudio.clip = shopBackgroundMusic;
            backgroundAudio.Play();
        }
    }

    public void RestorePreviousBackgroundMusic()
    {
        currentMusicIndex = previousMusicIndex;
        PlayBackgroundMusic();
    }

    public void AddItemsCollected(HashSet<Tile> matchedTiles)
    {
        //collect coins for all items collected
        int totalCoins = 0;
        foreach(Tile tile in matchedTiles)
        {
            totalCoins += tile.coinValue;
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

    void LevelWon()
    {
        level++;
        itemsLeftToCollect = intialCollectionGoal;
        
        // Award board clear credits
        coinsEarned += boardClearCredits;
        UIManager.Instance.UpdateCoinsEarned(coinsEarned);
        
        UIManager.Instance.ShowBoardCleared();
        
        // Play board cleared sound
        PlayBoardClearedSound();
        
        board.UpdateBoardSize(level);

        //reset for the next try
        itemsLeftToCollect = intialCollectionGoal;
        board.movesRemaining = 20;
        UIManager.Instance.UpdateCollectionGoal(itemsLeftToCollect);
        UIManager.Instance.UpdateMoves(board.movesRemaining);        
    }

    void PlayBoardClearedSound()
    {
        if (sfxAudio != null && boardClearedSound != null)
        {
            sfxAudio.PlayOneShot(boardClearedSound);
        }
    }

    public void GameOver()
    {
        UIManager.Instance.ShowOutOfMoves();
        
        //reset for the next try
        itemsLeftToCollect = intialCollectionGoal;
        board.movesRemaining = 20;    
        board.ClearBoard();
        board.GenerateBoard();
        UIManager.Instance.UpdateCollectionGoal(itemsLeftToCollect);
        UIManager.Instance.UpdateMoves(board.movesRemaining);   
    }
}
