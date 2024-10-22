using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static Board board;
    public int score;
    public int level;
    int scoreGoal;
    public int scoreGoalIncrement;
    
    void Awake()
    {
        Instance = this;
        board = FindFirstObjectByType<Board>();
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
    }

    public void GameOver()
    {
        UIManager.Instance.ShowGameOver();
    }
}
