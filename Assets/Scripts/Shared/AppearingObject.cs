using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearingObject : MonoBehaviour
{
    [Header("Appear")]
    [SerializeField] Vector3 finalScale;
    [SerializeField] protected Transform _Mesh;
    [SerializeField] float appearSpeed = 10;

    protected bool hasSpawned = false;

    protected void OnEnable()
    {
        StartCoroutine(Appear());
    }

    IEnumerator Appear()
    {
        yield return new WaitForSeconds(0.15f);
        while (Vector3.Distance(_Mesh.localScale, finalScale) > 0)
        {
            _Mesh.localScale = Vector3.MoveTowards(_Mesh.localScale, finalScale, Time.deltaTime * appearSpeed);
            yield return null;
        }
        hasSpawned = true;
    }
}
