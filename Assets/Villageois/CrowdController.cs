using UnityEngine;

public class CrowdController : MonoBehaviour
{
    [Header("Player Reference")]
    public PlayerMouvement player;

    [Header("Crowd Settings")]
    public float followDistance = 5f;    // distance initiale derrière le joueur
    public float catchUpSpeed = 1f;      // vitesse de rattrapage maximale

    private float zOffset;

    private void Start()
    {
        if (player == null) return;

        // Initialisation derrière le joueur
        zOffset = followDistance;
        transform.position = new Vector3(
            transform.position.x,          // garde la X initiale
            transform.position.y,
            player.transform.position.z - zOffset
        );
    }

    private void Update()
    {
        if (player == null) return;

        float playerSpeed = player.GetForwardSpeed();
        float speedFactor = 1f;

        // Déterminer le facteur selon la situation
        if (playerSpeed == 0f)
        {
            // Joueur bloqué → catch-up normal
            speedFactor = 1f;
            Debug.Log($"Joueur bloqué : catch-up normal ({catchUpSpeed} m/s)");
        }
        else if (playerSpeed > 0f && playerSpeed < player.runSpeed)
        {
            // Joueur ralenti (dans l'eau) → catch-up divisé par 5
            speedFactor = 0.2f;  // 1/5
            Debug.Log($"Joueur ralenti (eau) : catch-up divisé par 5 ({catchUpSpeed * speedFactor} m/s)");
        }

        // Appliquer le rattrapage si joueur ralenti ou bloqué
        if (playerSpeed < player.runSpeed)
        {
            float deltaZ = catchUpSpeed * speedFactor * Time.deltaTime;
            zOffset -= deltaZ;
            Debug.Log($"Rattrapage actif : ΔZ = {deltaZ}, zOffset = {zOffset}");
        }

        // Position finale de la foule (X fixe)
        transform.position = new Vector3(
            transform.position.x,          // X reste fixe
            transform.position.y,
            player.transform.position.z - zOffset
        );
    }
}