using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level", order = 1)]
public class LevelObject : ScriptableObject
{
    public float yPosition = 0;
    public int simpleEnemies = 1;
    public int fastEnemies = 1;
    public int rows = 10;
    public int columns = 10;
    public int destructibles = 5;
    public Destructible destructiblePrefab;
}
