using UnityEngine;
using System;

[CreateAssetMenu(fileName = "SectorColors", menuName = "ScriptableObjects/SectorColorScheme", order = 1)]
public class SectorColorScheme : ScriptableObject
{
    public string[] categories;
    public Color[] colors;

    //edge colours
    public Color weakPositiveColor = Color.white, strongPositiveColor = Color.green, 
        weakNegativeColor = Color.white, strongNegativeColor = Color.red;

    public Color ColorOfCategory(string cat){
        return colors[Array.IndexOf(categories, cat)];
    }
}
