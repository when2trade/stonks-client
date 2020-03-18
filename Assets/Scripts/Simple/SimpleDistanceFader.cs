using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fades a sprite renderer based on distance values
/// </summary>
public class SimpleDistanceFader : SimpleTransformInfluenced
{
    public float nearFadeStart = 0.5f, nearFadeEnd = 0.2f;
    public float farFadeStart = 5, farFadeEnd = 10;

    public SpriteRenderer sprite;

    void Update()
    {
        float dist = (transform.position - referenceTransform.position).magnitude;

        Color c = sprite.color;
        c.a = Mathf.Clamp01(Mathf.InverseLerp(nearFadeEnd, nearFadeStart, dist)) * 
          (1-Mathf.Clamp01(Mathf.InverseLerp(farFadeStart, farFadeEnd, dist)));

        sprite.color = c;
    }
}
