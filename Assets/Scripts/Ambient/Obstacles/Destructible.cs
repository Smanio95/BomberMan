using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : Obstacle
{
    [HideInInspector] public LevelMapPos currentPos;

    public delegate void DestructibleDestroy(LevelMapPos pos, bool isOccupied = false);
    public static DestructibleDestroy OnDestructibleDestroy;

    private void OnDestroy()
    {
        OnDestructibleDestroy?.Invoke(currentPos);
    }

}
