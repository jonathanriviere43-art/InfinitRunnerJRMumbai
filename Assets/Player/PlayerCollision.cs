using UnityEngine;
using System.Collections;

public class PlayerCollisionHandler : MonoBehaviour
{
    private PlayerMouvement player;

    [Header("Camera Shake")]
    [SerializeField] private CameraShake cameraShake;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip collectibleSound;

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

            speedBeforeWater = player.GetForwardSpeed() > 0 
                ? Mathf.RoundToInt(player.GetForwardSpeed()) 
                : 2;

            player.SetSpeed(Mathf.Max(1, speedBeforeWater / 5));
        }
        else if (other.CompareTag("Collectible"))
        {
            Debug.Log("Collectible ramassé !");

            if (audioSource != null && collectibleSound != null)
            {
                audioSource.PlayOneShot(collectibleSound);
            }

            Destroy(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            Debug.Log("Sortie de l'eau ! Reprise vitesse course");

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
