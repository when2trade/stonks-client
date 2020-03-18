using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spins an object to always face a given target.
/// </summary>
public class SimpleBillboard : SimpleTransformInfluenced
{    
    void Update()
    {
        transform.LookAt(referenceTransform);
    }
}
