using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] GameObject StartMenu;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject GameOverMenu;
    [SerializeField] GameObject GameWin;

    [Header("InGame")]
    [SerializeField] TMP_Text time;
    [SerializeField] TMP_Text score;
    [SerializeField] TMP_Text maxScore;

    private float elapsed = 0;

    private void Start()
    {
        time.text = elapsed.ToString();
        UpdateMaxScore(GameManager.maxScore);
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        time.text = ((int)elapsed).ToString();
    }

    public void Pause()
    {
        PauseMenu.SetActive(!PauseMenu.activeSelf);
    }

    public void StartGame()
    {
        StartMenu.SetActive(false);
    }

    public void GameOver(bool win = false)
    {
        if(!win)
            GameOverMenu.SetActive(true);
        else
            GameWin.SetActive(true);
    }

    public void UpdateScore(int currentScorePoints)
    {
        score.text = currentScorePoints.ToString();
    }

    private void UpdateMaxScore(int maxScorePoints)
    {
        maxScore.text = maxScorePoints.ToString();
    }

}
