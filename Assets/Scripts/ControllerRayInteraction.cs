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
    public float pressThreshold = 0.5f; //what should we consider a press?
    public float maxControllerMove = 0.1f; //how much movement is allowed before we consider this a drag?

    Vector3[] posesL = {Vector3.zero, Vector3.zero};
    Vector3[] posesR = {Vector3.zero, Vector3.zero};

    Transform oldHoverL, oldHoverR;

    bool wasLDown = false, wasRDown = false;

    Vector3 oldPosL, oldPosR;
    float distTravelledL = 0, distTravelledR = 0;

    public GameObject lockonEffect;
    SimpleLockOnto lockonEffectLock;
    Animation lockonEffectAnim;

    void Start(){
        lockonEffectLock = lockonEffect.GetComponent<SimpleLockOnto>();
        lockonEffectAnim = lockonEffect.GetComponent<Animation>();
    }

    void LateUpdate() //(late cause we want to let the controllers move first!)
    {
        bool lDown = Input.GetAxis("Oculus_CrossPlatform_PrimaryIndexTrigger") > pressThreshold 
            || Input.GetButton("Fire3") //left trigger or X button
            || Input.GetMouseButton(0);
        bool rDown = Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger") > pressThreshold
            || Input.GetButton("Fire1"); //right trigger or A button

        posesL[0] = rayAnchorL.position;
        posesR[0] = rayAnchorR.position;

        posesL[1] = UpdateRay(rayAnchorL, lineL, ref oldHoverL, true);
        posesR[1] = UpdateRay(rayAnchorR, lineR, ref oldHoverR, false);

        //reposition rays
        lineL.SetPositions(posesL);
        lineR.SetPositions(posesR);

        //if trigger released, something's over the ray, and controller hasn't moved too far, treat as click
        if(!lDown && wasLDown){
            lineL.enabled = true;
            lockonEffect.SetActive(false);
            if(oldHoverL != null && distTravelledL < maxControllerMove){
                oldHoverL.GetComponent<Clickable>()?.Click(posesL[1]);
                SFXController.singleton.PlayClick(posesL[0]);
            }
        }
        if(!rDown && wasRDown){
            lineR.enabled = true;
            lockonEffect.SetActive(false);
            if(oldHoverR != null && distTravelledR < maxControllerMove){
                oldHoverR.GetComponent<Clickable>()?.Click(posesR[1]);
                SFXController.singleton.PlayClick(posesR[0]);
            }
        }

        if(lDown){
            distTravelledL += (rayAnchorL.position - oldPosL).magnitude;
            if(!wasLDown) distTravelledL = 0;

            if(distTravelledL > maxControllerMove){
                lineL.enabled = false;
                lockonEffect.SetActive(true);
                lockonEffect.transform.position = rayAnchorL.position;
                lockonEffectLock.referenceTransform = rayAnchorL;
                lockonEffectAnim.Play();
            }
        }

        if(rDown){
            distTravelledR += (rayAnchorR.position - oldPosR).magnitude;
            if(!wasRDown) distTravelledR = 0;

            if(distTravelledR > maxControllerMove){
                lineR.enabled = false;
                lockonEffect.SetActive(true);
                lockonEffect.transform.position = rayAnchorR.position;
                lockonEffectLock.referenceTransform = rayAnchorR;
                lockonEffectAnim.Play();
            }
        }
        
        wasLDown = lDown; wasRDown = rDown;
        oldPosL = rayAnchorL.position; oldPosR = rayAnchorR.position;
    }

    //does raycasts & sends hover events.
    //returns a Vector3 for where the visible line should end.
    Vector3 UpdateRay(Transform rayAnchor, LineRenderer line, ref Transform oldHover, bool isLeft){
        RaycastHit hit;
        if(Physics.Raycast(rayAnchor.position, rayAnchor.up, out hit, raycastLimit)){ //if we hit an object,
            line.colorGradient = lineGradientHit;

            if(hit.transform != oldHover){ //if this is a new object,
                hit.transform.GetComponent<PointHoverGlow>()?.HoverEnter(); //do hover enter animation (if applicable)
                SFXController.singleton.PlayHover(rayAnchor.position);
                SFXController.singleton.PlayVibrate(isLeft);
                
                 if(oldHover!=null){ //if we were hovering over something else last frame,
                   oldHover.GetComponent<PointHoverGlow>()?.HoverExit(); //do hover exit animation (if applicable)
                }
            }
            oldHover = hit.transform;
            return hit.point;
        }
        else{
            line.colorGradient = lineGradientMiss;

            if(oldHover!=null){ //if we were just hovering over something last frame,
                oldHover.GetComponent<PointHoverGlow>()?.HoverExit(); //do hover exit animation (if applicable)
            }
            oldHover = null;
            return rayAnchor.position + rayAnchor.up * lineLength;
        }
    }
}
