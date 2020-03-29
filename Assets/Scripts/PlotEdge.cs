using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotEdge : MonoBehaviour
{
    public float value;

    public bool canvasOpen = false;

    public void Click(){
        if(canvasOpen) InfoPanelController.singleton.CloseEdgePanel(this);
        else InfoPanelController.singleton.OpenEdgePanel(this);
    }
}
