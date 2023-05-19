/*****************************************************************************
Author: Craig Hughes
Date: 5/18/2023

Description




*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //public variables
    [Header("Camera Variables")]
    [Tooltip("Reference to the transform of the camera that is attached to the player")]
    [SerializeField] private Transform CameraRef;

    [Tooltip("How fast the camera can move using the mouse")]
    [SerializeField] private float mouseSensitivity;

    [Tooltip("The amount of smoothing when the player starts and stops looking around with the mouse")]
    [SerializeField] [Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;

    [Header("Movement")]

    [Tooltip("How fast the player can move")]
    [SerializeField] private float speed;

    [Tooltip("how strong the player jump is")]
    [SerializeField] private float jumpForce;

    //private variables
    
    //
    private Vector2 currentMouseDelta = Vector2.zero;
    //
    private Vector2 currentMouseDeltaVelocity = Vector2.zero;
    //
    private float cameraPitch = 0f;
    //reference to the character controller
    private CharacterController CharCon;

    // Start is called before the first frame update
    void Start()
    {
        //set all variables
        CharCon = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    // Update is called once per frame
    void Update()
    {
        UpdateCameraRotation();
    }

    private void UpdateCameraRotation()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        cameraPitch -= currentMouseDelta.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);

        CameraRef.localEulerAngles = Vector3.right * cameraPitch;

        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }
}
