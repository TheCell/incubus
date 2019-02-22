﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [System.Serializable]
    public class InputSettings : System.Object
    {
        public float inputThreshold = 0.1f;
        public string VERTICAL_AXIS = "CameraVertical";
        public string HORIZONTAL_AXIS = "CameraHorizontal";
        public string LOCK_TARGET_PYRAMID = "LockToPyramid";
        public string LOCK_TARGET_AREA = "LockToArea";
    }

    [SerializeField] private Transform target;
    [SerializeField] private float cameraSpeed = 2;
    [SerializeField] private Vector3 lookAtPlayerOffset = new Vector3(0f, 0.6f);

    private PlayerController playerController;
    private float cameraHorizontal, cameraVertical;
    private float lockPyramid, lockArea;
    private InputSettings inputSettings;
    private Vector3 cameraPlayerOffset;

    void Start()
    {
        SetCameraTarget(target);

        inputSettings = new InputSettings();
        cameraPlayerOffset = transform.position - playerController.transform.position;
    }

    private void Update()
    {
        GetInput();
    }

    private void LateUpdate()
    {
        // moving
        //MoveToTarget();
        // rotate
        //LookAtTarget();
        RotateCamera();
        updateCameraPosition();
        //updateCameraPositionAndRotation();
    }

    /*
    private void updateCameraPositionAndRotation()
    {
        float eulerXDelta = cameraHorizontal * cameraSpeed;
        float eulerYDelta = cameraVertical * cameraSpeed;

        Quaternion frameRotationDelta = Quaternion.Euler(eulerXDelta, eulerYDelta, 0f);

        //Debug.Log(transform.rotation.eulerAngles);
        // set camera inside player for rotation
        transform.position = playerController.transform.position;

        // rotate
        Quaternion currentCameraAngle = transform.rotation;
        //transform.rotation = currentCameraAngle * frameRotationDelta;
        Vector3 newOffset = currentCameraAngle * frameRotationDelta * cameraPlayerOffset;

        // set camera offset again
        Vector3 newPos = newOffset;
        transform.position = newPos;
    }
    */

    private void RotateCamera()
    {
        Quaternion cameraRotationXDelta = Quaternion.AngleAxis(cameraHorizontal * cameraSpeed, Vector3.up);
        Quaternion cameraRotationYDelta = Quaternion.AngleAxis(cameraVertical * cameraSpeed, Vector3.right);

        // this does not work how I wish it would after rotating around a bit
        cameraPlayerOffset = cameraRotationYDelta * cameraRotationXDelta * cameraPlayerOffset;
        //transform.rotation = transform.rotation * cameraRotationXDelta * cameraRotationYDelta;

    }

    private void updateCameraPosition()
    {
        // prevent camera from clipping through floor
        Vector3 newPos = playerController.transform.position + cameraPlayerOffset;
        newPos.y = newPos.y < 0.2f ? newPos.y = 0.2f : newPos.y;
        transform.position = newPos;
        transform.LookAt(target.transform.position + lookAtPlayerOffset);
    }

    private void GetInput()
    {
        cameraHorizontal = Input.GetAxis(inputSettings.HORIZONTAL_AXIS); // interpolated
        cameraVertical = Input.GetAxis(inputSettings.VERTICAL_AXIS); // interpolated

        lockPyramid = Input.GetAxis(inputSettings.LOCK_TARGET_PYRAMID); // interpolated
        lockArea = Input.GetAxis(inputSettings.LOCK_TARGET_AREA); // interpolated
    }

    private void SetCameraTarget(Transform targetTransform)
    {
        target = targetTransform;

        if (target != null)
        {
            if (target.GetComponent<PlayerController>())
            {
                playerController = target.GetComponent<PlayerController>();
            }
            else
            {
                Debug.LogError("The camera's target needs a player character controller.");
            }
        }
        else
        {
            Debug.LogError("Your camera needs a target");
        }
    }
}