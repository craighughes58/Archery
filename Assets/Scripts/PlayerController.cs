/*****************************************************************************
Author: Craig Hughes
Date: 5/18/2023

Description




*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [Tooltip("How strong the gravity that affects the player is")]
    [SerializeField] private float gravityForce;
    //private variables
    
    //
    private Vector2 currentMouseDelta = Vector2.zero;
    //
    private Vector2 currentMouseDeltaVelocity = Vector2.zero;
    //
    private float cameraPitch = 0f;
    //reference to the character controller
    private CharacterController CharCon;
    // Movement
    private Vector2 moveInput;
    //
    private float velocityY = 0f;
    //
    private Vector2 currentDir = Vector2.zero;
    //
    private Vector3 currentDirVelocity = Vector2.zero;
    //
    private float maxSpeed = 53.0f;


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
        UpdateMovement();
        ApplyGravity();
        
    }

    /// <summary>
    /// 
    /// </summary>
    private void UpdateCameraRotation()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        cameraPitch -= currentMouseDelta.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);

        CameraRef.localEulerAngles = Vector3.right * cameraPitch;

        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    /// <summary>
    /// 
    /// </summary>
    private void UpdateMovement()
    {
        currentDir = Vector3.SmoothDamp(currentDir, moveInput, ref currentDirVelocity, mouseSmoothTime);
        Vector3 velocity = ((transform.forward * currentDir.y) + (transform.right * currentDir.x)) * speed + (Vector3.up * 0);
        CharCon.Move(velocity * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        CharCon.Move(new Vector3(CharCon.velocity.x,gravityForce,CharCon.velocity.z) * Time.deltaTime);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    private void OnMovement(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    

    private void OnJump(InputValue value)
    {
        print("hello world");
        //CharCon.isGrounded 
    }

    
    
}
