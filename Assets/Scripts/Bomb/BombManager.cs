using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombManager : MonoBehaviour
{
    [SerializeField] Bomb bombPrefab;
    [SerializeField] Transform bombParent;

    private readonly Queue<Bomb> bombQueue = new();

    public delegate void BombPlaced(LevelMapPos pos);
    public static BombPlaced OnBombPlaced;

    public void PlaceBomb(Vector3 position)
    {
        position = new Vector3(Mathf.Round(position.x / Constants._CellWidth) * Constants._CellWidth,
            position.y,
            Mathf.Round(position.z / Constants._CellWidth) * Constants._CellWidth);

        OnBombPlaced?.Invoke(CommonUtils.IntoLevelMapPos(position));

        Bomb bomb;

        if (bombQueue.Count == 0)
        {
            bomb = Instantiate(bombPrefab, position, bombPrefab.transform.rotation, bombParent);
            bomb.BM = this;
            return;
        }

        bomb = bombQueue.Dequeue();
        bomb.transform.position = position;
        bomb.gameObject.SetActive(true);

    }

    public void EnqueueBomb(Bomb bomb)
    {
        bombQueue.Enqueue(bomb);
    }

}
