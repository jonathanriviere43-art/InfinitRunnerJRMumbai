using UnityEngine;
using System.Collections;

public class PlayerCollisionHandler : MonoBehaviour
{
    private PlayerMouvement player;

    [SerializeField] private CameraShake cameraShake; // assigner dans l’inspector

    // mémorise la vitesse avant l'eau
    private int speedBeforeWater;

    private void Start()
    {
        player = GetComponent<PlayerMouvement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("Obstacle pierre touché !");
            player.SetSpeed(0);
        }
        else if (other.CompareTag("ObstacleWood"))
        {
            Debug.Log("Obstacle bois touché !");
            StartCoroutine(HitWoodCoroutine());

            if (cameraShake != null)
                cameraShake.Shake(0.5f, 0.2f);
        }
        else if (other.CompareTag("Water"))
        {
            Debug.Log("Water touché ! Vitesse divisée par 2");

            // mémorise la vitesse actuelle
            speedBeforeWater = player.GetForwardSpeed() > 0 ? Mathf.RoundToInt(player.GetForwardSpeed()) : 2;

            // divise la vitesse par 2
            player.SetSpeed(Mathf.Max(1, speedBeforeWater / 2));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            Debug.Log("Sortie de l'eau ! Reprise vitesse course");

            // remet la vitesse de course
            player.SetSpeed(2);
        }
    }

    private IEnumerator HitWoodCoroutine()
    {
        player.SetSpeed(0);
        yield return new WaitForSeconds(0.5f);
        player.SetSpeed(2);
    }
}
