using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotPoint : MonoBehaviour
{
    public void SetHeadTransform(Transform head){
        foreach(SimpleTransformInfluenced component in GetComponentsInChildren<SimpleTransformInfluenced>()){
            component.referenceTransform = head;
        }
    }
}
