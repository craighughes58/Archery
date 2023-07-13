/*****************************************************************************
Author: Craig Hughes
Date: 5/18/2023

Description: This holds the movement, interactions, and data of the player
anything involving the player directly is held within this script

*/
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

//Ensures required components are included in parent game object
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(AmmoInventory))]

public class PlayerController : MonoBehaviour
{

    #region Delegate events
    //subscribable delegate event to notify relevant parties that health is changing; currently used by UI -BMH
    public delegate void HealthUpdate(int newHealth);
    public static event HealthUpdate updateHealth;
    public delegate void AmmoUpdate(int newAmmo);
    public static event AmmoUpdate updateAmmo;
    public delegate void ChargeUpdate(float newCharge);
    public static event ChargeUpdate updateCharge;
    #endregion

    #region Private variables
    //public variables --did not remove label as I am not sure if change is intended later on, MKE
    [Header("Camera Variables")]
    [Tooltip("Reference to the transform of the camera that is attached to the player")]
    [SerializeField] private Transform CameraRef;

    [Tooltip("How fast the camera can move using the mouse")]
    [SerializeField] private float mouseSensitivity;

    [Tooltip("The amount of smoothing when the player starts and stops looking around with the mouse")]
    [SerializeField] [Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;

    #region Serialized Movement Variables
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
    #endregion

    [Tooltip("How strong your arrow can be shot")]
    [SerializeField] private float maxArrowForce;

    [Tooltip("Reference to the arrow prefab")]
    [SerializeField] private GameObject Arrow;

    [Tooltip("where the arrow will come out")]
    [SerializeField] private Transform ShootFrom;

    [Tooltip("How much distance the player can cover while grappled")]
    [SerializeField] private float maxGrappleSpeed;

    [Tooltip("How far off from the arrow that the player will be launched up and pushed forward. x will change the y and y will change the z")]
    [SerializeField] private Vector2 GrappleLaunchOffset;

    [Tooltip("This will affect how strong the jump is when you launch off a grapple")]
    [SerializeField, Range(0,10)] private float GrappleLaunchForce;
    //private variables
    #endregion

    //where the mouse currently is on the screen
    private Vector2 currentMouseDelta = Vector2.zero;
    //how fast the mouse is currently moving
    private Vector2 currentMouseDeltaVelocity = Vector2.zero;
    //the angle at which the camera is currently oriented 
    private float cameraPitch = 0f;
    //reference to the character controller
    private CharacterController CharCon;

    #region Non-Serialized Movement variables
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
    #endregion

    #region Non-Serialized Arrow variables 
    private float currentArrowForce;
    //
    private bool shotPressed = false;
    //
    private GameObject CurrentArrow;
    //
    private LineRenderer lr;
    //
    private Health healthSystem;
    //
    private AmmoInventory ammoSystem;
    //
    private bool isGrappled;
    //
    private Vector3 BoostVector;

    #endregion

    private void Awake()
    {
        healthSystem = GetComponent<Health>(); //Needs to link immediately to not break HUD -BMH
        ammoSystem = GetComponent<AmmoInventory>(); //Needs to link immediately to not break HUD - BMH
    }

    // Start is called before the first frame update
    void Start()
    {
        //set all variables
        CharCon = GetComponent<CharacterController>();
        lr = GetComponent<LineRenderer>();
        //hide mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentGravity = gravityForce;
        yVelocity = 0f;
        currentArrowForce = 0f;
        CurrentArrow = null;
        isGrappled = false;
        BoostVector = Vector3.zero;
        
    }


    // Update is called once per frame
    void Update()
    {
        UpdateGround();//check if on the groung
        UpdateCameraRotation();//check if moving mouse
        UpdateMovement();//check if moving

        //DrawArrowProjection();
        
    }
    private void FixedUpdate()
    {
        ApplyGravity();//every fixed frame apply gravity
        CheckGrappleArrow();//check if the player is grappled
    }

    #region movement
    /// <summary>
    /// this script handles the line renderer attached to the grappling arrow from the player
    /// then if a grappling arrow is out it checks if the arrow has collided with something
    /// once the arrow has collided with something the player will rapidly  move towards it
    /// </summary>
    private void CheckGrappleArrow()
    {
        lr.SetPosition(0, transform.position);//starting points for line renderer
        lr.SetPosition(1, transform.position);
        if (CurrentArrow != null && CurrentArrow.GetComponent<ArrowBehaviour>().GetArrowType() == 1)//is grapple arrow out
        {
            lr.SetPosition(0, ShootFrom.position);//draw line to arrow
            lr.SetPosition(1, CurrentArrow.transform.position);
            if (CurrentArrow.GetComponent<ArrowBehaviour>().GetCollided())//AND NOT COLLIDED WITH YET 
            {
                isGrappled = true;

                Vector3 offset = CurrentArrow.transform.position - transform.position;

                if (offset.magnitude > .1f)
                {
                    offset = offset.normalized * maxGrappleSpeed;
                    CharCon.Move(offset * Time.deltaTime);//move towards thearrow
                }

                //transform.position = Vector3.MoveTowards(transform.position, CurrentArrow.transform.position, maxGrappleSpeed);
            }
        }
    }


    /// <summary>
    /// this creates a sphere below the player to check if the player is on the floor
    /// if the player is on the ground then the ground variable will be set to True
    /// otherwise it's set to False
    /// if the object is grounded it will tell the gamecontroller the spot it's at 
    /// </summary>
    private void UpdateGround()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundOffset, transform.position.z);
        grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        if (grounded)
        {
            GameObject.Find("GameController").GetComponent<GameController>().SetLastPlayerPosition(transform.position);
        }
        
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
    /// This method handles all movement that affects the player in the XYZ planes
    /// calculates velocity and direction then applies it to the playercontroller 
    /// </summary>
    private void UpdateMovement()
    {
        currentDir = Vector3.SmoothDamp(currentDir, moveInput, ref currentDirVelocity, mouseSmoothTime);//the direction of the player 
        Vector3 velocity = ((transform.forward * (currentDir.y + BoostVector.z)) + (transform.right * (currentDir.x + BoostVector.x))) * speed + (Vector3.up * (yVelocity + BoostVector.y));//the speed at which the player is moving
        Physics.SyncTransforms();
        CharCon.Move(velocity * Time.deltaTime);//move in relation to time, direction, and speed
    }

