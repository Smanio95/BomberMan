using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] BombManager BM;

    void Update()
    {
        if (GameManager.gameStatus != GameManager.GameStatus.GameRunning) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            BM.PlaceBomb(new(transform.position.x, transform.position.y - (transform.localScale.y / 2), transform.position.z));
        }
    }
}
