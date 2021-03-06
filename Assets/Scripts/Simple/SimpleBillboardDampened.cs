using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spins an object over time to always face a given target.
/// </summary>
public class SimpleBillboardDampened : SimpleTransformInfluenced
{
    [Range(0,1)]
    public float rotateDamping = 0.9f;

    public Vector3 offset = Vector3.zero; 

    void Update()
    {
      Vector3 dir = referenceTransform.position - transform.position - offset;
      transform.rotation = Quaternion.Lerp( 
        Quaternion.LookRotation(dir, Vector3.up),
        transform.rotation,
        rotateDamping);
    }

    public void SnapToDesiredRotation(){
      Vector3 dir = referenceTransform.position - transform.position - offset;
      transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }
}
