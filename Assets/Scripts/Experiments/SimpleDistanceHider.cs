﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Disables a Behaviour when this object is inside/outside range of an object.
/// </summary>
public class SimpleDistanceHider : MonoBehaviour
{
    public Behaviour targetBehaviour;
    public Transform referenceTransform;

    public float minDist = 1, maxDist = 10;

    void Update()
    {
        float mag = (transform.position - referenceTransform.position).sqrMagnitude;
        targetBehaviour.enabled = mag > minDist && mag < maxDist;
    }
}