    /// <summary>
    /// this script adds or resets gravity based on where the player is in relation to the ground
    /// </summary>
    private void ApplyGravity()
    {
        if (grounded && !isJumping || isGrappled)//if not jumping or is on the ground then reset gravity forces
        {
            yVelocity = 0;
            BoostVector = Vector3.zero;
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
        if(grounded || isGrappled)//if not already jumping
        {
            if (isGrappled)// if grappled end grapple and jump midair
            {
                PullPlayer(CurrentArrow.transform);
                //add forward force
                CurrentArrow = null;
                isGrappled = false;
            }
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
    /// This method will be called if the player jumps while grappled.
    /// This script launches the player in the direction they're looking 
    /// </summary>
    /// <param name="pullPoint"></param>
    public void PullPlayer(Transform pullPoint)
    {
        if (pullPoint.position.y + 2f > transform.position.y)// if they're too close it won't work
        {
            yVelocity = 0f;//reset gravity
            BoostVector.y = Mathf.Sqrt(((pullPoint.position.y + GrappleLaunchOffset.x) - transform.position.y) * -GrappleLaunchForce * gravityForce);//find the disitance in a straight line between the player and the pull point
            BoostVector.z = GrappleLaunchOffset.y;//how quick
        }
    }


    #endregion

    #region shooting
    /// <summary>
    /// This will call the basic arrow shot and load it
    /// </summary>
    /// <param name="value">The button pressed by the player</param>
    private void OnPrimaryShoot(InputValue value)
    {
        LoadArrow(value, 0);
    }

    /// <summary>
    /// this will call the seconday arrow shot and load it
    /// </summary>
    /// <param name="value">The button pressed by the player</param>
    private void OnSecondaryShoot(InputValue value)
    {
        LoadArrow(value,1);
    }

    /// <summary>
    /// This method will call the charge shot coroutine when an arrow isn't already knocked
    /// then if the arrow is knocked already, it will create and shoot an arrow
    /// </summary>
    /// <param name="value">The button pressed by the player</param>
    /// <param name="arrowNum">0 = normal, 1 = grapple</param>
    private void LoadArrow(InputValue value, int arrowNum)
    {
        if (value.isPressed)// if the arrow is  pressed and has ammo left
        {
            if(!ammoSystem.QuiverIsEmpty())
            {
                shotPressed = true;
                StartCoroutine(chargeShot()); //start charging the arrow
            }
        }
        else//if the arrow is pressed and has ammo elft
        {                
            updateCharge?.Invoke(0);
            if(!ammoSystem.QuiverIsEmpty())
            {
                shotPressed = false;
                isGrappled = false;//shoot the arrow
                CurrentArrow = Instantiate(Arrow, ShootFrom.position,CameraRef.rotation);
                CurrentArrow.GetComponent<Rigidbody>().velocity = CameraRef.forward  * (currentArrowForce * maxArrowForce);
                CurrentArrow.GetComponent<ArrowBehaviour>().setArrowType(arrowNum);
                currentArrowForce = 0f;
                ammoSystem.PullFromQuiver();
                updateAmmo?.Invoke(ammoSystem.GetArrowCount());

                //need to get rid of this after I move path prediction into HUD properly -BMH
                arrowPathPredictionLineRenderer.enabled = false;
            }
        }
    }

    /// <summary>
    /// while the shot is pressed, add the time held down until it reaches a ceiling
    /// this float will be used to modify the arrow's speed
    /// </summary>
    /// <returns></returns>
    public IEnumerator chargeShot()
    {
        while(shotPressed)
        {
            if(currentArrowForce < 1f)//ceiling
            {
                currentArrowForce += Time.deltaTime;
                updateCharge?.Invoke(currentArrowForce);
            }

            //will turn this into an invocation after I pull out the arrow path rendering code into its own script -BMH
            DrawArrowProjection();


            yield return new WaitForSeconds(.01f);
        }
    }
    #endregion

    #region pickups
    /// <summary>
    /// function to process a healing request. Returns a bool to indicate whether health was added
    /// </summary>
    /// <returns></returns>
    public bool AddHealth(int healthAdd)
    {
        if (this.healthSystem.isFull())
        {
            return false;
        }
        else
        {
            this.healthSystem.heal(healthAdd);
            updateHealth?.Invoke(healthSystem.getCurrentHealth());
            return true;
        }
    }

    /// <summary>
    /// function to process an ammo reload request. Returns a boolean to indicate whether ammo was added
    /// </summary>
    /// <returns></returns>
    public bool AddAmmo(int ammoAdd)
    {
        bool addArrowStatus = ammoSystem.AddToQuiver(ammoAdd);
        updateAmmo?.Invoke(ammoSystem.GetArrowCount());
        return addArrowStatus;
    }

    #endregion

    #region collisions
    /// <summary>
    /// 
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        
    }
    #endregion

    #region gizmos & visualizations
    //need to offload this to a separate script in the near term. BMH
    [Header("Arrow Path Prediction Variables")]
    [Tooltip("Mask to specify raycast ignoring")]
    [SerializeField]
    private LayerMask ArrowColliderMask;
    [Tooltip("Preferred line renderer for visual path of arrow")]
    [SerializeField]
    private LineRenderer arrowPathPredictionLineRenderer;
    [Tooltip("Arrow instantiation point (for purposes of start of flight path)")]
    [SerializeField]
    private Transform releasePosition;
    [Tooltip("How many maximum points used to render line")]
    [SerializeField]
    [Range(10, 100)]
    private int linePoints = 25;
    [Tooltip("time distance between line segments - smaller = smoother")]
    [SerializeField]
    [Range(0.01f, 0.25f)]
    private float timeBetweenPoints = 0.025f;

    /// <summary>
    /// Draws an arrow projection on request.
    /// </summary>
    private void DrawArrowProjection()
    {
        //checks to see if we should even be fuddling with rendering
        if (currentArrowForce > 0)
        {
            //sets starting position of the predictive path line vector
            arrowPathPredictionLineRenderer.enabled = true;
            arrowPathPredictionLineRenderer.positionCount = Mathf.CeilToInt(linePoints / timeBetweenPoints) + 1;
            Vector3 startPosition = ShootFrom.position;
            Vector3 startVelocity = CameraRef.forward * (currentArrowForce * maxArrowForce);
            int i = 0;
            arrowPathPredictionLineRenderer.SetPosition(i, startPosition);

            //runs forward, generating straight line segments forming a ballistic arc
            for (float time = 0; time < linePoints; time += timeBetweenPoints)
            {
                i++;
                Vector3 point = startPosition + time * startVelocity;
                point.y = startPosition.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);
                arrowPathPredictionLineRenderer.SetPosition(i, point);

                //checks to see if we're hitting a collidable object, if so, will stop this process
                Vector3 lastPostition = arrowPathPredictionLineRenderer.GetPosition(i - 1);
                if (Physics.Raycast(lastPostition, (point - lastPostition).normalized, out RaycastHit hit, (point - lastPostition).magnitude, ArrowColliderMask))
                {
                    arrowPathPredictionLineRenderer.SetPosition(i, hit.point);
                    arrowPathPredictionLineRenderer.positionCount = i + 1;
                    return;
                }
            }
        }
        else
        {
            arrowPathPredictionLineRenderer.enabled = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - groundOffset, transform.position.z), GroundedRadius);

    }
    #endregion

    #region Getters
    public int getMaxHealth() { return this.healthSystem.getMaximumHealth(); }
    public int getCurrHealth() { return this.healthSystem.getCurrentHealth(); }
    public int getCurrAmmo() { return this.ammoSystem.GetArrowCount(); }
    public int getMaxAmmo() { return this.ammoSystem.GetQuiverSize(); }
    #endregion
}