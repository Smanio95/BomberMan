using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private const int SIZE = 2;

    [Header("Graphic")]
    [SerializeField] Transform _Mesh;
    [SerializeField] float scaleChange = 0.15f;
    [SerializeField] float scaleSpeed = 10;
    [SerializeField] float explodeTime = 3;
    [SerializeField] GameObject centralExplosion;
    [Tooltip("Insert explosions from the closest one to the bomb")]
    [SerializeField] GameObject[] leftExplosions = new GameObject[SIZE];
    [SerializeField] GameObject[] upExplosions = new GameObject[SIZE];
    [SerializeField] GameObject[] rightExplosions = new GameObject[SIZE];
    [SerializeField] GameObject[] downExplosions = new GameObject[SIZE];
    [SerializeField] float inBetweenExplosionTime = 0.15f;

    [Header("Explosion")]
    [SerializeField] float explosionTime = 1;
    [SerializeField] LayerMask mask;
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip explosionClip;

    [Header("Func")]
    [HideInInspector] public BombManager BM;

    public delegate void PlayerHit();
    public static PlayerHit OnPlayerHit;

    public delegate void BombPlaced();
    public static BombPlaced OnBombPlaced;

    private GameObject[,] fourDirectionExplosions = new GameObject[4, SIZE];
    private Vector3 initialScale;
    private List<GameObject> freePos;
    private List<Collider> hits;

    private void Awake()
    {
        initialScale = _Mesh.localScale;
        BuildExplosionArray();
    }

    private void Start()
    {
        if (BM == null) BM = FindObjectOfType<BombManager>();
    }

    private void OnEnable()
    {
        OnBombPlaced += StopTick;
        Enemy.OnPlayerDeath += StopTick;

        freePos = new();
        hits = new();

        OnBombPlaced?.Invoke();
        source.Play();

        StartCoroutine(Explode());
    }

    void StopTick()
    {
        if (source.isPlaying) source.Stop();
    }

    private void OnDisable()
    {
        OnBombPlaced -= StopTick;
        Enemy.OnPlayerDeath -= StopTick;

        StopTick();

        _Mesh.gameObject.SetActive(true);
        _Mesh.localScale = initialScale;
        DisableExplosions();
        StopAllCoroutines();

        BM.EnqueueBomb(this);
    }

    void DisableExplosions()
    {
        if (centralExplosion.activeSelf) centralExplosion.SetActive(false);

        foreach (GameObject explosion in fourDirectionExplosions)
        {
            if (explosion.activeSelf) explosion.SetActive(false);
        }
    }

    void BuildExplosionArray()
    {
        BuildDirection(Direction.left, leftExplosions);
        BuildDirection(Direction.up, upExplosions);
        BuildDirection(Direction.right, rightExplosions);
        BuildDirection(Direction.down, downExplosions);
    }

    void BuildDirection(Direction index, GameObject[] arr)
    {
        for (int i = 0; i < 2; i++)
        {
            fourDirectionExplosions[(int)index, i] = arr[i];
        }
    }

    IEnumerator Explode()
    {
        yield return null;
        float changer;
        float elapsed = 0;
        while (elapsed < explodeTime)
        {
            elapsed += Time.deltaTime;
            changer = Mathf.PingPong(elapsed * scaleSpeed, scaleChange);
            _Mesh.localScale = initialScale * (1 + changer);
            yield return null;
        }

        _Mesh.gameObject.SetActive(false);

        yield return new WaitForSeconds(inBetweenExplosionTime);

        ManageAudio();

        Vector3 origin = new(transform.position.x, transform.position.y + (transform.localScale.y / 2), transform.position.z);

        ManageCentralExplosion(origin);
        CatchFreePos(origin);

        foreach (GameObject obj in freePos)
        {
            obj.SetActive(true);
        }

        foreach (Collider hit in hits)
        {
            CheckHits(hit);
        }

        yield return new WaitForSeconds(explosionTime);

        gameObject.SetActive(false);

    }
    
    void ManageAudio()
    {
        if(source.isPlaying) source.Stop();

        source.PlayOneShot(explosionClip);
    }

    void ManageCentralExplosion(Vector3 origin)
    {
        centralExplosion.SetActive(true);
        Collider[] colliders = Physics.OverlapSphere(origin, Constants._CellWidth / 2, mask);
        if (colliders.Length > 0)
        {
            foreach (Collider c in colliders)
            {
                hits.Add(c);
            }
        }
    }

    void CheckHits(Collider hit)
    {
        switch (hit.tag)
        {
            case Tags.Player:
                OnPlayerHit?.Invoke();
                break;
            case Tags.Destructible:
                Destroy(hit.gameObject);
                break;
            case Tags.Enemy:
                hit.gameObject.SetActive(false);
                break;
        }
    }

    void CatchFreePos(Vector3 origin)
    {
        CatchFreePosAux(origin, -transform.right, Direction.left, SIZE);
        CatchFreePosAux(origin, transform.forward, Direction.up, SIZE);
        CatchFreePosAux(origin, transform.right, Direction.right, SIZE);
        CatchFreePosAux(origin, -transform.forward, Direction.down, SIZE);
    }

    void CatchFreePosAux(Vector3 origin, Vector3 direction, Direction index, int propagationN)
    {
        if (propagationN == 0) return;

        bool hasHit = Physics.SphereCast(origin, Constants._CellWidth / 2, direction, out RaycastHit hit, Constants._CellWidth, mask);
        if (hasHit)
        {
            if (!hit.collider.CompareTag(Tags.Permanent))
            {
                hits.Add(hit.collider);
                freePos.Add(fourDirectionExplosions[(int)index, SIZE - propagationN]);
            }
            return;
        }

        freePos.Add(fourDirectionExplosions[(int)index, SIZE - propagationN]);
        Vector3 newPos = direction * Constants._CellWidth;

        CatchFreePosAux(new(origin.x + newPos.x, origin.y, origin.z + newPos.z),
            direction,
            index,
            propagationN - 1);
    }
}
