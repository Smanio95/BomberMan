using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] LevelController[] levels;
    [SerializeField] Transform floor;
    [SerializeField] float ariseSpeed = 5;
    [SerializeField] GameObject player;

    private int currentLevel = 0;

    public delegate void LevelUpdate(float height);
    public static LevelUpdate OnLevelUpdate;

    public delegate void GameWin(bool win);
    public static GameWin OnGameWin;

    private void Awake()
    {
        EnemyManager.OnEnemiesKilled += UpdateLevel;
    }

    void UpdateLevel()
    {
        if (currentLevel == levels.Length - 1)
        {
            OnGameWin?.Invoke(true);
            return;
        }

        if (!CheckLevelsExistence()) return;

        OnLevelUpdate?.Invoke(levels[currentLevel + 1].transform.position.y);
        StartCoroutine(AriseLevel());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        EnemyManager.OnEnemiesKilled -= UpdateLevel;
    }

    private IEnumerator AriseLevel()
    {
        levels[currentLevel].enabled = false;
        currentLevel++;

        while(Vector3.Distance(floor.position, levels[currentLevel].transform.position) > 0)
        {
            floor.position = Vector3.MoveTowards(floor.position, levels[currentLevel].transform.position, Time.deltaTime * ariseSpeed);
            yield return null;
        }

        levels[currentLevel].gameObject.SetActive(true);
        levels[currentLevel].enabled = true;

        player.SetActive(true);
    }

    bool CheckLevelsExistence()
    {
        foreach(LevelController level in levels)
        {
            if (level == null) return false;
        }
        return true;
    }

}
