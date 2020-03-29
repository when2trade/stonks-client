using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles raycast interactions between the controllers and other objects.
/// </summary>
public class ControllerRayInteraction : MonoBehaviour
{
    public Transform rayAnchorL, rayAnchorR;
    public LineRenderer lineL, lineR;

    public float lineLength = 5;

    public float raycastLimit = 20;

    public Gradient lineGradientMiss, lineGradientHit;

    [Range(0,1)]
    public float pressThreshold = 0.8f; //what should we consider a press?

    Vector3[] posesL = {Vector3.zero, Vector3.zero};
    Vector3[] posesR = {Vector3.zero, Vector3.zero};

    Transform oldHoverL, oldHoverR;

    bool wasLDown = false, wasRDown = false;

    void LateUpdate() //(late cause we want to let the controllers move first!)
    {
        bool lDown = Input.GetAxis("Oculus_CrossPlatform_PrimaryIndexTrigger") > pressThreshold || Input.GetMouseButton(0);
        bool rDown = Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger") > pressThreshold;

        posesL[0] = rayAnchorL.position;
        posesR[0] = rayAnchorR.position;

        posesL[1] = UpdateRay(rayAnchorL, lineL, ref oldHoverL);
        posesR[1] = UpdateRay(rayAnchorR, lineR, ref oldHoverR);

        if(lDown && !wasLDown && oldHoverL !=null){ //press L and something's over the left ray
            oldHoverL.GetComponent<PlotPoint>()?.Click();
        }
        if(rDown && !wasRDown && oldHoverR !=null){ //press R and something's over the right ray
            oldHoverR.GetComponent<PlotPoint>()?.Click();
        }


        //reposition rays
        lineL.SetPositions(posesL);
        lineR.SetPositions(posesR);

        wasLDown = lDown; wasRDown = rDown;
    }

    //does raycasts & sends hover events.
    //returns a Vector3 for where the visible line should end.
    Vector3 UpdateRay(Transform rayAnchor, LineRenderer line, ref Transform oldHover){
        RaycastHit hit;
        if(Physics.Raycast(rayAnchor.position, rayAnchor.up, out hit, raycastLimit)){ //if we hit an object,
            line.colorGradient = lineGradientHit;

            if(hit.transform != oldHover){ //if this is a new object,
                if(hit.transform.GetComponent<PlotPoint>()!=null){
                    hit.transform.GetComponent<PointHoverGlow>().HoverEnter(); //do hover enter animation
                }
                 if(oldHover!=null){ //if we were hovering over something else last frame,
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
