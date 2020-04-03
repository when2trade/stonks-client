using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotEdge : Clickable
{
    public float value;
    public string symbol1, symbol2;
    public bool canvasOpen = false;

    public SectorColorScheme colorScheme;

    public void SetupEdge(string symbol1, string symbol2, float value){
        this.symbol1 = symbol1;
        this.symbol2 = symbol2;
        this.value = value;

        LineRenderer line = GetComponent<LineRenderer>();

        if(value > 0) //colour green or red
            //linear interpolation between weak and strong correlation colours
            line.startColor = line.endColor = Color.Lerp(colorScheme.weakPositiveColor, colorScheme.strongPositiveColor, value);
        else
            line.startColor = line.endColor = Color.Lerp(colorScheme.weakNegativeColor, colorScheme.strongNegativeColor, -value);
    }


    public override void Click(Vector3 hitPos){
        if(canvasOpen) InfoPanelController.singleton.CloseEdgePanel(this);
        else InfoPanelController.singleton.OpenEdgePanel(this, hitPos);
        canvasOpen = !canvasOpen;
    }
}
