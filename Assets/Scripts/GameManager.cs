using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] UIManager UIM;

    public static GameStatus gameStatus;
    public static int maxScore = 0;
    private int currentScore = 0;

    private GameStatus oldGameStatus;

    public enum GameStatus
    {
        GameStart,
        GameRunning,
        GamePaused,
        Spawning,
        GameEnd
    }

    private void Update()
    {

        Cursor.visible = !(gameStatus == GameStatus.GameRunning || gameStatus == GameStatus.Spawning);

        if (Input.GetKeyDown(KeyCode.Escape) && gameStatus != GameStatus.GameEnd)
        {
            Pause();
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            switch (gameStatus)
            {
                case GameStatus.GameStart:
                    StartGame();
                    break;
                case GameStatus.GameEnd:
                    Restart();
                    break;
                default:
                    break;
            }
        }
    }

    public void Pause()
    {
        if(gameStatus == GameStatus.GamePaused)
        {
            gameStatus = oldGameStatus;
            Time.timeScale = 1;
        }
        else
        {
            oldGameStatus = gameStatus;
            gameStatus = GameStatus.GamePaused;
            Time.timeScale = 0;
        }

        UIM.Pause();
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        gameStatus = GameStatus.Spawning;
        UIM.StartGame();
    }

    private void Awake()
    {
        Time.timeScale = 0;
        gameStatus = GameStatus.GameStart;
        LevelController.OnLevelSpawning += SpawnEvent;
        Enemy.OnEnemyDeath += UpdateScore;
        Enemy.OnPlayerDeath += GameOver;
        Bomb.OnPlayerHit += GameOver;
        LevelManager.OnGameWin += GameWin;
    }

    private void OnDestroy()
    {
        LevelController.OnLevelSpawning -= SpawnEvent;
        Enemy.OnEnemyDeath -= UpdateScore;
        Enemy.OnPlayerDeath -= GameOver;
        Bomb.OnPlayerHit -= GameOver;
        LevelManager.OnGameWin -= GameWin;
    }

    private void UpdateScore(bool isSimple, Enemy enemy)
    {
        currentScore += enemy.pointValue;
        UIM.UpdateScore(currentScore);
    }

    private void CheckMaxScore()
    {
        if(currentScore > maxScore)
        {
            maxScore = currentScore;
        }
    }

    void SpawnEvent(bool isSpawning)
    {
        ChangeStatus(isSpawning ? GameStatus.Spawning : GameStatus.GameRunning);
    }

    void GameWin(bool win)
    {
        gameStatus = GameStatus.GameEnd;
        Time.timeScale = 0;
        UIM.GameOver(win);
    }

    void GameOver()
    {
        gameStatus = GameStatus.GameEnd;
        Time.timeScale = 0;
        UIM.GameOver();
    }

    void ChangeStatus(GameStatus targetStatus)
    {
        gameStatus = targetStatus;
    }

    public void Restart()
    {
        CheckMaxScore();
        gameStatus = GameStatus.GameStart;
        SceneManager.LoadScene("SampleScene");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
