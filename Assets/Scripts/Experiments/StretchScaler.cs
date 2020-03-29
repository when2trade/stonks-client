using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scales this object when both controllers are pressed and 'stretched'.
/// Also updates position and rotation now (with one controller pressed) because who needs separation of concerns anyway
/// </summary>
public class StretchScaler : MonoBehaviour
{
    public Transform leftHand, rightHand;

    public LineRenderer scaleIndicatorLine;

    [Range(0,1)]
    public float pressThreshold = 0.5f; //what should we consider a press?

    [Range(0,1)]
    public float damping = 0.9f;

    public float moveScale = 1;

    float scaleVelocity = 0;
    Vector3 posVelocity = Vector3.zero;
    Quaternion rotVelocity = Quaternion.identity;
    
    bool wasBothDown = false;
    bool wasLDown = false;
    bool wasRDown = false;

    float oldDist;
    float startDist = 0;
    public float elasticShaderStretchFactor = 0.5f; //how much to alter the StretchAmount parameter

    public float elasticStartOffset = 0.03f; //how mush should we offset the start of the elastic, so it doesn't pass through the controllers

    Vector3 oldPos, oldHandVec;
    Quaternion startRot;

    void Update()
    {
        Vector3 lPos = leftHand.position, rPos = rightHand.position;
        Vector3 handVec = lPos - rPos;
        float dist = handVec.magnitude;
        Vector3 centrePos = (lPos + rPos)/2f; //get centre point of controllers

        bool lDown = Input.GetAxis("Oculus_CrossPlatform_PrimaryIndexTrigger") > pressThreshold;
        bool rDown = Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger") > pressThreshold;
        bool bothDown = lDown && rDown;

        if(bothDown){
            if(!wasBothDown){ //start stretch & rot
                //isActive = true;
                oldDist = dist;
                startDist = dist;

                oldHandVec = handVec;
                startRot = transform.rotation;
                scaleIndicatorLine.gameObject.SetActive(true);
            }
            else{ //continue stretch & rot
                scaleVelocity = dist - oldDist;
                rotVelocity = Quaternion.FromToRotation(oldHandVec, handVec);

                scaleIndicatorLine.SetPosition(0, lPos + (rPos-lPos).normalized*elasticStartOffset);
                scaleIndicatorLine.SetPosition(1, rPos + (lPos-rPos).normalized*elasticStartOffset);

                scaleIndicatorLine.material.SetFloat("_StretchAmount",(dist - startDist) * elasticShaderStretchFactor+0.5f);

                oldDist = dist;
                oldHandVec = handVec;
            }
        }
        else{
            if(wasBothDown){ //end stretch & rot
                //isActive = false;
                scaleIndicatorLine.gameObject.SetActive(false);
                startRot = transform.rotation;
            }
            if(lDown){
                if(!wasLDown || wasRDown){ //start grab on left hand
                    oldPos = lPos;
                }
                posVelocity = lPos - oldPos;
                oldPos = lPos;
            }
            else if(rDown){
                if(!wasRDown || wasLDown){ //start grab on right hand
                    oldPos = rPos;
                }
                posVelocity = rPos - oldPos;
                oldPos = rPos;
            }
        }

        scaleVelocity *= damping;
        posVelocity *= damping;
        rotVelocity = Quaternion.Lerp(Quaternion.identity, rotVelocity, damping);

        //scale around the controller centre 
        float scaleFactor = 1 + scaleVelocity;
        transform.localScale *= scaleFactor;
        transform.position = (transform.position - centrePos)*scaleFactor + centrePos;

        //position
        transform.position += posVelocity * moveScale;

        //rotation
        //Quaternion finalRot = startRot* rotVelocity;
        Vector3 delta = centrePos-transform.parent.position;
        transform.position -= delta;
        transform.parent.position += delta;
        transform.parent.rotation = rotVelocity * transform.parent.rotation;

        wasBothDown = bothDown; wasLDown = lDown; wasRDown = rDown;
    }
}
