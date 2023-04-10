using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] Transform enemyParent;
    [SerializeField] Enemy simpleEnemy;
    [SerializeField] Enemy fastEnemy;

    private Queue<Enemy> simpleEnemyQueue = new();
    private Queue<Enemy> fastEnemyQueue = new();
    private bool[,] levelMap;

    private int spawnedEnemies = 0;

    public delegate void EnemiesKilled();
    public static EnemiesKilled OnEnemiesKilled;

    private void Awake()
    {
        Enemy.OnEnemyDeath += EnqueueEnemy;
        LevelController.OnLevelSpawned += UpdateTakenPositions;
        BombManager.OnBombInteraction += UpdatePosition;
        Destructible.OnDestructibleDestroy += UpdatePosition;
    }

    void UpdatePosition(LevelMapPos pos, bool isOccupied)
    {
        levelMap[pos.row, pos.column] = isOccupied;
    }

    void UpdateTakenPositions(bool[,] positionTakenMap)
    {
        levelMap = new bool[positionTakenMap.GetLength(0), positionTakenMap.GetLength(1)];

        for (int i = 0; i < levelMap.GetLength(0); i++)
        {
            for (int j = 0; j < levelMap.GetLength(1); j++)
            {
                levelMap[i, j] = positionTakenMap[i, j];
            }
        }

    }

    public void PlaceEnemy(int row, int column, float height, bool isSimple)
    {
        Queue<Enemy> targetQueue;
        Enemy enemyPrefab;

        Vector3 position = new(column * Constants._CellWidth, height, row * Constants._CellWidth);

        if (isSimple)
        {
            targetQueue = simpleEnemyQueue;
            enemyPrefab = simpleEnemy;
        }
        else
        {
            targetQueue = fastEnemyQueue;
            enemyPrefab = fastEnemy;
        }

        Enemy enemy = RetrieveEnemy(targetQueue, enemyPrefab, position);

        enemy.currentPos = new(row, column);

        spawnedEnemies++;
    }

    Enemy RetrieveEnemy(Queue<Enemy> targetQueue, Enemy enemyPrefab, Vector3 position)
    {
        Enemy enemy;

        if (targetQueue.Count == 0)
        {
            enemy = Instantiate(enemyPrefab, position, Quaternion.identity, enemyParent);
            enemy.EM = this;
        }
        else
        {
            enemy = targetQueue.Dequeue();
            enemy.transform.SetPositionAndRotation(position, Quaternion.identity);
        }

        enemy.gameObject.SetActive(true);

        return enemy;
    }

    void EnqueueEnemy(bool isSimple, Enemy deadEnemy)
    {
        if (isSimple)
        {
            simpleEnemyQueue.Enqueue(deadEnemy);
        }
        else
        {
            fastEnemyQueue.Enqueue(deadEnemy);
        }

        spawnedEnemies--;

        if (spawnedEnemies == 0)
        {
            OnEnemiesKilled?.Invoke();
        }
    }

    public LevelMapPos RetrieveFreePos(LevelMapPos origin)
    {
        List<LevelMapPos> freePos = new();

        int maxRow = levelMap.GetLength(0) - 1,
            maxCol = levelMap.GetLength(1) - 1;

        foreach (Direction dir in Enum.GetValues(typeof(Direction)))
        {
            AddFreePos(origin, dir, freePos, maxRow, maxCol);
        }

        if (freePos.Count > 0)
        {
            LevelMapPos newPos = freePos[UnityEngine.Random.Range(0, freePos.Count)];
            levelMap[origin.row, origin.column] = false;
            levelMap[newPos.row, newPos.column] = true;
            return newPos;
        }

        return origin;

    }

    private void AddFreePos(LevelMapPos origin, Direction direction, List<LevelMapPos> freePos, int maxRow, int maxCol)
    {
        switch (direction)
        {
            case (Direction.up):
                if (origin.row < maxRow && !levelMap[origin.row + 1, origin.column])
                    freePos.Add(new(origin.row + 1, origin.column));
                break;
            case (Direction.right):
                if (origin.column < maxCol && !levelMap[origin.row, origin.column + 1])
                    freePos.Add(new(origin.row, origin.column + 1));
                break;
            case (Direction.down):
                if (origin.row > 0 && !levelMap[origin.row - 1, origin.column])
                    freePos.Add(new(origin.row - 1, origin.column));
                break;
            case (Direction.left):
                if (origin.column > 0 && !levelMap[origin.row, origin.column - 1])
                    freePos.Add(new(origin.row, origin.column - 1));
                break;
        }
    }

    private void OnDestroy()
    {
        Enemy.OnEnemyDeath -= EnqueueEnemy;
        LevelController.OnLevelSpawned -= UpdateTakenPositions;
        BombManager.OnBombInteraction -= UpdatePosition;
        Destructible.OnDestructibleDestroy -= UpdatePosition;
    }

}
