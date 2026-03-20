using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPos;
    private Coroutine currentShake;

    private void Awake()
    {
        originalPos = transform.localPosition;
    }

    /// <summary>
    /// Lance un shake de caméra
    /// </summary>
    /// <param name="duration">Durée totale du shake en secondes</param>
    /// <param name="magnitude">Intensité maximale</param>
    public void Shake(float duration = 0.5f, float magnitude = 0.2f)
    {
        // Stoppe un shake en cours si nécessaire
        if (currentShake != null)
            StopCoroutine(currentShake);

        currentShake = StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Intensité décroissante
            float damper = 1f - (elapsed / duration);

            // Randomisation X, Y, Z
            float x = Random.Range(-1f, 1f) * magnitude * damper;
            float y = Random.Range(-1f, 1f) * magnitude * damper;
            float z = Random.Range(-1f, 1f) * magnitude * damper * 0.5f; // moins sur Z pour ne pas être trop violent

            transform.localPosition = originalPos + new Vector3(x, y, z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Remet la position originale à la fin
        transform.localPosition = originalPos;
        currentShake = null;
    }
}
