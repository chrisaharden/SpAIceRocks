using UnityEngine;
using System.Collections.Generic;

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
    
    [Header("Audio")]
    private AudioSource backgroundAudio;
    public AudioClip[] backgroundMusics; // Array of background music tracks
    private int currentMusicIndex = 0;

    [Header("Robot Collection")]
    public int robotsCollected = 0;
    public GameObject[] robotPrefabs; // Array of different robot prefabs to award

    [Header("Backgrounds and Characters")]
    public Sprite[] planetaryBackgrounds;
    public Sprite[] characterSprites;
    private int currentBackgroundIndex = 0;
    private int currentCharacterIndex = 0;
    public SpriteRenderer backgroundRenderer;
    public SpriteRenderer characterRenderer;
    public int rocketCost = 10000;

    void Awake()
    {
        Instance = this;
        board = FindFirstObjectByType<Board>();
        mainCamera = Camera.main;
        backgroundAudio = gameObject.AddComponent<AudioSource>();
        backgroundAudio.loop = true;
        backgroundAudio.volume = 0.015f;
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

    public void TryPurchaseRocket()
    {
        if (coinsEarned >= rocketCost)
        {
            UIManager.Instance.ShowBuyRocketPanel();
        }
        else
        {
            Debug.Log("Not enough coins to purchase rocket");
        }
    }

    public void ConfirmRocketPurchase()
    {
        if (coinsEarned >= rocketCost)
        {
            coinsEarned -= rocketCost;
            UIManager.Instance.UpdateCoinsEarned(coinsEarned);
            
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

            // Update music track
            if (backgroundMusics != null && backgroundMusics.Length > 0)
            {
                currentMusicIndex = (currentMusicIndex + 1) % backgroundMusics.Length;
                PlayBackgroundMusic();
            }

            UIManager.Instance.HideBuyRocketPanel();
        }
    }

    void PlayBackgroundMusic()
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
        
        // Award a new robot
        if (robotPrefabs != null && robotPrefabs.Length > 0)
        {
            int robotIndex = robotsCollected % robotPrefabs.Length;
            robotsCollected++;
            UIManager.Instance.ShowBoardCleared(robotPrefabs[robotIndex]);
        }
        else
        {
            UIManager.Instance.ShowBoardCleared(null);
        }
        
        board.UpdateBoardSize(level);

        //reset for the next try
        itemsLeftToCollect = intialCollectionGoal;
        board.movesRemaining = 20;
        UIManager.Instance.UpdateCollectionGoal(itemsLeftToCollect);
        UIManager.Instance.UpdateMoves(board.movesRemaining);        
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
