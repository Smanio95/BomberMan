using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientAudio : MonoBehaviour
{
    [SerializeField] AudioSource source;

    public static float timeReached;

    private void Awake()
    {
        source.time = timeReached;
    }

    private void Update()
    {
        timeReached = source.time;
    }
}
