using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseInputMover : MonoBehaviour
{
    public Transform headTransform; //required to know direction of movement

    [Range(0,1)]
    public float damping = 0.5f; //how much slowing force to apply?
    public float acceleration = 1; //how quickly to speed up?
    public float maxSpeed = 1; //limit the vector magnitude

    private Vector3 velocity = Vector3.zero;

    float moveStartTime; //time when player started moving;
    bool isMoving = false;

    void Update()
    {
        //add and normalizer input vectors from both analogue sticks, for dual input
        Vector3 desiredMove = new Vector3(
            Input.GetAxis("Horizontal") + 
            Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal"),
            0,
            Input.GetAxis("Vertical") +
            Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical")
            ).normalized;
        
        if(desiredMove==Vector3.zero){
            isMoving = false;
        }
        else{
            if(!isMoving){
                moveStartTime = Time.time;
            }
            isMoving = true;
        }

        //rotate the desired movement vector based on head orientation, scale by acceleration
        Vector3 force = headTransform.rotation * desiredMove
         * (Time.time - moveStartTime) *acceleration;
        //add to velocity, apply damping and speed limit
        velocity = Vector3.ClampMagnitude(velocity*damping + force, maxSpeed);
        
        //move this object backwards based on velocity
        transform.position -= velocity * Time.deltaTime;

        //Debug.Log(velocity.magnitude);
    }
}
