/*****************************************************************************
Author: Craig Hughes
Date: 5/18/2023

Description




*/
using System.Collections;
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

    [Header("Ground Checking")]
    [Tooltip("The difference between the ground and the player")]
    [SerializeField] private float groundOffset;

    [Tooltip("The layermask the ground is on")]
    [SerializeField] private LayerMask GroundLayers;

    [Tooltip("The radius the ground will check out. best practice is to keep it at most the radius of the character controller")]
    [SerializeField] private float GroundedRadius;
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
    //
    private float currentGravity;
    //
    private float yVelocity;
    //
    private bool grounded = false;
    //
    private bool isJumping = false;


    // Start is called before the first frame update
    void Start()
    {
        //set all variables
        CharCon = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentGravity = gravityForce;
        yVelocity = 0f;
    }


    // Update is called once per frame
    void Update()
    {
        UpdateGround();
        UpdateCameraRotation();
        UpdateMovement();
        
    }
    private void FixedUpdate()
    {
        ApplyGravity();
    }


    private void UpdateGround()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundOffset, transform.position.z);
        grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
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
        Vector3 velocity = ((transform.forward * currentDir.y) + (transform.right * currentDir.x)) * speed + (Vector3.up * yVelocity);
        CharCon.Move(velocity * Time.deltaTime);
/*        print(yVelocity);
        print(currentGravity);*/
    }

    private void ApplyGravity()
    {
        if (grounded && !isJumping)
        {
            yVelocity = 0;
        }
        else
        {
            yVelocity += gravityForce * Time.deltaTime;
        }
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
        if(grounded)
        {
            print("COMPLETED");
            isJumping = true;
            yVelocity = jumpForce;
            StartCoroutine(tickDownJump());
        }
    }

    public IEnumerator tickDownJump()
    {
        yield return new WaitForSeconds(.1f);
        isJumping = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - groundOffset, transform.position.z), GroundedRadius);

    }

    

}
