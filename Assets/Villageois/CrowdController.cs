using UnityEngine;

public class CrowdController : MonoBehaviour
{
    [Header("Player Reference")]
    public PlayerMouvement player;

    [Header("Crowd Settings")]
    public float followDistance = 5f;    // distance initiale derrière le joueur
    public float catchUpSpeed = 1f;      // vitesse de rattrapage maximale
    public float recoilFactor = 0.5f;    // facteur pour limiter la vitesse de recul en vol

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
            // Catch-up normal quand joueur au sol
            if (playerSpeed == 0f)
            {
                speedFactor = 1f; // joueur bloqué
            }
            else if (playerSpeed > 0f && playerSpeed < player.runSpeed)
            {
                speedFactor = 0.2f; // joueur ralenti
            }

            if (playerSpeed < player.runSpeed)
            {
                float deltaZ = catchUpSpeed * speedFactor * Time.deltaTime;
                zOffset -= deltaZ;
            }
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
    
}