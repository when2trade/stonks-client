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

    public float moveScale = 1;

    float scaleVelocity = 0;
    Vector3 posVelocity = Vector3.zero;
    
    bool isActive = false;
    float oldDist;
    float startDist = 0;
    public float elasticShaderStretchFactor = 0.5f; //how much to alter the StretchAmount parameter

    Vector3 oldPos;

    void Update()
    {
        float dist = (leftHand.position - rightHand.position).magnitude;
        Vector3 centrePos = (leftHand.position + rightHand.position)/2f; //get centre point of controllers

        bool triggersDown = Input.GetAxis("Oculus_CrossPlatform_PrimaryIndexTrigger") > threshold
                && Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger") > threshold;

        if(!isActive && triggersDown){
            isActive = true;
            oldDist = dist;
            startDist = dist;
            oldPos = centrePos;
            scaleIndicatorLine.gameObject.SetActive(true);
        }
        if(isActive){
            if(!triggersDown){
                isActive = false;
                scaleIndicatorLine.gameObject.SetActive(false);
            }
            else{
                scaleVelocity = dist - oldDist;
                posVelocity = centrePos - oldPos;
                scaleIndicatorLine.SetPosition(0, leftHand.position);
                scaleIndicatorLine.SetPosition(1, rightHand.position);
                scaleIndicatorLine.material.SetFloat("_StretchAmount",(dist - startDist) * elasticShaderStretchFactor+0.5f);
                oldDist = dist;
                oldPos = centrePos;
            }
        }

        scaleVelocity *= damping;
        posVelocity *= damping;

        //scale around the controller centre 
        float scaleFactor = 1 + scaleVelocity;
        transform.localScale *= scaleFactor;
        transform.position = (transform.position - centrePos)*scaleFactor + centrePos;

        //position
        transform.position += posVelocity * moveScale;

    }
}
