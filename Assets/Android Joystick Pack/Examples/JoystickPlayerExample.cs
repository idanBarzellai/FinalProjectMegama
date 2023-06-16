using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickPlayerExample : MonoBehaviour
{
    public float speed;
    public FixedJoystick variableJoystick;
    public Rigidbody rb;


    
    protected virtual Vector3 GetMovement()
    {
        return new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
    }

    Vector3 GetJoystickMovement(){
        Vector3 direction = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;
        return direction * speed * Time.fixedDeltaTime;
    }

    public void FixedUpdate()
    {
        rb.AddForce(GetJoystickMovement() , ForceMode.VelocityChange);
    }
}