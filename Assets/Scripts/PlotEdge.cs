using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotEdge : Clickable
{
    public float value;

    public bool canvasOpen = false;

    public override void Click(){
        if(canvasOpen) InfoPanelController.singleton.CloseEdgePanel(this);
        else InfoPanelController.singleton.OpenEdgePanel(this);
        canvasOpen = !canvasOpen;
    }
}
