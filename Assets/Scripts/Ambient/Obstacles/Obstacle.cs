using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : AppearingObject
{
    [SerializeField] AlphaChange graphic;
    [SerializeField] Renderer[] structure;

    private new void OnEnable()
    {
        base.OnEnable();
        LevelManager.OnLevelUpdate += ChangeMesh;
    }

    void ChangeMesh(float height)
    {
        for (int i = 0; i < structure.Length; i++)
        {
            structure[i].material = graphic.transparentMaterial[i];
        }
        enabled = false;
    }

    private void OnDisable()
    {
        LevelManager.OnLevelUpdate -= ChangeMesh;
    }

}
