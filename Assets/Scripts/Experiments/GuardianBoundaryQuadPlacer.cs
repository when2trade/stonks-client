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

    void Start()
    {
        m_enforcer.TrackingChanged += RefreshDisplay;
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
        quadObject.localScale = new Vector3(length,1,width);
        Vector3 center = (points[0]+points[1]+points[2]+points[3])/4f;
        quadObject.position = center;
        quadObject.rotation = Quaternion.LookRotation(points[1]-points[0], Vector3.up);
      }
    }
}
