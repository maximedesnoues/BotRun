using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxMoveSpeed;
    [SerializeField] private float rotationSpeed;

    [Header("Jump Settings")]
    [SerializeField] private float initialJumpForce;
    [SerializeField] private float maxJumpForce;
    [SerializeField] private float jumpForceIncreaseRate;

    [Header("Climb Settings")]
    [SerializeField] private float climbDetectionDistance;
    [SerializeField] private float climbSpeed;

    [Header("Roll Settings")]
    [SerializeField] private float rollForce;

    [Header("Wall Run Settings")]
    [SerializeField] private float wallRunDetectionDistance;
    [SerializeField] private float wallRunSpeed;
    [SerializeField] private float wallRunDuration;

    [Header("Ground Check Settings")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundLayer;

    [Header("Stumble Settings")]
    [SerializeField] private float stumbleDuration;
    [SerializeField] private float stumbleSpeedMultiplier;

    [Header("MiniMap Settings")]
    [SerializeField] private GameObject miniMapCamera;

    private CapsuleCollider capsuleCollider;
    private Rigidbody rb;
    private Animator animator;
    private BoostManager boostManager;
    private Timer timer;
    private ScoreManager scoreManager;
    private LevelComplete levelComplete;
    private Controls controls;

    private bool isGrounded;
    private bool isJumping;
    private bool isClimbing;
    private bool isAirRolling;
    private bool isWallRunning;
    private bool isNearClimbable;
    private bool isNearWall;
    private bool hasRolledInAir;
    private bool isStumbling = false;
    private bool isEndClimbAnimationPlaying = false;

    private float jumpPressTime;
    private float currentJumpForce;
    private float wallRunTime;
    private float boostEndTime;
    private float currentSpeedBoostMultiplier = 1f;

    private BuildingProperties currentBuildingProperties;

    private void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        boostManager = FindObjectOfType<BoostManager>();
        timer = FindObjectOfType<Timer>();
        scoreManager = FindObjectOfType<ScoreManager>();
        levelComplete = FindObjectOfType<LevelComplete>();
        controls = FindObjectOfType<Controls>();
    }

    private void Update()
    {
        UpdateGroundStatus();
        UpdateWallDetection();
        UpdateClimbDetection();

        if (!isClimbing && !isAirRolling && !isWallRunning && !isEndClimbAnimationPlaying)
        {
            HandleMovement();
            HandleJump();
        }

        HandleClimb();
        HandleWallRun();

        if (currentBuildingProperties != null && isClimbing)
        {
            CheckIfReachedTop();
        }

        UpdateBoostStatus();

        if (Input.GetKeyDown(controls.controls["Boost"]) && boostManager.IsBoostFull())
        {
            ActivateSpeedBoost(GameManager.Instance.speedBoostMultiplier, GameManager.Instance.boostDuration);
            boostManager.ConsumeBoost();
        }

        if (Input.GetKeyDown(controls.controls["MiniMap"]))
        {
            ToggleMiniMap();
        }

        Debug.DrawRay(transform.position + Vector3.up * 1.5f, transform.forward * climbDetectionDistance, Color.green);
        Debug.DrawRay(transform.position, transform.right * wallRunDetectionDistance, Color.blue);
        Debug.DrawRay(transform.position, -transform.right * wallRunDetectionDistance, Color.blue);
    }

    private void FixedUpdate()
    {
        if (!isClimbing && !isAirRolling && !isWallRunning && !isEndClimbAnimationPlaying)
        {
            HandleRotation();
        }

        UpdateCapsuleColliderPosition();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destination"))
        {
            scoreManager.AddTimeScore(CalculateTimeScore());
            ShowLevelCompleteScreen();
        }
    }

    private void UpdateCapsuleColliderPosition()
    {
        if (animator)
        {
            capsuleCollider.center = animator.bodyPosition - transform.position;
        }
    }

    private void ShowLevelCompleteScreen()
    {
        int totalScore = scoreManager.GetTotalScore();
        float elapsedTime = timer.elapsedTime;
        int tricks = scoreManager.GetTricksPerformed();
        string objectsCollected = scoreManager.GetCollectablesCollected().ToString();

        levelComplete.ShowLevelComplete(totalScore, elapsedTime, tricks, objectsCollected);
    }

    private void UpdateGroundStatus()
    {
        isGrounded = CheckIfGrounded();

        if (isGrounded)
        {
            hasRolledInAir = false;
        }
    }

    private bool CheckIfGrounded()
    {
        Vector3 raycastOrigin = transform.position + Vector3.up * 0.2f;
        float raycastDistance = groundCheckDistance + 0.2f;
        RaycastHit hit;

        bool isHit = Physics.Raycast(raycastOrigin, Vector3.down, out hit, raycastDistance, groundLayer);
        Debug.DrawRay(raycastOrigin, Vector3.down * raycastDistance, Color.red);

        return isHit;
    }

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

    private void HandleMovement()
    {
        float moveZ = 0;
        if (Input.GetKey(controls.controls["MoveForward"]))
        {
            moveZ = 1;
        }
        else if (Input.GetKey(controls.controls["MoveBackward"]))
        {
            moveZ = -1;
        }

        float speed = isStumbling ? maxMoveSpeed * stumbleSpeedMultiplier : maxMoveSpeed * currentSpeedBoostMultiplier;
        Vector3 move = transform.forward * moveZ * speed;
        rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
        float animationSpeed = new Vector3(move.x, 0, move.z).magnitude;

        if (isGrounded)
        {
            animator.SetFloat("RunSpeed", animationSpeed);
            animator.SetBool("IsRunning", animationSpeed > 0);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
        
        animator.SetFloat("AnimationSpeed", speed / maxMoveSpeed);
    }

    private void HandleRotation()
    {
        if (Input.GetKey(controls.controls["TurnLeft"]))
        {
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(controls.controls["TurnRight"]))
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleJump()
    {
        if (isGrounded && Input.GetKeyDown(controls.controls["Jump"]))
        {
            StartJump();
        }

        if (isJumping)
        {
            ContinueJump();
        }

        HandleAirRoll();
    }

    private void StartJump()
    {
        isJumping = true;
        jumpPressTime = 0f;
        currentJumpForce = initialJumpForce;
        rb.velocity = new Vector3(rb.velocity.x, currentJumpForce, rb.velocity.z);
        animator.SetTrigger("Jump");
    }

    private void ContinueJump()
    {
        if (Input.GetKey(controls.controls["Jump"]))
        {
            jumpPressTime += Time.deltaTime;
            currentJumpForce = Mathf.Min(initialJumpForce + jumpPressTime * jumpForceIncreaseRate, maxJumpForce);
            rb.velocity = new Vector3(rb.velocity.x, currentJumpForce, rb.velocity.z);

            float normalizedJumpForce = currentJumpForce / maxJumpForce;
            float minAnimationSpeed = 0.5f;
            float maxAnimationSpeed = 1.5f;
            float animationSpeed = Mathf.Lerp(maxAnimationSpeed, minAnimationSpeed, normalizedJumpForce);

            animator.SetFloat("JumpSpeed", animationSpeed);
        }

        if (Input.GetKeyUp(controls.controls["Jump"]) || currentJumpForce >= maxJumpForce)
        {
            isJumping = false;
        }
    }

    private void HandleClimb()
    {
        if (Input.GetKey(controls.controls["Climb"]) && isNearClimbable)
        {
            StartClimb();
        }
        else
        {
            StopClimb();
        }
    }

    private void StartClimb()
    {
        isClimbing = true;
        rb.velocity = new Vector3(0, climbSpeed, 0);
        animator.SetBool("IsClimbing", true);
    }

    private void StopClimb()
    {
        isClimbing = false;
        animator.SetBool("IsClimbing", false);
    }

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

    private void StartEndClimbAnimation()
    {
        isClimbing = false;
        isEndClimbAnimationPlaying = true;
        animator.SetBool("IsClimbing", false);
        animator.SetBool("IsClimbEnding", true);
        animator.SetTrigger("ClimbEnd");

        StartCoroutine(EndClimbAnimationCoroutine());
    }

    private IEnumerator EndClimbAnimationCoroutine()
    {
        yield return new WaitForSeconds(1f);

        Vector3 climbEndForce = transform.forward * 0.5f + transform.up * 0.2f;
        rb.AddForce(climbEndForce, ForceMode.Impulse);

        isEndClimbAnimationPlaying = false;
        animator.SetBool("IsClimbEnding", false);
    }

    private void HandleAirRoll()
    {
        if (Input.GetKey(controls.controls["Jump"]) && Input.GetAxis("Horizontal") != 0 && !isGrounded && !hasRolledInAir)
        {
            StartAirRoll();
        }

        if (isGrounded)
        {
            StopAirRoll();
        }
    }

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

    private IEnumerator StopAirRollAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isAirRolling = false;
    }

    private void StopAirRoll()
    {
        isAirRolling = false;
    }

    private void HandleWallRun()
    {
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

        if (isWallRunning)
        {
            ContinueWallRun();
            CheckWallRunEnd();
        }
    }

    private void StartWallRun(float wallSide)
    {
        isWallRunning = true;
        wallRunTime = 0f;
        animator.SetBool("IsWallRunning", true);
        animator.SetFloat("WallSide", wallSide);
        scoreManager.AddTrickScore(100);
        boostManager.AddBoost(50f);
    }

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

    private void StopWallRun()
    {
        isWallRunning = false;
        animator.SetBool("IsWallRunning", false);
    }

    public void Stumble()
    {
        if (!isStumbling)
        {
            StartCoroutine(StumbleCoroutine());
        }
    }

    private IEnumerator StumbleCoroutine()
    {
        isStumbling = true;
        float endTime = Time.time + stumbleDuration;

        while (Time.time < endTime)
        {
            // Ajouter une animation ou un effet visuel de trébuchement ici
            yield return null;
        }

        isStumbling = false;
    }

    public void ActivateSpeedBoost(float multiplier, float duration)
    {
        currentSpeedBoostMultiplier = multiplier;
        boostEndTime = Time.time + duration;
    }

    private void UpdateBoostStatus()
    {
        if (Time.time > boostEndTime)
        {
            currentSpeedBoostMultiplier = 1f;
        }
    }

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

    private void ToggleMiniMap()
    {
        miniMapCamera.SetActive(!miniMapCamera.activeSelf);
    }

    public void UpdateControlKeys(Dictionary<string, KeyCode> newControls)
    {
        controls.controls = newControls;
    }
}