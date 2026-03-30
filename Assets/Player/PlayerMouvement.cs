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

    [Header("Launch Latency")]
    [SerializeField] private float launchDelay = 1f;

    [Header("Jump → Fly Combo")]
    [SerializeField] private float jumpFlyWindow = 0.3f;

    [Header("Wood Hit Settings")]
    [SerializeField] private float woodStunDuration = 0.5f;

    [Header("Air Obstacle Stun")]
    [SerializeField] private float airStunDuration = 1f; 
    [SerializeField] private float airFallSpeed = 10f;

    [Header("Invincibilité après respawn")]
    [SerializeField] private float invincibleDuration = 2f; // 🔹 2 secondes

    private int currentSpeedState = 2;
    private float forwardSpeed;

    private PlayerControls controls;
    private int currentLane = 1;
    private int targetLane = 1;

    // Jump
    private bool isJumping = false;
    private float jumpTimeElapsed = 0f;
    private float startY;

    // Stop system
    private int speedBeforeStop = 2;
    private Vector3 stopPosition;

    // Fly
    private bool isFlying = false;
    private float baseY;
    private PlayerInventory inventory;
    private int flyDirection = 0;
    private bool forceDescend = false;

    // Latence décollage
    private bool isWaitingToLaunch = false;
    private float launchTimer = 0f;

    // Combo Jump -> Fly
    private float justJumpedTimer = 0f;
    private bool jumpToFlyComboActive = false;

    // Base speed
    private float baseRunSpeed;

    // Blocage
    private bool blockedByObstacle = false;

    // WOOD STUN
    private bool isWoodStunned = false;
    private float woodStunTimer = 0f;
    private int woodObstacleCounter = 0;

    // AIR STUN
    private bool isAirStunned = false;
    private float airStunTimer = 0f;

    // PANNE DE CARBURANT
    private bool isOutOfFuel = false;
    private float outOfFuelTimer = 0f;
    [SerializeField] private float outOfFuelDelay = 1f;

    // 🔹 Invincibilité
    private bool isInvincible = false;
    private float invincibleTimer = 0f;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Jump.performed += ctx =>
        {
            if (!isJumping && !isFlying && !isWaitingToLaunch)
                StartJump();
        };

        controls.Player.Move.performed += ctx =>
        {
            if (isWaitingToLaunch) return;

            float moveValue = ctx.ReadValue<float>();

            if (moveValue < 0 && targetLane > 0)
                targetLane--;
            else if (moveValue > 0 && targetLane < lanes.Length - 1)
                targetLane++;

            if (blockedByObstacle && !isWoodStunned && !isAirStunned)
            {
                blockedByObstacle = false;
                SetSpeed(speedBeforeStop);
            }
        };

        controls.Player.Fly.performed += ctx =>
        {
            string key = ctx.control.name.ToLower();
            if (inventory == null) return;

            if (key == "w" && !blockedByObstacle && inventory.currentFuel > 0f && !isOutOfFuel && !isFlying)
            {
                if (justJumpedTimer > 0f)
                {
                    jumpToFlyComboActive = true;
                    Vector3 pos = transform.position;
                    pos.y = Mathf.Max(baseY + 0.1f, pos.y);
                    transform.position = pos;

                    isFlying = true;
                    flyDirection = 1;
                    forceDescend = false;
                    forwardSpeed = runSpeed;

                    isWaitingToLaunch = false;
                    launchTimer = 0f;
                }
                else
                {
                    isWaitingToLaunch = true;
                    launchTimer = launchDelay;
                    forwardSpeed = 0f;
                }
            }
            else if (key == "s")
            {
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

        justJumpedTimer = jumpFlyWindow;
        jumpToFlyComboActive = false;

        if (forwardSpeed == 0f)
            SetSpeed(speedBeforeStop);
    }

    private void HandleJump()
    {
        if (!isJumping) return;

        if (jumpToFlyComboActive)
        {
            isJumping = false;
            return;
        }

        jumpTimeElapsed += Time.unscaledDeltaTime;
        float t = jumpTimeElapsed / jumpDuration;
        float yOffset = -4 * jumpHeight * Mathf.Pow(t - 0.5f, 2) + jumpHeight;

        transform.position = new Vector3(transform.position.x, startY + yOffset, transform.position.z);

        if (jumpTimeElapsed >= jumpDuration)
        {
            isJumping = false;
            transform.position = new Vector3(transform.position.x, baseY, transform.position.z);
        }
    }

    private void HandleMovement()
    {
        if (isWaitingToLaunch || lanes.Length == 0) return;

        targetLane = Mathf.Clamp(targetLane, 0, lanes.Length - 1);

        Vector3 targetPos = new Vector3(lanes[targetLane].position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.unscaledDeltaTime);

        if (Mathf.Abs(transform.position.x - targetPos.x) < 0.01f)
        {
            currentLane = targetLane;
            transform.position = targetPos;
        }
    }

    private void HandleFly()
    {
        if (inventory == null) return;

        Vector3 pos = transform.position;

        if (isWaitingToLaunch)
        {
            launchTimer -= Time.unscaledDeltaTime;
            forwardSpeed = 0f;

            if (launchTimer <= 0f)
            {
                isWaitingToLaunch = false;
                isFlying = true;
                flyDirection = 1;
                forceDescend = false;
                forwardSpeed = runSpeed;
            }
            return;
        }

        if (isFlying && flyDirection == 1 && pos.y < baseY + flyHeight)
            pos.y += flySpeed * Time.unscaledDeltaTime;
        else if ((flyDirection == -1 || forceDescend) && pos.y > baseY)
            pos.y -= flySpeed * Time.unscaledDeltaTime;

        pos.y = Mathf.Clamp(pos.y, baseY, baseY + flyHeight);
        transform.position = pos;

        if (isFlying && pos.y > baseY && !isOutOfFuel)
        {
            bool hasFuel = inventory.ConsumeFuel(fuelConsumption * Time.unscaledDeltaTime);
            if (!hasFuel)
            {
                isOutOfFuel = true;
                forceDescend = true;
                flyDirection = -1;
                outOfFuelTimer = outOfFuelDelay;
                SetSpeed(0);
            }
        }

        if (isFlying)
        {
            runSpeed += flySpeedIncreaseRate * Time.unscaledDeltaTime;
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

        if (pos.y <= baseY)
        {
            isFlying = false;
            flyDirection = 0;
            forceDescend = false;
        }

        if (isAirStunned)
        {
            if (pos.y > baseY)
            {
                pos.y -= airFallSpeed * Time.unscaledDeltaTime;
                if (pos.y < baseY) pos.y = baseY;
                transform.position = pos;

                isFlying = false;
                flyDirection = 0;
                forceDescend = false;
                forwardSpeed = 0f;
            }
            else
            {
                if (airStunTimer <= 0f)
                    airStunTimer = airStunDuration;

                airStunTimer -= Time.unscaledDeltaTime;

                if (airStunTimer <= 0f)
                {
                    isAirStunned = false;
                    SetSpeed(speedBeforeStop);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isInvincible) 
        {
            Debug.Log("Collision ignorée car invincible avec : " + other.name);
            return; // Ignore collisions si invincible
        }

        if (other.CompareTag("Obstacle"))
        {
            if (isFlying)
            {
                isAirStunned = true;
                forceDescend = true;
            }
            else
            {
                blockedByObstacle = true;
                SetSpeed(0);
            }
        }
        else if (other.CompareTag("ObstacleWood"))
        {
            if (isFlying)
            {
                isAirStunned = true;
                forceDescend = true;
            }
            else
            {
                woodObstacleCounter++;
                if (!isWoodStunned)
                {
                    isWoodStunned = true;
                    woodStunTimer = woodStunDuration;
                    blockedByObstacle = true;
                    SetSpeed(0);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            blockedByObstacle = false;
            SetSpeed(speedBeforeStop);
        }
        else if (other.CompareTag("ObstacleWood"))
        {
            woodObstacleCounter = Mathf.Max(0, woodObstacleCounter - 1);
        }
    }

    private void Update()
    {
        // 🔹 Gestion invincibilité
        if (isInvincible)
        {
            invincibleTimer -= Time.unscaledDeltaTime; // 🔹 ignore Time.timeScale
            if (invincibleTimer <= 0f)
            {
                isInvincible = false;
                Debug.Log("Invincibilité terminée !");
            }
        }

        if (justJumpedTimer > 0f)
            justJumpedTimer -= Time.unscaledDeltaTime;

        if (isWoodStunned)
        {
            woodStunTimer -= Time.unscaledDeltaTime;
            if (woodStunTimer <= 0f && woodObstacleCounter == 0)
            {
                isWoodStunned = false;
                blockedByObstacle = false;
                SetSpeed(speedBeforeStop);
            }
        }

        HandleMovement();
        HandleJump();
        HandleFly();

        if (isOutOfFuel)
        {
            outOfFuelTimer -= Time.unscaledDeltaTime;
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

    // 🔹 Reset joueur et invincibilité après respawn
    public void ResetPlayerPosition()
    {
        if (lanes != null && lanes.Length > 0)
        {
            int middleLane = lanes.Length / 2;
            Vector3 lanePos = lanes[middleLane].position;
            transform.position = new Vector3(lanePos.x, baseY, transform.position.z);
            currentLane = middleLane;
            targetLane = middleLane;
        }
        else
        {
            transform.position = new Vector3(0f, baseY, transform.position.z);
            currentLane = 0;
            targetLane = 0;
        }

        isFlying = false;
        flyDirection = 0;
        forceDescend = false;
        isJumping = false;
        jumpToFlyComboActive = false;
        SetSpeed(2);

        // 🔹 Active l'invincibilité 2 secondes
        isInvincible = true;
        invincibleTimer = invincibleDuration;
        Debug.Log("Invincibilité activée !");
    }
}