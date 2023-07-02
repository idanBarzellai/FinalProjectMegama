using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FixedJoystick : Joystick
{
    AndroidMovementButtons androidMovement;
    private void Awake(){
        androidMovement = transform.parent.GetComponent<AndroidMovementButtons>();
    }
    private void Update() {
        BasicsController controller = androidMovement.controller;
        if (controller == null) {Debug.LogWarning("no controller"); return;}

        controller.Horizontal = Horizontal;
        controller.Vertical = Vertical;
    }
    
}