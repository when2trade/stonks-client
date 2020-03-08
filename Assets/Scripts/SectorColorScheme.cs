using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SectorColors", menuName = "ScriptableObjects/SectorColorScheme", order = 1)]
public class SectorColorScheme : ScriptableObject
{
    public string[] categories;
    public Color[] colors;
}
