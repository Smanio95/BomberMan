using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LevelMapPos
{
    public int row;
    public int column;

    public LevelMapPos(int i, int j)
    {
        row = i;
        column = j;
    }
}

public enum Direction
{
    left,
    up,
    right,
    down
}

public class LevelController : MonoBehaviour
{
    [SerializeField] EnemyManager EM;
    [SerializeField] LevelObject currentLevel;
    [SerializeField] Transform[] permanentObjects;
    [SerializeField] Transform destructiblesParent;
    [SerializeField] float waitingTime = 0.15f;
    [SerializeField] float spawnThreshold = 0.8f;
    [SerializeField] float freedomRowThreshold = 4;

    public delegate void LevelSpawning(bool isSpawning);
    public static LevelSpawning OnLevelSpawning;

    public delegate void LevelSpawned(bool[,] levelMap);
    public static LevelSpawned OnLevelSpawned;

    private bool[,] levelMap;

    private void Awake()
    {
        if (EM == null) EM = FindObjectOfType<EnemyManager>();
    }

    private void OnEnable()
    {
        OnLevelSpawning?.Invoke(true);

        BuildLevelMap();
        InsertDestructibles();
    }

    void BuildLevelMap()
    {
        levelMap = new bool[currentLevel.rows, currentLevel.columns];

        for (int i = 0; i < currentLevel.rows; i++)
        {
            for (int j = 0; j < currentLevel.columns; j++)
            {
                levelMap[i, j] = false;
            }
        }

        foreach (Transform p in permanentObjects)
        {
            int row = ((int)p.position.z) / (int)Constants._CellWidth;
            int column = ((int)p.position.x) / (int)Constants._CellWidth;
            levelMap[row, column] = true;
        }
    }

    void InsertDestructibles()
    {
        if (currentLevel.destructiblePrefab == null) return;

        StartCoroutine(InsertDestructibleObjs());
    }

    IEnumerator InsertDestructibleObjs()
    {
        int numberToSpawn = currentLevel.destructibles;

        for (int i = 0; i < currentLevel.rows && numberToSpawn > 0; i++)
        {
            for (int j = 0; j < currentLevel.columns && numberToSpawn > 0; j++)
            {
                if (!levelMap[i, j] && Random.value > spawnThreshold && !(i > currentLevel.rows - freedomRowThreshold && j < freedomRowThreshold))
                {
                    CreateDestructible(i, j, ref numberToSpawn);
                    yield return new WaitForSeconds(waitingTime);
                }
            }
        }

        InsertEnemies();
    }

    void CreateDestructible(int row, int column, ref int toSpawn)
    {
        Vector3 position = new(column * Constants._CellWidth, currentLevel.yPosition, row * Constants._CellWidth);
        Destructible destructible = Instantiate(currentLevel.destructiblePrefab, position, Quaternion.identity, destructiblesParent);
        destructible.currentPos = new(row, column);
        levelMap[row, column] = true;
        toSpawn--;
    }

    void InsertEnemies()
    {
        StartCoroutine(InsertEnemiesCoroutine());
    }

    IEnumerator InsertEnemiesCoroutine()
    {
        OnLevelSpawned?.Invoke(levelMap);

        List<LevelMapPos> freePositions = new();
        int simpleToSpawn = currentLevel.simpleEnemies;
        int fastToSpawn = currentLevel.fastEnemies;

        for (int i = 0; i < (currentLevel.rows - freedomRowThreshold) && (simpleToSpawn > 0 || fastToSpawn > 0); i++)
        {
            for (int j = 0; j < currentLevel.columns && (simpleToSpawn > 0 || fastToSpawn > 0); j++)
            {
                if (!levelMap[i, j])
                {
                    freePositions.Add(new(i,j));
                }
            }
        }

        for (int i = 0; i < simpleToSpawn; i++)
        {
            TakeRandom(freePositions, true);
            yield return new WaitForSeconds(waitingTime);
        }

        for (int i = 0; i < fastToSpawn; i++)
        {
            TakeRandom(freePositions, false);
            yield return new WaitForSeconds(waitingTime);
        }

        OnLevelSpawning?.Invoke(false);

    }

    void TakeRandom(List<LevelMapPos> freePositions, bool isSimple)
    {
        int index = Random.Range(0, freePositions.Count);
        LevelMapPos freePos = freePositions[index];
        
        freePositions.RemoveAt(index);

        EM.PlaceEnemy(freePos.row, freePos.column, currentLevel.yPosition, isSimple);

        levelMap[freePos.row, freePos.column] = true;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
