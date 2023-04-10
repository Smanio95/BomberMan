using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CommonUtils
{
    public static Vector3 IntoVector3(LevelMapPos pos, float height)
    {
        return new Vector3(pos.column * Constants._CellWidth, height, pos.row * Constants._CellWidth);
    }

    public static LevelMapPos IntoLevelMapPos(Vector3 pos)
    {
        return new((int)(pos.z / Constants._CellWidth), (int)(pos.x / Constants._CellWidth));
    }
}
