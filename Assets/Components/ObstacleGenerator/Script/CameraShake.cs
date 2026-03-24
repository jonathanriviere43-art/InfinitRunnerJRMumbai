using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPos;
    private Coroutine currentShake;

    private Camera cam;
    private float originalFOV;

    private void Awake()
    {
        originalPos = transform.localPosition;

        cam = GetComponent<Camera>();
        if (cam != null)
            originalFOV = cam.fieldOfView;
    }

    public void Shake(float duration = 0.5f, float magnitude = 0.2f, float fovKick = 5f)
    {
        if (currentShake != null)
            StopCoroutine(currentShake);

        currentShake = StartCoroutine(ShakeCoroutine(duration, magnitude, fovKick));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude, float fovKick)
    {
        float elapsed = 0f;

        float targetFOV = originalFOV + fovKick;

        while (elapsed < duration)
        {
            float damper = 1f - (elapsed / duration);

            float x = Random.Range(-1f, 1f) * magnitude * damper;
            float y = Random.Range(-1f, 1f) * magnitude * damper;
            float z = Random.Range(-1f, 1f) * magnitude * damper * 0.5f;

            transform.localPosition = originalPos + new Vector3(x, y, z);

            // Zoom progressif
            if (cam != null)
                cam.fieldOfView = Mathf.Lerp(targetFOV, originalFOV, elapsed / duration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;

        if (cam != null)
            cam.fieldOfView = originalFOV;

        currentShake = null;
    }
}
