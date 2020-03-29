using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets this object's pos/rot/scale to that of the target transform. Useful when parent manipulation is undesirable. 
/// </summary>
public class SimpleLockOnto : SimpleTransformInfluenced
{
    public bool lockPosition = true;
    public bool lockRotation = false;
    public bool lockScale = false;

    void LateUpdate(){
        if(lockPosition) transform.position = referenceTransform.position;
        if(lockRotation) transform.rotation = referenceTransform.rotation;
        if(lockScale) transform.localScale = referenceTransform.localScale;
    }
}
