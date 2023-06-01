/*****************************************************************************
Author: Craig Hughes
Date: 5/18/2023

Description: This holds the movement, interactions, and data of the player
anything involving the player directly is held within this script

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

    [Tooltip("The multiplier added when the player presses shift")]
    [SerializeField] private float speedMultiplier;

    [Header("Ground Checking")]
    [Tooltip("The difference between the ground and the player")]
    [SerializeField] private float groundOffset;

    [Tooltip("The layermask the ground is on")]
    [SerializeField] private LayerMask GroundLayers;

    [Tooltip("The radius the ground will check out. best practice is to keep it at most the radius of the character controller")]
    [SerializeField] private float GroundedRadius;

    [Header("Health")]
    [Tooltip("How much damage the player can take before losing")]
    [SerializeField] private int health;

    [Header("Arrow stats")]
    [Tooltip("How many arrows the player currently has")]
    [SerializeField] private int ammo;

    [Tooltip("How strong your arrow can be shot")]
    [SerializeField] private float maxArrowForce;

    [Tooltip("Reference to the arropw prefab")]
    [SerializeField] private GameObject Arrow;

    [Tooltip("where the arrow will come out")]
    [SerializeField] private Transform ShootFrom;
    //private variables
    
    //where the mouse currently is on the screen
    private Vector2 currentMouseDelta = Vector2.zero;
    //how fast the mouse is currently moving
    private Vector2 currentMouseDeltaVelocity = Vector2.zero;
    //the angle at which the camera is currently oriented 
    private float cameraPitch = 0f;
    //reference to the character controller
    private CharacterController CharCon;

    // Movement
    //the vector passed by the player input 
    private Vector2 moveInput;
    //how fast and in what direction the player is moving on the y axis 
    //private float velocityY = 0f;
    //the direction the player moves on the x/z axis
    private Vector2 currentDir = Vector2.zero;
    //the speed the player moves at on the x/z axis
    private Vector3 currentDirVelocity = Vector2.zero;
    //the absolute fastest the player can move on the y axis
    private float maxSpeed = 53.0f;
    //how much gravity force is currently affecting the player
    private float currentGravity;
    //how fast and in what direction the player is moving on the y axis 
    private float yVelocity;
    //is the player currently on the ground
    private bool grounded = false;
    //keeps track of if the player is jumping
    private bool isJumping = false;
    //
    private float currentArrowForce;
    //
    private bool shotPressed = false;
    //
    private GameObject CurrentArrow;


    

    // Start is called before the first frame update
    void Start()
    {
        //set all variables
        CharCon = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentGravity = gravityForce;
        yVelocity = 0f;
        currentArrowForce = 0f;
        CurrentArrow = null;
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


    /// <summary>
    /// this creates a sphere below the player to check if the player is on the floor
    /// if the player is on the ground then the ground variable will be set to True
    /// otherwise it's set to False
    /// </summary>
    private void UpdateGround()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundOffset, transform.position.z);
        grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
    }
    /// <summary>
    /// This script calculates the next position of the camera based on the mouse's movement 
    /// </summary>
    private void UpdateCameraRotation()
    {
        //get the mouse's position,desired end spot, and the player's input
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);
        //calculate where the camera can move next
        cameraPitch -= currentMouseDelta.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);
        CameraRef.localEulerAngles = Vector3.right * cameraPitch;
        //rotate the camera towards the calculated rotation
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
    }

    /// <summary>
    /// this script adds or resets gravity based on where the player is in relation to the ground
    /// </summary>
    private void ApplyGravity()
    {
        if (grounded && !isJumping)//if not jumping or is on the ground then reset gravity forces
        {
            yVelocity = 0;
        }
        else//otherwise add the gravity that affects the player
        {
            yVelocity += gravityForce * Time.deltaTime;
        }
    }


    /// <summary>
    /// sets the movement input to the vector that determines what direction the player
    /// is moving in
    /// </summary>
    /// <param name="value">the vector that the player must move in</param>
    private void OnMovement(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    
    /// <summary>
    /// adds a force to the player to move them up 
    /// </summary>
    /// <param name="value">the input of the player</param>
    private void OnJump(InputValue value)
    {
        if(grounded)//if not already jumping
        {
            isJumping = true;
            yVelocity = jumpForce;
            StartCoroutine(tickDownJump());//prevents desynchronization of jumping
        }
    }

    /// <summary>
    /// turns spprinting on and off when the player presses the sprint button
    /// </summary>
    /// <param name="value">a bool that determines if the button is being pressed or unpressed</param>
    private void OnSprint(InputValue value)
    {
        if(value.isPressed)//add sprint
        {
            speed *= speedMultiplier;
        }
        else//remove sprint
        {
            speed /= speedMultiplier;
        }
    }
    /// <summary>
    /// a coroutine that prevents the player from not being able to jump
    /// by delaying when they can jump again
    /// </summary>
    /// <returns> .1 seconds </returns>
    public IEnumerator tickDownJump()
    {
        yield return new WaitForSeconds(.1f);
        isJumping = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    private void OnPrimaryShoot(InputValue value)
    {
        LoadArrow(value, 1);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    private void OnSecondaryShoot(InputValue value)
    {
        LoadArrow(value,2);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="arrowNum"></param>
    private void LoadArrow(InputValue value, int arrowNum)
    {
        if (value.isPressed)
        {
            if(ammo > 0)
            {
                shotPressed = true;
                StartCoroutine(chargeShot());
            }
        }
        else
        {
            if(ammo > 0)
            {
                CurrentArrow = Instantiate(Arrow, ShootFrom.position,Arrow.transform.rotation);
                print(CurrentArrow.transform.rotation);

                CurrentArrow.GetComponent<Rigidbody>().velocity = CameraRef.forward  * (currentArrowForce * maxArrowForce);
                currentArrowForce = 0f;
                ammo--;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerator chargeShot()
    {
        while(shotPressed)
        {
            if(currentArrowForce < 1f)
            {
                currentArrowForce += Time.deltaTime;
            }
            yield return new WaitForSeconds(.01f);

        }
    }








    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - groundOffset, transform.position.z), GroundedRadius);

    }

    

}
