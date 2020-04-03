using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlotPoint : Clickable
{
    public string symbol;
    public bool canvasOpen = false;

    public void SetupPoint(Transform head, string symbol){
        foreach(SimpleTransformInfluenced component in GetComponentsInChildren<SimpleTransformInfluenced>()){
            component.referenceTransform = head;
        }
        GetComponentInChildren<TextMeshPro>().text = symbol;
        this.symbol = symbol;
    }

    public override void Click(Vector3 hitpos){
        if(canvasOpen) InfoPanelController.singleton.ClosePointPanel(this);
        else InfoPanelController.singleton.OpenPointPanel(this);
        canvasOpen = !canvasOpen;
    }
}
