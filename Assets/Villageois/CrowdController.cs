using UnityEngine;

public class CrowdController : MonoBehaviour
{
    [Header("Player Reference")]
    public PlayerMouvement player;

    [Header("Game Manager Reference")]
    public GameManager gameManager;

    [Header("Crowd Settings")]
    public float followDistance = 5f;
    public float catchUpSpeed = 1f;
    public float recoilFactor = 0.5f;

    [Header("Pressure Settings")]
    public float crowdSpeedMultiplier = 1.2f;

    private float zOffset;

    private void Start()
    {
        if (player == null) return;

        zOffset = followDistance;

        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            player.transform.position.z - zOffset
        );
    }

    private void Update()
    {
        if (player == null) return;

        // 🔥 Potion Vitesse (crowd slowdown)
        if (PotionEffectManager.Instance != null)
        {
            crowdSpeedMultiplier = PotionEffectManager.Instance.GetCrowdSpeedMultiplier();
        }

        float playerSpeed = player.GetForwardSpeed();
        float speedFactor = 1f;

        if (!player.IsFlying)
        {
            if (playerSpeed == 0f)
            {
                speedFactor = 1.5f;
            }
            else
            {
                float speedRatio = playerSpeed / player.runSpeed;
                speedFactor = Mathf.Lerp(2f, 0.5f, speedRatio);
            }

            float deltaZ = catchUpSpeed * speedFactor * crowdSpeedMultiplier * Time.deltaTime;
            zOffset -= deltaZ;
        }
        else
        {
            float deltaZ = playerSpeed * recoilFactor * Time.deltaTime;
            zOffset += deltaZ;
        }

        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            player.transform.position.z - zOffset
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Contact avec le joueur !");

            // 🔥 SECOND LIFE CHECK
            if (PotionEffectManager.Instance != null)
            {
                bool used = PotionEffectManager.Instance.UseSecondLife();

                if (used)
                {
                    // Repousser la foule
                    zOffset += 10f;

                    Debug.Log("💥 Second Life activée → foule repoussée");

                    return; // pas de game over
                }
            }

            // ❌ Game Over normal
            if (gameManager != null)
                gameManager.PlayerCaughtByCrowd();
        }
    }

    public void ResetCrowd()
    {
        if (player == null) return;

        zOffset = followDistance;

        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            player.transform.position.z - zOffset
        );
    }
}