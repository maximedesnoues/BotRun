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
    [SerializeField] private float climbSpeed;
    [SerializeField] private float climbEndTolerance;
    [SerializeField] private float climbDetectionDistance;

    [Header("Roll Settings")]
    [SerializeField] private float rollForce;

    [Header("Wall Run Settings")]
    [SerializeField] private float wallRunSpeed;
    [SerializeField] private float wallRunDuration;

    [Header("Ground Check Settings")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundLayer;

    [Header("MiniMap Settings")]
    [SerializeField] private GameObject miniMapCamera;

    private Rigidbody rb;
    private Animator animator;
    private ScoreManager scoreManager;

    private bool isGrounded;
    private bool isJumping;
    private bool isClimbing;
    private bool isNearClimbable;
    private bool isClimbEnding;
    private bool isRolling;
    private bool isWallRunning;
    private bool isNearWall;
    private bool hasRolledInAir;
    private WallProperties currentWallProperties;

    private float jumpPressTime;
    private float currentJumpForce;
    private float wallRunTime;

    private bool isSpeedBoosted = false;
    private bool isJumpBoosted = false;

    private float boostEndTime;
    private float currentSpeedBoostMultiplier = 1f;
    private float currentJumpBoostMultiplier = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    private void Update()
    {
        UpdateGroundStatus();

        if (!isClimbing && !isRolling && !isWallRunning)
        {
            HandleMovement();
            HandleJump();
        }

        HandleClimb();
        HandleRoll();
        HandleWallRun();

        if (isClimbing && currentWallProperties != null)
        {
            CheckIfReachedTop();
        }

        UpdateBoostStatus();

        if (Input.GetKeyDown(KeyCode.V))
        {
            ToggleMiniMap();
        }
    }

    private void FixedUpdate()
    {
        if (!isClimbing && !isRolling && !isWallRunning)
        {
            HandleRotation();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        WallProperties wallProperties = collision.gameObject.GetComponent<WallProperties>();

        if (wallProperties != null)
        {
            currentWallProperties = wallProperties;

            if (wallProperties.isClimbable)
            {
                isNearClimbable = true;
            }

            if (wallProperties.isWallRunnable)
            {
                isNearWall = true;
            }
        }

        if (collision.gameObject.name == "Destination")
        {
            scoreManager.AddTimeScore(CalculateTimeScore());
            Debug.Log("Destination Reached!");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        WallProperties wallProperties = collision.gameObject.GetComponent<WallProperties>();

        if (wallProperties != null && wallProperties == currentWallProperties)
        {
            isNearClimbable = false;
            isNearWall = false;
            currentWallProperties = null;
        }
    }

    private void ToggleMiniMap()
    {
        miniMapCamera.SetActive(!miniMapCamera.activeSelf);
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
        Vector3 raycastOrigin = transform.position + Vector3.up * 0.1f;
        float raycastDistance = groundCheckDistance;

        return Physics.Raycast(raycastOrigin, Vector3.down, raycastDistance, groundLayer);
    }

    private void HandleRotation()
    {
        float turn = Input.GetAxis("Horizontal");

        if (turn != 0)
        {
            float targetAngle = transform.eulerAngles.y + turn * rotationSpeed * Time.deltaTime;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.5f);
        }
    }

    private void HandleMovement()
    {
        float moveZ = Input.GetAxis("Vertical");
        float speed = isSpeedBoosted ? maxMoveSpeed * currentSpeedBoostMultiplier : maxMoveSpeed;
        Vector3 move = transform.forward * moveZ * speed;
        rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
        float animationSpeed = new Vector3(move.x, 0, move.z).magnitude;

        animator.SetFloat("RunSpeed", animationSpeed);
        animator.SetBool("IsRunning", animationSpeed > 0);

        animator.SetFloat("AnimationSpeed", speed / maxMoveSpeed);
    }

    private void HandleJump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            StartJump();
        }

        if (isJumping)
        {
            ContinueJump();
        }
    }

    private void StartJump()
    {
        isJumping = true;
        jumpPressTime = 0f;
        currentJumpForce = initialJumpForce * currentJumpBoostMultiplier;
        rb.velocity = new Vector3(rb.velocity.x, currentJumpForce, rb.velocity.z);
        animator.SetTrigger("Jump");
    }

    private void ContinueJump()
    {
        if (Input.GetButton("Jump"))
        {
            jumpPressTime += Time.deltaTime;
            currentJumpForce = Mathf.Min(initialJumpForce * currentJumpBoostMultiplier + jumpPressTime * jumpForceIncreaseRate, maxJumpForce * currentJumpBoostMultiplier);
            rb.velocity = new Vector3(rb.velocity.x, currentJumpForce, rb.velocity.z);
        }

        if (Input.GetButtonUp("Jump") || currentJumpForce >= maxJumpForce * currentJumpBoostMultiplier)
        {
            isJumping = false;
        }
    }

    private void HandleClimb()
    {
        if (Input.GetButton("Climb") && isNearClimbable && IsFacingClimbable())
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

    private bool IsFacingClimbable()
    {
        if (isNearClimbable)
        {
            RaycastHit hit;
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 origin = transform.position + Vector3.up * 1f;

            if (Physics.Raycast(origin, forward, out hit, climbDetectionDistance))
            {
                WallProperties wallProperties = hit.collider.GetComponent<WallProperties>();

                if (wallProperties != null && wallProperties.isClimbable)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void CheckIfReachedTop()
    {
        float buildingHeight = currentWallProperties.GetBuildingHeight();
        float playerHeight = transform.position.y;

        if (playerHeight >= currentWallProperties.transform.position.y + buildingHeight - climbEndTolerance)
        {
            isClimbEnding = true;
            animator.SetBool("IsClimbing", false);
            animator.SetBool("IsClimbEnding", true);
            animator.SetTrigger("ClimbEnd");
        }
        else
        {
            animator.SetBool("IsClimbEnding", false);
        }
    }

    private void HandleRoll()
    {
        if (Input.GetAxis("Horizontal") != 0 && !isGrounded && !hasRolledInAir)
        {
            StartRoll();
        }

        if (isGrounded)
        {
            StopRoll();
        }
    }

    private void StartRoll()
    {
        isRolling = true;
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
        scoreManager.AddTrickScore(5);
        ActivateSpeedBoost(GameManager.Instance.speedBoostMultiplier, GameManager.Instance.boostDuration);
    }

    private void StopRoll()
    {
        isRolling = false;
    }

    private void HandleWallRun()
    {
        if (Input.GetButtonDown("WallRun") && animator.GetBool("IsRunning") && isNearWall)
        {
            StartWallRun();
        }

        if (isWallRunning)
        {
            ContinueWallRun();
        }
    }

    private void StartWallRun()
    {
        isWallRunning = true;
        wallRunTime = 0f;
        animator.SetBool("IsWallRunning", true);
        scoreManager.AddTrickScore(15);
        ActivateSpeedBoost(GameManager.Instance.speedBoostMultiplier, GameManager.Instance.boostDuration);

        if (Physics.Raycast(transform.position, transform.right, out RaycastHit hitRight, 1f))
        {
            WallProperties wallProperties = hitRight.collider.GetComponent<WallProperties>();

            if (wallProperties != null && wallProperties.isWallRunnable)
            {
                animator.SetFloat("WallSide", 1f);
            }
        }
        else if (Physics.Raycast(transform.position, -transform.right, out RaycastHit hitLeft, 1f))
        {
            WallProperties wallProperties = hitLeft.collider.GetComponent<WallProperties>();

            if (wallProperties != null && wallProperties.isWallRunnable)
            {
                animator.SetFloat("WallSide", 0f);
            }
        }
    }

    private void ContinueWallRun()
    {
        wallRunTime += Time.deltaTime;

        if (wallRunTime < wallRunDuration)
        {
            float moveZ = Input.GetAxis("Vertical");
            Vector3 move = transform.forward * moveZ;
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, move.z * wallRunSpeed);
        }
        else
        {
            StopWallRun();
        }
    }

    private void StopWallRun()
    {
        isWallRunning = false;
        animator.SetBool("IsWallRunning", false);
    }

    public void ActivateSpeedBoost(float multiplier, float duration)
    {
        isSpeedBoosted = true;
        currentSpeedBoostMultiplier = multiplier;
        boostEndTime = Time.time + duration;
    }

    public void ActivateJumpBoost(float multiplier, float duration)
    {
        isJumpBoosted = true;
        currentJumpBoostMultiplier = multiplier;
        boostEndTime = Time.time + duration;
    }

    private void UpdateBoostStatus()
    {
        if (Time.time > boostEndTime)
        {
            isSpeedBoosted = false;
            currentSpeedBoostMultiplier = 1f;
            isJumpBoosted = false;
            currentJumpBoostMultiplier = 1f;
        }
    }

    private int CalculateTimeScore()
    {
        float totalTime = FindObjectOfType<Timer>().elapsedTime;
        return Mathf.Max(0, 1000 - Mathf.FloorToInt(totalTime * 10));
    }
}