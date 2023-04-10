using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AlphaChange", menuName = "ScriptableObjects/AlphaChange", order = 2)]
public class AlphaChange : ScriptableObject
{
    public Material[] mainMaterial;
    public Material[] transparentMaterial;
}
