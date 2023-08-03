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
    public delegate void HealthUpdate(float newHealth);
    public static event HealthUpdate updateHealth;

    public delegate void AmmoUpdate(int newAmmo);
    public static event AmmoUpdate updateAmmo;

    public delegate void ChargeUpdate(float newCharge);
    public static event ChargeUpdate updateCharge;
    #endregion

    #region Variables (all private)

    //Camera Variables
    [Header("Camera Variables")]
    [Tooltip("Reference to the transform of the camera that is attached to the player")]
        [SerializeField] private Transform _CameraRef;
    [Tooltip("How fast the camera can move using the mouse")]
        [SerializeField] private float _MouseSensitivity = 3.5f; //default value
    [Tooltip("The amount of smoothing when the player starts and stops looking around with the mouse")]
        [SerializeField][Range(0.0f, 0.5f)] float _MouseSmoothTime = 0.03f; //default value


    [Header("Movement")]
    [Tooltip("How fast the player can move")]
        [SerializeField] private float _PlayerSpeed = 5.0f; //default value
    [Tooltip("how strong the player jump is")]
        [SerializeField] private float _PlayerJumpForce = 10.0f; //default value
    [Tooltip("How strong the gravity that affects the player is")]
        [SerializeField] private float _PlayerGravityForce = -15.0f; //default value
    [Tooltip("The multiplier added when the player presses shift")]
        [SerializeField] private float _SpeedMultiplier = 3.0f; //default value 
    // Movement
    //the vector passed by the player input 
    private Vector2 _MoveInput;
    //how fast and in what direction the player is moving on the y axis 
    //private float velocityY = 0f;
    //the direction the player moves on the x/z axis
    private Vector2 _CurrentDir = Vector2.zero;
    //the speed the player moves at on the x/z axis
    private Vector3 _CurrentDirVelocity = Vector2.zero;
    //the absolute fastest the player can move on the y axis
    private float _MaxSpeed = 53.0f;
    //how much gravity force is currently affecting the player
    private float _CurrentGravity;
    //how fast and in what direction the player is moving on the y axis 
    private float _PlayerVelocityY;
    //is the player currently on the ground
    private bool _bGrounded = false;
    //keeps track of if the player is jumping
    private bool _bIsJumping = false;

    [Header("Ground Checking")]
    [Tooltip("The difference between the ground and the player")]
        [SerializeField] private float _GroundOffset = 0.8f; //default value
    [Tooltip("The layermask the ground is on")]
        [SerializeField] private LayerMask _GroundLayers; //set by default to "default" in case of reset"
    [Tooltip("The radius the ground will check out. best practice is to keep it at most the radius of the character controller")]
        [SerializeField] private float _GroundedRadius = 0.3f; //default value

    [Tooltip("How strong your arrow can be shot")]
        [SerializeField] private float _MaxArrowForce = 20.0f; //default value
    [Tooltip("Reference to the arrow prefab")]
        [SerializeField] private GameObject _Arrow; //make reference to arrow prefab
    [Tooltip("Reference to the grapple arrow prefab")]
        [SerializeField] private GameObject _GrappleArrow; //make reference to grapple arrow prefab
    [Tooltip("where the arrow will come out")]
        [SerializeField] private Transform _ShootFrom; //reference to "ShootPos" component in Player->MainCamera->Bow_01
    [Tooltip("How much distance the player can cover while grappled")]
        [SerializeField] private float _MaxGrappleSpeed = 10.0f; //default value
    [Tooltip("How far off from the arrow that the player will be launched up and pushed forward. x will change the y and y will change the z")]
        [SerializeField] private Vector2 _GrappleLaunchOffset; //set to x @ 0.25f & y @ 1 at this time
    [Tooltip("This will affect how strong the jump is when you launch off a grapple")]
        [SerializeField, Range(0, 10)] private float _GrappleLaunchForce = 1.75f; //default value


    //where the mouse currently is on the screen
    private Vector2 _CurrentMouseDelta = Vector2.zero;
    //how fast the mouse is currently moving
    private Vector2 _CurrentMouseDeltaVelocity = Vector2.zero;
    //the angle at which the camera is currently oriented 
    private float _CameraPitch = 0f;
    //reference to the character controller
    private CharacterController _CharCon;

    //need to offload this to a separate script in the near term. BMH
    [Header("Arrow Path Prediction Variables")]
    [Tooltip("Mask to specify raycast ignoring")]
        [SerializeField] private LayerMask _ArrowColliderMask; //mask set to "Default" Layer only
    [Tooltip("Preferred line renderer for visual path of arrow")]
        [SerializeField] private LineRenderer _ArrowPathPredictionLR; //set to bow prefab LR
    [Tooltip("Arrow instantiation point (for purposes of start of flight path)")]
        [SerializeField] private Transform _ReleasePosition; // set to ShootPos object in Player->MainCam
    [Tooltip("How many maximum points used to render line")]
        [SerializeField] [Range(10, 100)] private int _LinePoints = 25;
    [Tooltip("time distance between line segments - smaller = smoother")]
        [SerializeField] [Range(0.01f, 0.25f)] private float _TimeBetweenPoints = 0.05f;

    private float _CurrentArrowForce;
    //
    private bool _bShotPressed = false;
    //
    private GameObject _CurrentArrow;
    //
    private GameObject _CurrGrappleArrow;
    //
    private LineRenderer _GrappleLineRenderer;
    //
    private Health _HealthSystem;
    //
    private AmmoInventory _AmmoSystem;
    //
    private bool _bIsGrappled;
    //
    private Vector3 _BoostVector;

    #endregion



    private void Awake()
    {
        _HealthSystem = GetComponent<Health>(); //Needs to link immediately to not break HUD -BMH
        _AmmoSystem = GetComponent<AmmoInventory>(); //Needs to link immediately to not break HUD - BMH
    }



    // Start is called before the first frame update
    void Start()
    {
        //set all variables
        _CharCon = GetComponent<CharacterController>();
        _GrappleLineRenderer = GetComponent<LineRenderer>();
        //hide mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //CONSIDER removing _CurrentGravity as it's unused. Unless we were thinking of using it to store default val -BMH
        //_CurrentGravity = _PlayerGravityForce;
        _PlayerVelocityY = 0f;
        _CurrentArrowForce = 0f;
        _CurrentArrow = null;
        _CurrGrappleArrow = null;
        _bIsGrappled = false;
        _BoostVector = Vector3.zero;
    }



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
        _GrappleLineRenderer.SetPosition(0, transform.position);//starting points for line renderer
        _GrappleLineRenderer.SetPosition(1, transform.position);
        if (_CurrGrappleArrow != null)//is grapple arrow out
        {
            _GrappleLineRenderer.SetPosition(0, _ShootFrom.position);//draw line to arrow
            _GrappleLineRenderer.SetPosition(1, _CurrGrappleArrow.transform.position);
            if (_CurrGrappleArrow.GetComponent<ArrowBehaviour>().GetCollided())//AND NOT COLLIDED WITH YET 
            {
                _bIsGrappled = true;

                Vector3 Offset = _CurrGrappleArrow.transform.position - transform.position;

                if (Offset.magnitude > .1f)
                {
                    Offset = Offset.normalized * _MaxGrappleSpeed;
                    _CharCon.Move(Offset * Time.deltaTime);//move towards thearrow
                }

                //transform.position = Vector3.MoveTowards(transform.position, CurrentArrow.transform.position, maxGrappleSpeed);
            }
        }
    }



    /// <summary>
    /// Creates a sphere below the player to check if the player is on the floor
    /// if the player is on the ground then the ground variable will be set to True
    /// otherwise it's set to False
    /// if the object is grounded it will tell the gamecontroller the spot it's at 
    /// </summary>
    private void UpdateGround()
    {
        Vector3 SpherePosition = new Vector3(transform.position.x, transform.position.y - _GroundOffset, transform.position.z);
        _bGrounded = Physics.CheckSphere(SpherePosition, _GroundedRadius, _GroundLayers, QueryTriggerInteraction.Ignore);

        if (_bGrounded)
        {
            GameObject.Find("GameController").GetComponent<GameController>().SetLastPlayerPosition(transform.position);
        }
    }



    /// <summary>
    /// Calculates the next position of the camera based on the mouse's movement 
    /// </summary>
    private void UpdateCameraRotation()
    {
        //get the mouse's position,desired end spot, and the player's input
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        _CurrentMouseDelta = Vector2.SmoothDamp(_CurrentMouseDelta, targetMouseDelta, ref _CurrentMouseDeltaVelocity, _MouseSmoothTime);
        //calculate where the camera can move next
        _CameraPitch -= _CurrentMouseDelta.y * _MouseSensitivity;
        _CameraPitch = Mathf.Clamp(_CameraPitch, -90f, 90f);
        _CameraRef.localEulerAngles = Vector3.right * _CameraPitch;
        //rotate the camera towards the calculated rotation
        transform.Rotate(Vector3.up * _CurrentMouseDelta.x * _MouseSensitivity);
    }



    /// <summary>
    /// This method handles all movement that affects the player in the XYZ planes
    /// calculates velocity and direction then applies it to the playercontroller 
    /// </summary>
    private void UpdateMovement()
    {
        _CurrentDir = Vector3.SmoothDamp(_CurrentDir, _MoveInput, ref _CurrentDirVelocity, _MouseSmoothTime);//the direction of the player 
        Vector3 velocity = ((transform.forward * (_CurrentDir.y + _BoostVector.z)) + (transform.right * (_CurrentDir.x + _BoostVector.x))) * _PlayerSpeed + (Vector3.up * (_PlayerVelocityY + _BoostVector.y));//the speed at which the player is moving
        Physics.SyncTransforms();
        _CharCon.Move(velocity * Time.deltaTime);//move in relation to time, direction, and speed        
    }



    /// <summary>
    /// this script adds or resets gravity based on where the player is in relation to the ground
    /// </summary>
    private void ApplyGravity()
    {
        if (_bGrounded && !_bIsJumping || _bIsGrappled)//if not jumping or is on the ground then reset gravity forces
        {
            _PlayerVelocityY = 0;
            _BoostVector = Vector3.zero;
        }
        else//otherwise add the gravity that affects the player
        {
            _PlayerVelocityY += _PlayerGravityForce * Time.deltaTime;
        }
    }



    /// <summary>
    /// sets the movement input to the vector that determines what direction the player
    /// is moving in
    /// </summary>
    /// <param name="value">the vector that the player must move in</param>
    private void OnMovement(InputValue value)
    {
        _MoveInput = value.Get<Vector2>();
    }



    /// <summary>
    /// adds a force to the player to move them up 
    /// </summary>
    /// <param name="value">the input of the player</param>
    private void OnJump(InputValue value)
    {
        if (_bGrounded || _bIsGrappled)//if not already jumping
        {
            if (_bIsGrappled)// if grappled end grapple and jump midair
            {
                PullPlayer(_CurrGrappleArrow.transform);
                //add forward force
                _CurrGrappleArrow = null;
                _bIsGrappled = false;
            }
            _bIsJumping = true;
            _PlayerVelocityY = _PlayerJumpForce;
            StartCoroutine(tickDownJump());//prevents desynchronization of jumping
        }
    }



    /// <summary>
    /// turns spprinting on and off when the player presses the sprint button
    /// </summary>
    /// <param name="value">a bool that determines if the button is being pressed or unpressed</param>
    private void OnSprint(InputValue value)
    {
        if (value.isPressed)//add sprint
        {
            _PlayerSpeed *= _SpeedMultiplier;
        }
        else//remove sprint
        {
            _PlayerSpeed /= _SpeedMultiplier;
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
        _bIsJumping = false;
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
            _PlayerVelocityY = 0f;//reset gravity
            _BoostVector.y = Mathf.Sqrt(((pullPoint.position.y + _GrappleLaunchOffset.x) - transform.position.y) * -_GrappleLaunchForce * _PlayerGravityForce);//find the disitance in a straight line between the player and the pull point
            _BoostVector.z = _GrappleLaunchOffset.y;//how quick
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
        LoadGrappleArrow(value);
    }



    /// <summary>
    /// This method will call the charge shot coroutine when an arrow isn't already knocked
    /// then if the arrow is knocked already, it will create and shoot an arrow
    /// </summary>
    /// <param name="value">The button pressed by the player</param>
    /// <param name="arrowNum">0 = normal, 1 = grapple</param>
    private void LoadArrow(InputValue value, int arrowNum)
    {
        if (value.isPressed && !_bShotPressed)// if the arrow is  pressed and has ammo left
        {
            if (!_AmmoSystem.QuiverIsEmpty())
            {
                _bShotPressed = true;
                StartCoroutine(chargeShot()); //start charging the arrow
            }
        }
        else//if the arrow is pressed and has ammo elft
        {
            updateCharge?.Invoke(0);
            if (!_AmmoSystem.QuiverIsEmpty())
            {
                _bShotPressed = false;
                //_bIsGrappled = false;//shoot the arrow
                _CurrentArrow = Instantiate(_Arrow, _ShootFrom.position, _CameraRef.rotation);
                _CurrentArrow.GetComponent<Rigidbody>().velocity = _CameraRef.forward * (_CurrentArrowForce * _MaxArrowForce);
                _CurrentArrow.GetComponent<ArrowBehaviour>().setArrowType(arrowNum);
                _CurrentArrowForce = 0f;
                _AmmoSystem.PullFromQuiver();
                updateAmmo?.Invoke(_AmmoSystem.GetArrowCount());

                //need to get rid of this after I move path prediction into HUD properly -BMH
                _ArrowPathPredictionLR.enabled = false;
            }
        }
    }



    private void LoadGrappleArrow(InputValue value)
    {
        if (value.isPressed && !_bShotPressed)// if the arrow is  pressed and has ammo left
        {
            _bShotPressed = true;
            StartCoroutine(chargeShot()); //start charging the arrow
        }
        else//if the arrow is pressed and has ammo elft
        {
            updateCharge?.Invoke(0);
            _bShotPressed = false;
            _bIsGrappled = false;//shoot the arrow
            _CurrGrappleArrow = Instantiate(_GrappleArrow, _ShootFrom.position, _CameraRef.rotation);
            _CurrGrappleArrow.GetComponent<Rigidbody>().velocity = _CameraRef.forward * (_CurrentArrowForce * _MaxArrowForce);
            _CurrGrappleArrow.GetComponent<ArrowBehaviour>().setArrowType(1);
            _CurrentArrowForce = 0f;

            //need to get rid of this after I move path prediction into HUD properly -BMH
            _ArrowPathPredictionLR.enabled = false;
        }
    }



    /// <summary>
    /// while the shot is pressed, add the time held down until it reaches a ceiling
    /// this float will be used to modify the arrow's speed
    /// </summary>
    /// <returns></returns>
    public IEnumerator chargeShot()
    {
        while (_bShotPressed)
        {
            if (_CurrentArrowForce < 1f)//ceiling
            {
                _CurrentArrowForce += Time.deltaTime;
                updateCharge?.Invoke(_CurrentArrowForce);
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
        if (this._HealthSystem.isFull())
        {
            return false;
        }
        else
        {
            this._HealthSystem.heal(healthAdd);
            updateHealth?.Invoke(_HealthSystem.getCurrentHealth());
            return true;
        }
    }



    /// <summary>
    /// function to process an ammo reload request. Returns a boolean to indicate whether ammo was added
    /// </summary>
    /// <returns></returns>
    public bool AddAmmo(int ammoAdd)
    {
        bool addArrowStatus = _AmmoSystem.AddToQuiver(ammoAdd);
        updateAmmo?.Invoke(_AmmoSystem.GetArrowCount());
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

    /// <summary>
    /// Draws an arrow projection on request.
    /// </summary>
    private void DrawArrowProjection()
    {
        //checks to see if we should even be fuddling with rendering
        if (_CurrentArrowForce > 0)
        {
            //sets starting position of the predictive path line vector
            _ArrowPathPredictionLR.enabled = true;
            _ArrowPathPredictionLR.positionCount = Mathf.CeilToInt(_LinePoints / _TimeBetweenPoints) + 2;
            Vector3 startPosition = _ShootFrom.position;
            Vector3 startVelocity = _CameraRef.forward * (_CurrentArrowForce * _MaxArrowForce);
            int i = 0;
            _ArrowPathPredictionLR.SetPosition(i, startPosition);

            //runs forward, generating straight line segments forming a ballistic arc
            for (float time = 0; time < _LinePoints; time += _TimeBetweenPoints)
            {
                i++;
                Vector3 point = startPosition + time * startVelocity;
                point.y = startPosition.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);
                _ArrowPathPredictionLR.SetPosition(i, point);

                //checks to see if we're hitting a collidable object, if so, will stop this process
                Vector3 lastPostition = _ArrowPathPredictionLR.GetPosition(i - 1);
                if (Physics.Raycast(lastPostition, (point - lastPostition).normalized, out RaycastHit hit, (point - lastPostition).magnitude, _ArrowColliderMask))
                {
                    _ArrowPathPredictionLR.SetPosition(i, hit.point);
                    _ArrowPathPredictionLR.positionCount = i + 1;
                    return;
                }
            }
        }
        else
        {
            _ArrowPathPredictionLR.enabled = false;
        }
    }



    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - _GroundOffset, transform.position.z), _GroundedRadius);

    }
    #endregion

    #region Getters
    public float getMaxHealth() { return this._HealthSystem.getMaximumHealth(); }
    public float getCurrHealth() { return this._HealthSystem.getCurrentHealth(); }
    public int getCurrAmmo() { return this._AmmoSystem.GetArrowCount(); }
    public int getMaxAmmo() { return this._AmmoSystem.GetQuiverSize(); }
    #endregion
}