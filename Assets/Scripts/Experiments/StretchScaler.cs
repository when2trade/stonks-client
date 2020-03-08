using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StretchScaler : MonoBehaviour
{
    public Transform leftHand, rightHand;

    public LineRenderer scaleIndicatorLine;

    [Range(0,1)]
    public float threshold = 0.5f; //what should we consider a press?

    [Range(0,1)]
    public float damping = 0.9f;

    float velocity = 0;
    
    bool isActive = false;
    float oldDist;

    void Update()
    {
        float dist = (leftHand.position - rightHand.position).magnitude;

        bool triggersDown = Input.GetAxis("Oculus_CrossPlatform_PrimaryIndexTrigger") > threshold
                && Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger") > threshold;

        if(!isActive && triggersDown){
            isActive = true;
            oldDist = dist;
            scaleIndicatorLine.gameObject.SetActive(true);
        }
        if(isActive){
            if(!triggersDown){
                isActive = false;
                scaleIndicatorLine.gameObject.SetActive(false);
            }
            else{
                velocity = dist - oldDist;
                scaleIndicatorLine.SetPosition(0, leftHand.position);
                scaleIndicatorLine.SetPosition(1, rightHand.position);
                oldDist = dist;
            }
        }

        velocity*=damping;
        float scaleFactor = 1+velocity;
        transform.localScale *= scaleFactor;
        Vector3 scalePoint = (leftHand.position + rightHand.position)/2;
        transform.position = (transform.position - scalePoint)*scaleFactor + scalePoint;
    }
}
