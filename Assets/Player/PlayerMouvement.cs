using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMouvement : MonoBehaviour
{
    [Header("Saut Settings")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpDuration = 0.5f;

    [Header("Slide Settings")]
    [SerializeField] private float slideDuration = 0.5f;

    [Header("Déplacement Lanes")]
    [SerializeField] private Transform[] lanes;
    [SerializeField] private float moveSpeed = 5f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private PlayerControls controls;

    private int currentLane = 1;
    private int targetLane = 1;

    private float lockedMoveDirection = 0f;

    // Jump
    private bool isJumping = false;
    private float jumpTimeElapsed = 0f;
    private float startY;

    // Slide
    private bool isSliding = false;

    private void Awake()
    {
        controls = new PlayerControls();

        // Jump
        controls.Player.Jump.performed += ctx =>
        {
            if (!isJumping && !isSliding)
                StartJump();
        };

        // Slide
        controls.Player.Slide.performed += ctx =>
        {
            if (!isSliding && !isJumping)
                StartSlide();
        };

        // Move
        controls.Player.Move.performed += ctx =>
        {
            if (lockedMoveDirection != 0f) return;

            float moveValue = ctx.ReadValue<float>();

            if (moveValue < 0)
                MoveLeft();
            else if (moveValue > 0)
                MoveRight();
        };
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    // -------- JUMP --------

    private void StartJump()
    {
        isJumping = true;
        jumpTimeElapsed = 0f;
        startY = transform.position.y;

        animator.SetTrigger("Jump");
    }

    // -------- SLIDE --------

    private void StartSlide()
    {
        isSliding = true;

        animator.SetTrigger("Slide");

        Invoke(nameof(EndSlide), slideDuration);
    }

    private void EndSlide()
    {
        isSliding = false;
    }

    // -------- MOVEMENT --------

    private void MoveLeft()
    {
        targetLane = Mathf.Max(0, currentLane - 1);
        lockedMoveDirection = -1f;
    }

    private void MoveRight()
    {
        targetLane = Mathf.Min(lanes.Length - 1, currentLane + 1);
        lockedMoveDirection = 1f;
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
    }

    private void HandleMovement()
    {
        Vector3 targetPos = new Vector3(
            lanes[targetLane].position.x,
            transform.position.y,
            transform.position.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        if (Mathf.Abs(transform.position.x - targetPos.x) < 0.01f)
        {
            currentLane = targetLane;
            lockedMoveDirection = 0f;
        }
    }

    // -------- JUMP PHYSICS --------

    private void HandleJump()
    {
        if (!isJumping) return;

        jumpTimeElapsed += Time.deltaTime;

        float t = jumpTimeElapsed / jumpDuration;

        float yOffset = -4 * jumpHeight * Mathf.Pow(t - 0.5f, 2) + jumpHeight;

        transform.position = new Vector3(
            transform.position.x,
            startY + yOffset,
            transform.position.z
        );

        if (jumpTimeElapsed >= jumpDuration)
        {
            isJumping = false;

            transform.position = new Vector3(
                transform.position.x,
                startY,
                transform.position.z
            );
        }
    }
}