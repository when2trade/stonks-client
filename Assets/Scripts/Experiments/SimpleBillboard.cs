using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spins an object to always face a given target.
/// </summary>
public class SimpleBillboard : MonoBehaviour
{
    public Transform targetTransform;
    
    void Update()
    {
        transform.LookAt(targetTransform);
    }
}
