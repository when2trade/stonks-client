using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerRayInteraction : MonoBehaviour
{
    public Transform rayAnchorL, rayAnchorR;
    public LineRenderer lineL, lineR;

    public float lineLength = 5;

    public float raycastLimit = 20;

    public Gradient lineGradientMiss, lineGradientHit;

    Vector3[] posesL = {Vector3.zero, Vector3.zero};
    Vector3[] posesR = {Vector3.zero, Vector3.zero};

    Transform oldHoverL, oldHoverR;

    void LateUpdate() //(late cause we want to let the controllers move first!)
    {
        posesL[0] = rayAnchorL.position;
        posesR[0] = rayAnchorR.position;

        posesL[1] = UpdateRay(rayAnchorL, lineL, ref oldHoverL);
        posesR[1] = UpdateRay(rayAnchorR, lineR, ref oldHoverR);

        //reposition rays
        lineL.SetPositions(posesL);
        lineR.SetPositions(posesR);
    }

    Vector3 UpdateRay(Transform rayAnchor, LineRenderer line, ref Transform oldHover){
        RaycastHit hit;
        if(Physics.Raycast(rayAnchor.position, rayAnchor.up, out hit, raycastLimit)){ //if we hit an object,
            line.colorGradient = lineGradientHit;

            if(hit.transform != oldHover){ //if this is a new object,
                if(hit.transform.GetComponent<PlotPoint>()!=null){
                    hit.transform.GetComponent<PointHoverGlow>().HoverEnter(); //do hover enter animation
                }
                 if(oldHover!=null){ //if we were hovering over something else last frame,
                    Debug.Log("AAAAAAAAAAAAAAAA");
                    if(oldHover.GetComponent<PlotPoint>()!=null){
                        oldHover.GetComponent<PointHoverGlow>().HoverExit(); //do hover exit animation
                    }
                }
            }
            oldHover = hit.transform;
            return hit.point;
        }
        else{
            line.colorGradient = lineGradientMiss;

            if(oldHover!=null){ //if we were just hovering over something last frame,
                if(oldHover.GetComponent<PlotPoint>()!=null){
                    oldHover.GetComponent<PointHoverGlow>().HoverExit(); //do hover exit animation
                }
            }
            oldHover = null;
            return rayAnchor.position + rayAnchor.up * lineLength;
        }
    }
}
