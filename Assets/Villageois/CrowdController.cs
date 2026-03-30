using UnityEngine;

public class CrowdController : MonoBehaviour
{
    [Header("Player Reference")]
    public PlayerMouvement player;

    [Header("Game Manager Reference")]
    public GameManager gameManager; // Référence au GameManager via l'inspecteur

    [Header("Crowd Settings")]
    public float followDistance = 5f;    // distance initiale derrière le joueur
    public float catchUpSpeed = 1f;      // vitesse de rattrapage maximale
    public float recoilFactor = 0.5f;    // facteur pour limiter la vitesse de recul en vol

    [Header("Pressure Settings")]
    public float crowdSpeedMultiplier = 1.2f; // 🔹 multiplicateur de pression

    private float zOffset;

    private void Start()
    {
        if (player == null) return;

        // Initialisation derrière le joueur
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

        float playerSpeed = player.GetForwardSpeed();
        float speedFactor = 1f;

        if (!player.IsFlying)
        {
            // 🔥 NOUVEAU SYSTEME INTELLIGENT
            if (playerSpeed == 0f)
            {
                speedFactor = 1.5f; // joueur bloqué → la foule avance
            }
            else
            {
                float speedRatio = playerSpeed / player.runSpeed;

                // plus le joueur est lent, plus la foule accélère
                speedFactor = Mathf.Lerp(2f, 0.5f, speedRatio);
            }

            float deltaZ = catchUpSpeed * speedFactor * crowdSpeedMultiplier * Time.deltaTime;
            zOffset -= deltaZ;
        }
        else
        {
            // Joueur en vol → la foule recule proportionnellement à sa vitesse
            float deltaZ = playerSpeed * recoilFactor * Time.deltaTime;
            zOffset += deltaZ;
        }

        // Position finale de la foule (X fixe)
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

            if (gameManager != null)
                gameManager.PlayerCaughtByCrowd();
        }
    }

    // 🔹 Optionnel : remettre la foule derrière le joueur après une mort/continuer
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