using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scales this object when both controllers are pressed and 'stretched'.
/// Also updates position and rotation now (with one controller pressed) because who needs separation of concerns anyway
/// </summary>
public class StretchScaler : MonoBehaviour
{
    private static StretchScaler singletonInstance;
    public static StretchScaler singleton { get { return singletonInstance; } }
    void Awake(){
        if (singletonInstance != null && singletonInstance != this){
        Destroy(this);
        return;
        } else {
        singletonInstance = this;
        }
    }


    public Transform leftHand, rightHand, headAnchor;

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

    bool inputAllowed = true;

    void Update()
    {
        if(inputAllowed){
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

            float noiseAmnt = posVelocity.magnitude + 1-Quaternion.Dot(rotVelocity.normalized, Quaternion.identity) + Mathf.Abs(scaleVelocity);
            SFXController.singleton.UpdateDragNoise(scaleVelocity, noiseAmnt);
            Debug.Log(noiseAmnt);

            wasBothDown = bothDown; wasLDown = lDown; wasRDown = rDown;
        }
    }

    public float flyAnimRotateDuration = 0.2f, flyAnimPauseDuration = 0.1f, flyAnimMoveDuration = 0.2f;
    public Vector3 flyAnimEndOffset; //where the selected obj should end up
    public float flyAnimMinScale = 5; //minimum scale of world

    public void FlyTo(Transform target){
        StartCoroutine(FlyToCo(target));
    }

    IEnumerator FlyToCo(Transform target){
        inputAllowed = false;
        posVelocity = Vector3.zero;
        rotVelocity = Quaternion.identity;
        scaleVelocity = 0;
        wasBothDown = wasLDown = wasRDown = false;
        
        //rotate cloud so point lines up with view vector
        Vector3 delta = headAnchor.position - transform.parent.position;
        transform.position -= delta;
        transform.parent.position += delta;
        
        Quaternion startRot = transform.parent.rotation,
            endRot = Quaternion.FromToRotation(target.position - headAnchor.position,
                headAnchor.forward);

        var t = 0f;
        while(t<flyAnimRotateDuration){
            float tt = t/flyAnimRotateDuration-1;
            float f = 1+tt*tt*tt;

            transform.parent.rotation = Quaternion.Lerp(startRot, endRot, f);

            t+=Time.deltaTime;
            yield return null;
        }
        transform.parent.rotation = endRot;

        t = 0;
        while(t<flyAnimPauseDuration){ t+=Time.deltaTime; yield return null;}

        //move point to in front of the user. also scale space.
        Vector3 startPos = transform.position,
            endPos = transform.position +  //current pos
                (headAnchor.position + headAnchor.rotation*flyAnimEndOffset) //desired target pos
                 - target.position; //current target pos

        float oldScale = transform.localScale.x, newScale = Mathf.Max(oldScale, flyAnimMinScale);
            
        t = 0;
        while(t<flyAnimMoveDuration){
            float tt = t/flyAnimMoveDuration-1;
            float f = 1+tt*tt*tt;

            transform.position = Vector3.Lerp(startPos, endPos, f);
            float s = Mathf.Lerp(oldScale, newScale, f);
            //transform.localScale = new Vector3(s,s,s);
            t+=Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;
        transform.localScale = new Vector3(newScale,newScale,newScale);
        inputAllowed = true;
    }
}
