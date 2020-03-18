using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Modification of GuardianBoundryDisplay. Positions an existing quad instead of drawing a Line.
/// </summary>
public class GuardianBoundaryQuadPlacer : MonoBehaviour
{
    // Depending on the demo manager only for reorient notifications.
    public GuardianBoundaryEnforcer m_enforcer;

    public Transform quadObject;

    public MeshRenderer mesh;

    Vector2 baseTextureScale; //original texture scale.

    Material mat;

    void Start()
    {
        m_enforcer.TrackingChanged += RefreshDisplay;
        mat = mesh.material;
        baseTextureScale = mat.mainTextureScale;
        RefreshDisplay();
    }

    void RefreshDisplay()
    {
		  bool configured = OVRManager.boundary.GetConfigured();
      if(configured)
      {
        //get the boundary points
        Vector3[] points = OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.PlayArea);

        float length = (points[1]-points[0]).magnitude;
        float width = (points[2]-points[1]).magnitude;
        quadObject.localScale = new Vector3(width,1,length);
        Vector3 center = (points[0]+points[1]+points[2]+points[3])/4f;
        quadObject.position = center;
        quadObject.rotation = Quaternion.LookRotation(points[1]-points[0], Vector3.up);
        mat.mainTextureScale = Vector2.Scale(baseTextureScale, new Vector2(length, width));
      }
    }
}
