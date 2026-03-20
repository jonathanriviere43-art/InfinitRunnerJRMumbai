using System;
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

    [Header("Speed Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float flySpeed = 15f;

    private int currentSpeedState = 2; // start en course
    private float forwardSpeed;

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

    // ----- NOUVEAU -----
    private int speedBeforeStop = 2; // mémorise la vitesse avant l'arrêt

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Jump.performed += ctx =>
        {
            if (!isJumping && !isSliding)
                StartJump();
        };

        controls.Player.Slide.performed += ctx =>
        {
            if (!isSliding && !isJumping)
                StartSlide();
        };

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

    private void Start()
    {
        SetSpeed(currentSpeedState);
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    // -------- JUMP --------

    private void StartJump()
    {
        isJumping = true;
        jumpTimeElapsed = 0f;
        startY = transform.position.y;
    }

    // -------- SLIDE --------

    private void StartSlide()
    {
        isSliding = true;
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

        // Si on était à vitesse 0, on reprend la vitesse précédente
        if (forwardSpeed == 0f)
            SetSpeed(speedBeforeStop);
    }

    private void MoveRight()
    {
        targetLane = Mathf.Min(lanes.Length - 1, currentLane + 1);
        lockedMoveDirection = 1f;

        // Si on était à vitesse 0, on reprend la vitesse précédente
        if (forwardSpeed == 0f)
            SetSpeed(speedBeforeStop);
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

    // -------- SPEED SYSTEM --------

    public void SetSpeed(int v)
    {
        currentSpeedState = v;

        // mémoriser la vitesse avant l'arrêt
        if (v != 0)
            speedBeforeStop = v;

        switch (v)
        {
            case 0: forwardSpeed = 0f; break;
            case 1: forwardSpeed = walkSpeed; break;
            case 2: forwardSpeed = runSpeed; break;
            case 3: forwardSpeed = flySpeed; break;
        }
    }

    public float GetForwardSpeed()
    {
        return forwardSpeed;
    }
}