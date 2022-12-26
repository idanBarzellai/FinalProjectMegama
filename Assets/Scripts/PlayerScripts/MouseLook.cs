using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    private Vector2 mousePlace;
    private float sensitivity = 10;
    private Vector3 cameraFocusPointVector;
    private Vector3 focusPointCameraVector;
    private float cameraFocusPointDistance;
    private Ray zoomRay;
    private RaycastHit hit;
    private float CameraFocusPointDistanceAdjust;
    private int layerMask = 1 << 8;

    public Transform playerTransform;
    public Transform focusPoint;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cameraFocusPointDistance = 4f;
    }

    void Update()
    {
        lockAndUnlockCursorCheck();
        lookToMouseDirection();
        zoomAdjust();
    }

    void lockAndUnlockCursorCheck()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else if (Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    void lookToMouseDirection()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            //get mouse pos
            mousePlace = new Vector2(-Input.GetAxis(MouseAxis.MOUSE_Y), Input.GetAxis(MouseAxis.MOUSE_X));
            //update x rotation to player
            playerTransform.rotation *= Quaternion.Euler(0f, mousePlace.y * sensitivity * 0.15f, 0f);

            cameraFocusPointVector = (transform.position - focusPoint.position).normalized * 4;
            cameraFocusPointVector += new Vector3(0f, mousePlace.x * sensitivity * 0.03f, 0f);
            cameraFocusPointVector.y = Mathf.Clamp(cameraFocusPointVector.y, -2.5f, 3.875f);
            setCameraInDistance(cameraFocusPointDistance);

            transform.LookAt(focusPoint);
        }
    }

    void zoomAdjust()
    {
        focusPointCameraVector = transform.position - focusPoint.position;
        zoomRay = new Ray(focusPoint.position, focusPointCameraVector.normalized);
        cameraFocusPointDistance -= Input.GetAxis(MouseAxis.SCROLL_WHEEL) * sensitivity * 0.2f;
        Physics.Raycast(zoomRay, out hit, cameraFocusPointDistance, ~layerMask);
        if (hit.collider)
        {
            CameraFocusPointDistanceAdjust = hit.distance - 0.3f;
            setCameraInDistance(CameraFocusPointDistanceAdjust);
        }
        else
        {
            cameraFocusPointDistance = Mathf.Clamp(cameraFocusPointDistance, 2.5f, 5);
            setCameraInDistance(cameraFocusPointDistance);
        }
    }

    void setCameraInDistance(float distance)
    {
        //cameraFocusPointVector = (transform.position - focusPoint.position).normalized * distance;
        transform.position = (cameraFocusPointVector.normalized * distance) + focusPoint.position;
    }
}