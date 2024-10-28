using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static Board board;
    public int itemsLeftToCollect;
    public int coinsEarned;
    public int intialCollectionGoal = 200;
    int collectionGoal;
    public int level;
    private Camera mainCamera;
    
    [Header("Audio")]
    private AudioSource backgroundAudio;
    public AudioClip evenLevelMusic;
    public AudioClip oddLevelMusic;

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
        board.GenerateBoard();
        UpdateBackgroundMusic();
        
        // Initialize all UI text fields
        UIManager.Instance.UpdateCollectionGoal(itemsLeftToCollect);
        UIManager.Instance.UpdateCoinsEarned(coinsEarned);
        UIManager.Instance.UpdateMoves(board.movesRemaining);
    }

    void UpdateBackgroundMusic()
    {
        AudioClip newTrack = (level % 2 == 0) ? evenLevelMusic : oddLevelMusic;
        if (backgroundAudio.clip != newTrack)
        {
            backgroundAudio.clip = newTrack;
            backgroundAudio.Play();
        }
    }

    public void AddItemsCollected(int amount)
    {
        

        //collect coins for all items collected
        coinsEarned += amount;
        UIManager.Instance.UpdateCoinsEarned(coinsEarned);

        //check collected items for matches, then decrement if match.  When items left reaches zero, the level has been beaten
        //TODO: update the logic to look for specific items, such as certain stones
        itemsLeftToCollect-=amount;
        if (itemsLeftToCollect <= 0) itemsLeftToCollect=0;
        UIManager.Instance.UpdateCollectionGoal(itemsLeftToCollect);
        if (itemsLeftToCollect == 0)
        {
            LevelWon();
        }
    }

    void LevelWon()
    {
        level++;
        itemsLeftToCollect = intialCollectionGoal;
        UIManager.Instance.ShowBoardCleared();
        board.UpdateBoardSize(level);
        UpdateBackgroundMusic();

        //reset for the next try
        itemsLeftToCollect = intialCollectionGoal;
        board.movesRemaining = 20;
        UIManager.Instance.UpdateCollectionGoal(itemsLeftToCollect);
        UIManager.Instance.UpdateMoves(board.movesRemaining);        
    }

    public void GameOver()
    {
        UIManager.Instance.ShowtimesUp();
        
        //reset for the next try
        itemsLeftToCollect = intialCollectionGoal;
        board.movesRemaining = 20;    
        board.ClearBoard();
        board.GenerateBoard();
        UIManager.Instance.UpdateCollectionGoal(itemsLeftToCollect);
        UIManager.Instance.UpdateMoves(board.movesRemaining);   
    }
}
