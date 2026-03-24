using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMouvement : MonoBehaviour
{
    [Header("Saut Settings")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpDuration = 0.5f;

    [Header("Déplacement Lanes")]
    [SerializeField] private Transform[] lanes;
    [SerializeField] private float moveSpeed = 5f;

    [Header("Speed Settings")]
    [SerializeField] private float walkSpeed = 1f;
    public float runSpeed = 2f;
    [SerializeField] private float flySpeed = 15f;

    [Header("Fly System")]
    [SerializeField] private float flyHeight = 3f;
    [SerializeField] private float fuelConsumption = 1f;

    [Header("Fly Speed Boost")]
    [SerializeField] private float flySpeedIncreaseRate = 2f;
    [SerializeField] private float maxFlyRunSpeed = 5f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private int currentSpeedState = 2;
    private float forwardSpeed;

    private PlayerControls controls;
    private int currentLane = 1;
    private int targetLane = 1;
    private float lockedMoveDirection = 0f;

    // Jump
    private bool isJumping = false;
    private float jumpTimeElapsed = 0f;
    private float startY;

    // Stop system
    private int speedBeforeStop = 2;
    private Vector3 stopPosition;

    // Fly
    private bool isFlying = false;        // true si en vol
    private float baseY;
    private PlayerInventory inventory;
    private int flyDirection = 0;          // 1 = monter, -1 = descendre, 0 = stable
    private bool forceDescend = false;     // true si on doit descendre automatiquement

    // Base speed
    private float baseRunSpeed;

    // Blocage vol au sol
    private bool blockedByObstacle = false;

    // PANNE DE CARBURANT
    private bool isOutOfFuel = false;
    private float outOfFuelTimer = 0f;
    [SerializeField] private float outOfFuelDelay = 1f;

    private void Awake()
    {
        controls = new PlayerControls();

        // Saut
        controls.Player.Jump.performed += ctx =>
        {
            if (!isJumping && !isFlying) StartJump();
        };

        // Déplacement latéral
        controls.Player.Move.performed += ctx =>
        {
            if (lockedMoveDirection != 0f) return;

            float moveValue = ctx.ReadValue<float>();
            if (moveValue < 0) MoveLeft();
            else if (moveValue > 0) MoveRight();
        };

        // Vol
        controls.Player.Fly.performed += ctx =>
        {
            string key = ctx.control.name.ToLower();
            if (inventory == null) return;

            if (key == "w" && !blockedByObstacle && inventory.currentFuel > 0f && !isOutOfFuel)
            {
                // Décollage : on monte
                flyDirection = 1;
                forceDescend = false;
                isFlying = true;
            }
            else if (key == "s")
            {
                // Descente forcée uniquement si en vol
                if (isFlying)
                {
                    flyDirection = -1;
                    forceDescend = true;
                }
            }
        };
    }

    private void Start()
    {
        baseRunSpeed = runSpeed;
        SetSpeed(currentSpeedState);
        inventory = GetComponent<PlayerInventory>();
        baseY = transform.position.y;
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void StartJump()
    {
        isJumping = true;
        jumpTimeElapsed = 0f;
        startY = transform.position.y;

        if (forwardSpeed == 0f)
        {
            SetSpeed(speedBeforeStop);
            lockedMoveDirection = 0f;
        }

        animator?.SetTrigger("Jump");
    }

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
            transform.position = new Vector3(transform.position.x, baseY, transform.position.z);
        }
    }

    private void MoveLeft()
    {
        targetLane = Mathf.Max(0, currentLane - 1);
        lockedMoveDirection = -1f;
        if (forwardSpeed == 0f) SetSpeed(speedBeforeStop);
    }

    private void MoveRight()
    {
        targetLane = Mathf.Min(lanes.Length - 1, currentLane + 1);
        lockedMoveDirection = 1f;
        if (forwardSpeed == 0f) SetSpeed(speedBeforeStop);
    }

    private void HandleMovement()
    {
        if (forwardSpeed == 0f)
        {
            transform.position = new Vector3(stopPosition.x, transform.position.y, stopPosition.z);
            return;
        }

        Vector3 targetPos = new Vector3(lanes[targetLane].position.x, transform.position.y, transform.position.z);

        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - targetPos.x) < 0.01f)
        {
            currentLane = targetLane;
            lockedMoveDirection = 0f;
        }
    }

    private void HandleFly()
    {
        if (inventory == null) return;

        Vector3 pos = transform.position;

        // Monter
        if (flyDirection == 1 && pos.y < baseY + flyHeight)
        {
            pos.y += flySpeed * Time.deltaTime;
        }
        // Descendre (vol ou panne)
        else if ((flyDirection == -1 || forceDescend) && pos.y > baseY)
        {
            pos.y -= flySpeed * Time.deltaTime;
        }

        pos.y = Mathf.Clamp(pos.y, baseY, baseY + flyHeight);
        transform.position = pos;

        // Consommation fuel : seulement si le joueur est en vol et pas au sol
        if (isFlying && pos.y > baseY && !isOutOfFuel)
        {
            bool hasFuel = inventory.ConsumeFuel(fuelConsumption * Time.deltaTime);

            if (!hasFuel)
            {
                isOutOfFuel = true;
                forceDescend = true;
                flyDirection = -1;
                outOfFuelTimer = outOfFuelDelay;
                SetSpeed(0); // blocage sol
            }
        }

        // Mise à jour vitesse forward
        if (isFlying)
        {
            runSpeed += flySpeedIncreaseRate * Time.deltaTime;
            runSpeed = Mathf.Min(runSpeed, maxFlyRunSpeed);
            if (currentSpeedState == 2 || currentSpeedState == 3)
                forwardSpeed = runSpeed;
        }
        else if (!isOutOfFuel)
        {
            runSpeed = baseRunSpeed;
            if (currentSpeedState == 2 || currentSpeedState == 3)
                forwardSpeed = runSpeed;
        }

        // Fin de vol si arrivé au sol
        if (pos.y <= baseY)
        {
            isFlying = false;
            flyDirection = 0;
            forceDescend = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isFlying)
        {
            if (other.CompareTag("Obstacle") || other.CompareTag("ObstacleWood"))
                blockedByObstacle = true;
        }
        else
        {
            if (other.CompareTag("Obstacle") || other.CompareTag("ObstacleWood"))
            {
                flyDirection = -1;
                forceDescend = true;
                runSpeed = Mathf.Max(baseRunSpeed, runSpeed * 0.8f);
                if (currentSpeedState == 2 || currentSpeedState == 3)
                    forwardSpeed = runSpeed;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("ObstacleWood"))
            if (!isFlying)
                blockedByObstacle = false;
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
        HandleFly();

        // Gestion panne de fuel
        if (isOutOfFuel)
        {
            outOfFuelTimer -= Time.deltaTime;
            if (outOfFuelTimer <= 0f)
            {
                isOutOfFuel = false;
                SetSpeed(2);
                flyDirection = 0;
                forceDescend = false;
            }
        }
    }

    public void SetSpeed(int v)
    {
        currentSpeedState = v;

        if (v != 0) speedBeforeStop = v;
        else stopPosition = transform.position;

        switch (v)
        {
            case 0: forwardSpeed = 0f; break;
            case 1: forwardSpeed = walkSpeed; break;
            case 2: forwardSpeed = runSpeed; break;
            case 3: forwardSpeed = runSpeed; break;
        }
    }

    public float GetForwardSpeed() => forwardSpeed;
    public bool IsFlying => isFlying;
    public bool IsOutOfFuel => isOutOfFuel;
    
}