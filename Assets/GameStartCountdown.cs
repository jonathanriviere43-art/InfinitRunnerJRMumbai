using System.Collections;
using UnityEngine;
using TMPro;

public class GameStartCountdown : MonoBehaviour
{
    [Header("TextMeshPro")]
    public TextMeshProUGUI countdownText; // Assigne ton TMP Text ici

    [Header("Countdown Settings")]
    public float messageDuration = 1.5f; // Durée du "Prépare-toi !" 
    public float countdownInterval = 1f; // Intervalle entre 3,2,1
    public float shakeMagnitude = 5f;    // Magnitude du shake
    public Vector3 initialScale = Vector3.zero;
    public Vector3 targetScale = Vector3.one;

    private void Start()
    {
        if (countdownText == null)
        {
            Debug.LogError("Countdown TextMeshProUGUI non assigné !");
            return;
        }

        StartCoroutine(StartCountdownRoutine());
    }

    private IEnumerator StartCountdownRoutine()
    {
        // 1️⃣ Bloquer le jeu
        Time.timeScale = 0f;
        float unscaledTime = 0f;

        // 2️⃣ Afficher le message "Prépare-toi !"
        countdownText.text = "Prépare-toi au buchet sorcière";
        countdownText.color = new Color(countdownText.color.r, countdownText.color.g, countdownText.color.b, 0f);
        countdownText.transform.localScale = initialScale;

        unscaledTime = 0f;
        while (unscaledTime < messageDuration)
        {
            unscaledTime += Time.unscaledDeltaTime;
            float t = unscaledTime / messageDuration;

            // Fade + scale
            countdownText.color = new Color(countdownText.color.r, countdownText.color.g, countdownText.color.b, t);
            countdownText.transform.localScale = Vector3.Lerp(initialScale, targetScale, t);

            yield return null;
        }

        // Petite pause
        yield return new WaitForSecondsRealtime(0.3f);

        // Shake rapide
        yield return ShakeText(countdownText, 0.3f, shakeMagnitude);

        // 3️⃣ Décompte 3 → 2 → 1 → GO !
        string[] countdownNumbers = { "3", "2", "1", "GO !" };
        foreach (string s in countdownNumbers)
        {
            countdownText.text = s;
            countdownText.color = new Color(countdownText.color.r, countdownText.color.g, countdownText.color.b, 1f);
            countdownText.transform.localScale = targetScale;

            yield return ShakeText(countdownText, 0.5f, shakeMagnitude);

            // Intervalle entre les nombres
            yield return new WaitForSecondsRealtime(countdownInterval);
        }

        // 4️⃣ Nettoyer le texte
        countdownText.text = "";

        // 5️⃣ Reprendre le jeu
        Time.timeScale = 1f;
    }

    private IEnumerator ShakeText(TextMeshProUGUI text, float duration, float magnitude)
    {
        Vector3 originalPos = text.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float x = Random.Range(-magnitude, magnitude);
            float y = Random.Range(-magnitude, magnitude);
            text.transform.localPosition = originalPos + new Vector3(x, y, 0f);
            yield return null;
        }

        text.transform.localPosition = originalPos;
    }
}
