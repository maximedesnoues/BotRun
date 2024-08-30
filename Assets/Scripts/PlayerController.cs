using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Movement-related settings
    [Header("Movement Settings")]
    [SerializeField] private float maxMoveSpeed; // Maximum speed the player can move
    [SerializeField] private float accelerationRate; // Rate at which the player accelerates
    [SerializeField] private float rotationSpeed; // Speed of player rotation

    // Jump-related settings
    [Header("Jump Settings")]
    [SerializeField] private float initialJumpForce; // Initial force applied when jumping
    [SerializeField] private float maxJumpForce; // Maximum jump force
    [SerializeField] private float jumpForceIncreaseRate; // Rate at which jump force increases when holding the jump button

    // Climbing-related settings
    [Header("Climb Settings")]
    [SerializeField] private float climbDetectionDistance; // Distance at which climbable surfaces are detected
    [SerializeField] private float climbSpeed; // Speed at which the player climbs

    // Rolling-related settings
    [Header("Roll Settings")]
    [SerializeField] private float rollForce; // Force applied when rolling in the air

    // Wall run-related settings
    [Header("Wall Run Settings")]
    [SerializeField] private float wallRunDetectionDistance; // Distance at which walls are detected for wall running
    [SerializeField] private float wallRunSpeed; // Speed during wall run
    [SerializeField] private float wallRunDuration; // Duration of the wall run

    // Ground check settings
    [Header("Ground Check Settings")]
    [SerializeField] private float groundCheckDistance; // Distance for ground detection
    [SerializeField] private LayerMask groundLayer; // Layer used to determine what counts as ground

    // Stumble-related settings
    [Header("Stumble Settings")]
    [SerializeField] private float stumbleDuration; // Duration of the stumble animation
    [SerializeField] private float stumbleSpeedMultiplier; // Speed multiplier when stumbling

    // MiniMap settings
    [Header("MiniMap Settings")]
    [SerializeField] private GameObject miniMapCamera; // Reference to the minimap camera

    // Animation names for stumbling
    [Header("Stumble Animations")]
    [SerializeField] private string stumbleLeftAnimation = "StumbleLeft"; // Animation for stumbling to the left
    [SerializeField] private string stumbleRightAnimation = "StumbleRight"; // Animation for stumbling to the right

    // Component references
    private CapsuleCollider capsuleCollider; // Reference to the capsule collider
    private Rigidbody rb; // Reference to the player's rigidbody
    private Animator animator; // Reference to the animator component
    private BoostManager boostManager; // Reference to the boost manager
    private Timer timer; // Reference to the timer
    private ScoreManager scoreManager; // Reference to the score manager
    private LevelComplete levelComplete; // Reference to the level completion handler
    private Controls controls; // Reference to the controls manager

    // State flags
    private bool isGrounded; // Flag indicating if the player is on the ground
    private bool isJumping; // Flag indicating if the player is currently jumping
    private bool isClimbing; // Flag indicating if the player is currently climbing
    private bool isAirRolling; // Flag indicating if the player is currently rolling in the air
    private bool isWallRunning; // Flag indicating if the player is currently wall running
    private bool isNearClimbable; // Flag indicating if the player is near a climbable surface
    private bool isNearWall; // Flag indicating if the player is near a wall for wall running
    private bool hasRolledInAir; // Flag indicating if the player has already rolled in the air during this jump
    private bool isStumbling = false; // Flag indicating if the player is currently stumbling
    private bool isEndClimbAnimationPlaying = false; // Flag indicating if the end climb animation is playing
    private bool controlsEnabled = true; // Flag indicating if the controls are enabled

    // Various timing and force-related variables
    private float jumpPressTime; // Time for which the jump button is held
    private float currentJumpForce; // Current force applied during jump
    private float wallRunTime; // Time for which the player has been wall running
    private float boostEndTime; // Time when the speed boost will end
    private float currentSpeedBoostMultiplier = 1f; // Multiplier for the player's speed during a boost

    // Movement input timing
    private float moveForwardPressTime; // Time for which the move forward key is held
    private float moveBackwardPressTime; // Time for which the move backward key is held
    private BuildingProperties currentBuildingProperties; // Reference to the properties of the currently interacted building

    // Start is called before the first frame update
    private void Start()
    {
        // Initialize component references
        capsuleCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        boostManager = FindObjectOfType<BoostManager>();
        timer = FindObjectOfType<Timer>();
        scoreManager = FindObjectOfType<ScoreManager>();
        levelComplete = FindObjectOfType<LevelComplete>();
        controls = FindObjectOfType<Controls>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Disable controls if controls are not enabled
        if (!controlsEnabled)
        {
            return;
        }

        // Update various states based on player position and input
        UpdateGroundStatus();
        UpdateWallDetection();
        UpdateClimbDetection();

        // Handle movement and actions if player is not performing special actions
        if (!isClimbing && !isAirRolling && !isWallRunning && !isEndClimbAnimationPlaying)
        {
            HandleMovement();
            HandleJump();
        }

        // Handle specific actions like climbing and wall running
        HandleClimb();
        HandleWallRun();

        // Check if player has reached the top of a climbable surface
        if (currentBuildingProperties != null && isClimbing)
        {
            CheckIfReachedTop();
        }

        // Handle boost activation
        UpdateBoostStatus();

        // Activate speed boost if boost key is pressed and boost is full
        if (Input.GetKeyDown(controls.controls["Boost"]) && boostManager.IsBoostFull())
        {
            ActivateSpeedBoost(GameManager.Instance.speedBoostMultiplier, GameManager.Instance.boostDuration);
            boostManager.ConsumeBoost();
        }

        // Toggle the minimap when the minimap key is pressed
        if (Input.GetKeyDown(controls.controls["MiniMap"]))
        {
            ToggleMiniMap();
        }

        // Debugging rays for climb and wall run detection
        Debug.DrawRay(transform.position + Vector3.up * 1.5f, transform.forward * climbDetectionDistance, Color.green);
        Debug.DrawRay(transform.position, transform.right * wallRunDetectionDistance, Color.blue);
        Debug.DrawRay(transform.position, -transform.right * wallRunDetectionDistance, Color.blue);
    }

    // FixedUpdate is called every fixed framerate frame
    private void FixedUpdate()
    {
        // Disable controls if controls are not enabled
        if (!controlsEnabled)
        {
            return;
        }

        // Handle rotation and other actions in fixed update
        if (!isClimbing && !isAirRolling && !isWallRunning && !isEndClimbAnimationPlaying)
        {
            HandleRotation();
        }

        // Update the position of the capsule collider to match the player model
        UpdateCapsuleColliderPosition();
    }

    // Triggered when the player enters a trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // Handle actions when player reaches a destination
        if (other.CompareTag("Destination"))
        {
            scoreManager.AddTimeScore(CalculateTimeScore());
            ShowLevelCompleteScreen();
        }
    }

    // Enable player controls
    public void EnableControls()
    {
        controlsEnabled = true;
    }

    // Disable player controls and reset velocity
    public void DisableControls()
    {
        controlsEnabled = false;
        rb.velocity = Vector3.zero;
    }

    // Update the capsule collider position based on the player's animation
    private void UpdateCapsuleColliderPosition()
    {
        if (animator)
        {
            capsuleCollider.center = animator.bodyPosition - transform.position;
        }
    }

    // Display the level completion screen with relevant stats
    private void ShowLevelCompleteScreen()
    {
        int totalScore = scoreManager.GetTotalScore();
        float elapsedTime = timer.elapsedTime;
        int tricks = scoreManager.GetTricksPerformed();
        string objectsCollected = scoreManager.GetCollectablesCollected().ToString();

        levelComplete.ShowLevelComplete(totalScore, elapsedTime, tricks, objectsCollected);
    }

    // Check if the player is on the ground
    private void UpdateGroundStatus()
    {
        isGrounded = CheckIfGrounded();

        if (isGrounded)
        {
            hasRolledInAir = false;

            RaycastHit hit;
            
            // Perform a raycast slightly above the player's position to detect the ground
            if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, groundCheckDistance + 0.1f, groundLayer))
            {
                float groundY = hit.point.y;
                float playerY = transform.position.y;

                // Apply a small tolerance to avoid frequent adjustments
                float tolerance = 0.1f;

                // Correct the player's position only if they are significantly below the ground level
                if (playerY < groundY - tolerance)
                {
                    transform.position = new Vector3(transform.position.x, groundY, transform.position.z);
                    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Reset vertical velocity to prevent bouncing
                }
            }
        }
    }

    // Use a raycast to check if the player is touching the ground
    private bool CheckIfGrounded()
    {
        Vector3 raycastOrigin = transform.position + Vector3.up * 0.2f;
        float raycastDistance = groundCheckDistance;
        RaycastHit hit;

        bool isHit = Physics.Raycast(raycastOrigin, Vector3.down, out hit, raycastDistance, groundLayer);
        Debug.DrawRay(raycastOrigin, Vector3.down * raycastDistance, Color.red);

        return isHit;
    }

    // Check if the player is near a climbable surface
    private void UpdateClimbDetection()
    {
        isNearClimbable = false;

        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 forward = transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(origin, forward, out RaycastHit hit, climbDetectionDistance))
        {
            BuildingProperties buildingProperties = hit.collider.GetComponent<BuildingProperties>();

            if (buildingProperties != null && buildingProperties.isClimbable)
            {
                isNearClimbable = true;
                currentBuildingProperties = buildingProperties;
            }
        }
    }

    // Check if the player is near a wall that can be used for wall running
    private void UpdateWallDetection()
    {
        isNearWall = false;

        if (Physics.Raycast(transform.position, transform.right, out RaycastHit hitRight, wallRunDetectionDistance))
        {
            BuildingProperties buildingProperties = hitRight.collider.GetComponent<BuildingProperties>();

            if (buildingProperties != null && buildingProperties.isWallRunnable)
            {
                isNearWall = true;
            }
        }
        else if (Physics.Raycast(transform.position, -transform.right, out RaycastHit hitLeft, wallRunDetectionDistance))
        {
            BuildingProperties buildingProperties = hitLeft.collider.GetComponent<BuildingProperties>();

            if (buildingProperties != null && buildingProperties.isWallRunnable)
            {
                isNearWall = true;
            }
        }
    }

    // Handle player movement based on input
    private void HandleMovement()
    {
        float moveZ = 0;

        // Moving forward
        if (Input.GetKey(controls.controls["MoveForward"]))
        {
            moveForwardPressTime += Time.deltaTime;
            moveZ += Mathf.Clamp(moveForwardPressTime * accelerationRate, 0, 1);
        }
        else
        {
            moveForwardPressTime = 0;
        }

        // Moving backward
        if (Input.GetKey(controls.controls["MoveBackward"]))
        {
            moveBackwardPressTime += Time.deltaTime;
            moveZ -= Mathf.Clamp(moveBackwardPressTime * accelerationRate, 0, 1);
        }
        else
        {
            moveBackwardPressTime = 0;
        }

        // Calculate movement speed
        float speed = isStumbling ? maxMoveSpeed * stumbleSpeedMultiplier : maxMoveSpeed * currentSpeedBoostMultiplier;
        Vector3 move = transform.forward * moveZ * speed;
        rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);

        // Update animations based on movement
        bool isRunning = moveZ != 0;
        animator.SetBool("IsRunning", isRunning);

        // Handle boosting animation
        if (isRunning && currentSpeedBoostMultiplier > 1f)
        {
            animator.SetBool("IsBoosting", true);
        }
        else
        {
            animator.SetBool("IsBoosting", false);
        }
    }

    // Handle player rotation based on input
    private void HandleRotation()
    {
        if (isAirRolling)
        {
            return;
        }

        // Rotate left
        if (Input.GetKey(controls.controls["TurnLeft"]))
        {
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        }
        // Rotate right
        else if (Input.GetKey(controls.controls["TurnRight"]))
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    // Handle jump initiation and continuation
    private void HandleJump()
    {
        // Start jump if grounded and jump key is pressed
        if (isGrounded && Input.GetKeyDown(controls.controls["Jump"]))
        {
            StartJump();
        }

        // Continue jump while jump key is held
        if (isJumping)
        {
            ContinueJump();
        }

        // Handle air roll while in the air
        HandleAirRoll();
    }

    // Start the jump process
    private void StartJump()
    {
        isJumping = true;
        jumpPressTime = 0f;
        currentJumpForce = initialJumpForce;
        rb.velocity = new Vector3(rb.velocity.x, currentJumpForce, rb.velocity.z);
        animator.SetTrigger("Jump");
    }

    // Continue the jump as long as the key is held and conditions are met
    private void ContinueJump()
    {
        float maxJumpDuration = 0.2f;

        if (Input.GetKey(controls.controls["Jump"]) && jumpPressTime < maxJumpDuration)
        {
            jumpPressTime += Time.deltaTime;
            currentJumpForce = Mathf.Min(initialJumpForce + jumpPressTime * jumpForceIncreaseRate, maxJumpForce);
            rb.velocity = new Vector3(rb.velocity.x, currentJumpForce, rb.velocity.z);

            // Adjust jump animation speed based on jump force
            float normalizedJumpForce = currentJumpForce / maxJumpForce;
            float minAnimationSpeed = 0.5f;
            float maxAnimationSpeed = 1.5f;
            float animationSpeed = Mathf.Lerp(maxAnimationSpeed, minAnimationSpeed, normalizedJumpForce);

            animator.SetFloat("JumpSpeed", animationSpeed);
        }

        // Stop jumping if key is released or max jump force/duration is reached
        if (Input.GetKeyUp(controls.controls["Jump"]) || jumpPressTime >= maxJumpDuration || currentJumpForce >= maxJumpForce)
        {
            isJumping = false;
        }
    }

    // Handle climbing initiation and stopping
    private void HandleClimb()
    {
        // Start climbing if climb key is pressed and player is near a climbable surface
        if (Input.GetKey(controls.controls["Climb"]) && isNearClimbable)
        {
            StartClimb();
        }
        else
        {
            StopClimb();
        }
    }

    // Start climbing action
    private void StartClimb()
    {
        isClimbing = true;
        rb.velocity = new Vector3(0, climbSpeed, 0);
        animator.SetBool("IsClimbing", true);
    }

    // Stop climbing action
    private void StopClimb()
    {
        isClimbing = false;
        animator.SetBool("IsClimbing", false);
    }

    // Check if the player has reached the top of the climbable surface
    private void CheckIfReachedTop()
    {
        Vector3 origin = transform.position + Vector3.up * 2.5f;
        Vector3 forward = transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(origin, forward, out RaycastHit hit, climbDetectionDistance))
        {
            BuildingProperties buildingProperties = hit.collider.GetComponent<BuildingProperties>();

            if (buildingProperties == null || !buildingProperties.isClimbable)
            {
                StartEndClimbAnimation();
            }
            else
            {
                animator.SetBool("IsClimbEnding", false);
            }
        }
        else
        {
            StartEndClimbAnimation();
        }
    }

    // Start the end climb animation when reaching the top
    private void StartEndClimbAnimation()
    {
        isClimbing = false;
        isEndClimbAnimationPlaying = true;
        animator.SetBool("IsClimbing", false);
        animator.SetBool("IsClimbEnding", true);
        animator.SetTrigger("ClimbEnd");

        StartCoroutine(EndClimbAnimationCoroutine());
    }

    // Coroutine to handle actions after end climb animation
    private IEnumerator EndClimbAnimationCoroutine()
    {
        yield return new WaitForSeconds(1f);

        Vector3 climbEndForce = transform.forward * 1.5f + transform.up * 0.5f;
        rb.AddForce(climbEndForce, ForceMode.Impulse);

        isEndClimbAnimationPlaying = false;
        animator.SetBool("IsClimbEnding", false);
    }

    // Handle the air roll action during jumps
    private void HandleAirRoll()
    {
        if (isGrounded)
        {
            StopAirRoll();
            return;
        }

        float minHeightForAirRoll = 1.0f;
        if (transform.position.y < minHeightForAirRoll)
        {
            return;
        }

        if (Input.GetKey(controls.controls["Jump"]) && Input.GetAxis("Horizontal") != 0 && !isGrounded && !hasRolledInAir)
        {
            StartAirRoll();
        }
    }

    // Start the air roll action
    private void StartAirRoll()
    {
        isAirRolling = true;
        float direction = Input.GetAxis("Horizontal");

        if (direction > 0)
        {
            animator.SetTrigger("RollRight");
            rb.AddForce(transform.right * rollForce, ForceMode.Impulse);
        }
        else
        {
            animator.SetTrigger("RollLeft");
            rb.AddForce(-transform.right * rollForce, ForceMode.Impulse);
        }

        hasRolledInAir = true;
        scoreManager.AddTrickScore(50);
        boostManager.AddBoost(30f);

        StartCoroutine(StopAirRollAfterDelay(0.5f));
    }

    // Stop the air roll action after a delay
    private IEnumerator StopAirRollAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isAirRolling = false;
    }

    // Immediately stop the air roll action
    private void StopAirRoll()
    {
        isAirRolling = false;
    }

    // Handle wall run initiation and continuation
    private void HandleWallRun()
    {
        // Start wall run if the conditions are met
        if (Input.GetKeyDown(controls.controls["WallRun"]) && animator.GetBool("IsRunning") && isNearWall)
        {
            if (Physics.Raycast(transform.position, transform.right, out RaycastHit hitRight, wallRunDetectionDistance))
            {
                BuildingProperties buildingProperties = hitRight.collider.GetComponent<BuildingProperties>();

                if (buildingProperties != null && buildingProperties.isWallRunnable)
                {
                    StartWallRun(1f);
                }
            }
            else if (Physics.Raycast(transform.position, -transform.right, out RaycastHit hitLeft, wallRunDetectionDistance))
            {
                BuildingProperties buildingProperties = hitLeft.collider.GetComponent<BuildingProperties>();

                if (buildingProperties != null && buildingProperties.isWallRunnable)
                {
                    StartWallRun(0f);
                }
            }
        }

        // Continue wall run if already running
        if (isWallRunning)
        {
            ContinueWallRun();
            CheckWallRunEnd();
        }
    }

    // Start the wall run action
    private void StartWallRun(float wallSide)
    {
        isWallRunning = true;
        wallRunTime = 0f;
        animator.SetBool("IsWallRunning", true);
        animator.SetFloat("WallSide", wallSide);
        scoreManager.AddTrickScore(100);
        boostManager.AddBoost(50f);
    }

    // Continue the wall run action
    private void ContinueWallRun()
    {
        wallRunTime += Time.deltaTime;

        if (wallRunTime < wallRunDuration)
        {
            float moveZ = Input.GetAxis("Vertical");
            Vector3 move = transform.forward * moveZ;
            rb.velocity = new Vector3(move.x, rb.velocity.y, move.z * wallRunSpeed);
        }
        else
        {
            StopWallRun();
        }
    }

    // Check if the player should stop wall running
    private void CheckWallRunEnd()
    {
        bool wallOnRight = Physics.Raycast(transform.position, transform.right, wallRunDetectionDistance);
        bool wallOnLeft = Physics.Raycast(transform.position, -transform.right, wallRunDetectionDistance);

        if (!wallOnRight && animator.GetFloat("WallSide") == 1f)
        {
            StopWallRun();
        }
        else if (!wallOnLeft && animator.GetFloat("WallSide") == 0f)
        {
            StopWallRun();
        }
    }

    // Stop the wall run action
    private void StopWallRun()
    {
        isWallRunning = false;
        animator.SetBool("IsWallRunning", false);
    }

    // Trigger stumble animation
    public void Stumble()
    {
        if (!isStumbling)
        {
            StartCoroutine(StumbleCoroutine());
        }
    }

    // Handle the stumble action
    private IEnumerator StumbleCoroutine()
    {
        isStumbling = true;

        string stumbleAnimation = Random.value > 0.5f ? stumbleLeftAnimation : stumbleRightAnimation;
        animator.SetTrigger(stumbleAnimation);

        float endTime = Time.time + stumbleDuration;

        while (Time.time < endTime)
        {
            yield return null;
        }

        isStumbling = false;
    }

    // Activate a speed boost
    public void ActivateSpeedBoost(float multiplier, float duration)
    {
        currentSpeedBoostMultiplier = multiplier;
        boostEndTime = Time.time + duration;
    }

    // Update the boost status and reset if expired
    private void UpdateBoostStatus()
    {
        if (Time.time > boostEndTime)
        {
            currentSpeedBoostMultiplier = 1f;
            animator.SetBool("IsBoosting", false);
        }
    }

    // Calculate the score based on the time taken to complete the level
    private int CalculateTimeScore()
    {
        float totalTime = FindObjectOfType<Timer>().elapsedTime;

        if (totalTime < 60)
        {
            return 1000;
        }
        else if (totalTime < 120)
        {
            return 800;
        }
        else if (totalTime < 180)
        {
            return 600;
        }
        else if (totalTime < 240)
        {
            return 400;
        }
        else
        {
            return 200;
        }
    }

    // Toggle the minimap on and off
    private void ToggleMiniMap()
    {
        miniMapCamera.SetActive(!miniMapCamera.activeSelf);
    }

    // Update the player's control key bindings
    public void UpdateControlKeys(Dictionary<string, KeyCode> newControls)
    {
        controls.controls = newControls;
    }
}