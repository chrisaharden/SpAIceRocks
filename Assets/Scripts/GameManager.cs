using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static Board board;
    public int score;
    public int level;
    int scoreGoal;
    public int scoreGoalIncrement;
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
        score = 0;
        scoreGoal = scoreGoalIncrement;
        board.GenerateBoard();
        SetRandomBackgroundColor();
        UpdateBackgroundMusic();
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

    public void AddScore(int amount)
    {
        score += amount;
        UIManager.Instance.UpdateScore(score);

        if (score >= scoreGoal)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        scoreGoal += scoreGoalIncrement;
        UIManager.Instance.UpdateLevel(level);
        SetRandomBackgroundColor();
        board.UpdateBoardSize(level);
        UpdateBackgroundMusic();
    }

    void SetRandomBackgroundColor()
    {
        if (mainCamera != null)
        {
            Color randomColor = new Color(
                Random.Range(0.1f, 0.3f),
                Random.Range(0.1f, 0.3f),
                Random.Range(0.1f, 0.3f)
            );
            mainCamera.backgroundColor = randomColor;
        }
    }

    public void GameOver()
    {
        UIManager.Instance.ShowGameOver();
    }
}
