using UnityEngine;
using System.Collections;

public class PlayerCollisionHandler : MonoBehaviour
{
    private PlayerMouvement player;

    [SerializeField] private CameraShake cameraShake; // assigner dans l’inspector

    private void Start()
    {
        player = GetComponent<PlayerMouvement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // --- Obstacle Pierre ---
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("Obstacle pierre touché !");
            player.SetSpeed(0); // bloque jusqu'à ce que le joueur change de lane

            // Camera shake
            if (cameraShake != null)
                cameraShake.Shake(0.5f, 0.2f);
        }
        // --- Obstacle Bois ---
        else if (other.CompareTag("ObstacleWood"))
        {
            Debug.Log("Obstacle bois touché !");
            StartCoroutine(HitWoodCoroutine());

            // Camera shake
            if (cameraShake != null)
                cameraShake.Shake(0.5f, 0.2f);
        }
    }

    private IEnumerator HitWoodCoroutine()
    {
        player.SetSpeed(0);                  // arrêter le joueur
        yield return new WaitForSeconds(0.5f); // pause 0.5 seconde
        player.SetSpeed(2);                  // remettre la vitesse de course
    }
}
