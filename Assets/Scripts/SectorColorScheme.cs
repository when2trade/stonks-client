using UnityEngine;
using System;

[CreateAssetMenu(fileName = "SectorColors", menuName = "ScriptableObjects/SectorColorScheme", order = 1)]
public class SectorColorScheme : ScriptableObject
{
    public string[] categories;
    public Color[] colors;

    public Color ColorOfCategory(string cat){
        return colors[Array.IndexOf(categories, cat)];
    }
}
