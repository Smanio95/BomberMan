using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : AppearingObject
{
    [SerializeField] bool isSimple;
    [SerializeField] float movementSpeed = 5;
    [SerializeField] LayerMask playerMask;
    [SerializeField] LayerMask blockingMask;
    public int pointValue = 100;

    [HideInInspector] public LevelMapPos currentPos;

    public delegate void EnemyDeath(bool isSimple, Enemy self);
    public static EnemyDeath OnEnemyDeath;

    public delegate void PlayerDeath();
    public static PlayerDeath OnPlayerDeath;

    [HideInInspector] public EnemyManager EM;

    private Vector3 nextPosition;

    private new void OnEnable()
    {
        base.OnEnable();
        LevelMapPos nextPos = EM.RetrieveFreePos(currentPos);
        nextPosition = CommonUtils.IntoVector3(nextPos, transform.position.y);
    }

    void Update()
    {
        if (!hasSpawned) return;

        Vector3 origin = new(transform.position.x, transform.position.y + (transform.localScale.y / 2), transform.position.z);

        CheckPlayerCollision(origin);

        if (Vector3.Distance(transform.position, nextPosition) > 0)
        {
            transform.LookAt(nextPosition);
            transform.position = Vector3.MoveTowards(transform.position, nextPosition, Time.deltaTime * movementSpeed);
        }
        else
        {
            CalculateNextPosition(transform.position.y);
        }

    }

    void CheckPlayerCollision(Vector3 origin)
    {
        Collider[] hit = Physics.OverlapSphere(origin, Constants._CellWidth / 2, playerMask);
        if(hit.Length > 0)
        {
            OnPlayerDeath?.Invoke();
        }
    }

    private void OnDisable()
    {
        OnEnemyDeath?.Invoke(isSimple, this);
    }


    void CalculateNextPosition(float height)
    {
        LevelMapPos nextPos = CommonUtils.IntoLevelMapPos(nextPosition);

        if (nextPos.row != currentPos.row || nextPos.column != currentPos.column)
        {
            EM.UpdatePosition(currentPos, false);
            currentPos = nextPos;
        }

        nextPos = EM.RetrieveFreePos(currentPos);

        if (nextPos.row == currentPos.row && nextPos.column == currentPos.column) return;

        nextPosition = CommonUtils.IntoVector3(nextPos, height);

    }

}
