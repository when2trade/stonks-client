using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Negates the scaling effect of this transform on its children.
/// </summary>
public class ScaleNegator : MonoBehaviour
{
    private Vector3 parentOriginalScale;
    public float originalChildScale = 0.1f;

    public List<Transform> objectsToScale = new List<Transform>();

    private void LateUpdate()
    {
      var scale = transform.localScale;
      var inverse = new Vector3 (1/scale.x, 1/scale.y, 1/scale.z) * originalChildScale;

      foreach (Transform child in objectsToScale)
      {
        child.localScale = inverse;
      }
    }
}