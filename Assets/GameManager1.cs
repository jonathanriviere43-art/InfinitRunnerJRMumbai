using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Player & Crowd")]
    public PlayerMouvement player;
    public CrowdController crowd;

    [Header("Vies")]
    [SerializeField] private int maxLives = 5;
    private int currentLives;

    [Header("UI")]
    public GameObject deathPanel;
    public TMP_Text livesText;
    public TMP_Text deathMessageText;
    public TMP_Text runsDetailText;

    [Header("Score")]
    public DistanceTracker distanceTracker;

    [Header("Audio")]
    public AudioSource gameplayMusic;
    public AudioSource gameOverMusic;

    private bool isGamePaused = false;

    private void Start()
    {
        currentLives = maxLives;
        UpdateLivesUI();

        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    private void UpdateLivesUI()
    {
        if (livesText != null)
            livesText.text = "Vies : " + currentLives;
    }

    // 🔴 Mort
    public void PlayerCaughtByCrowd()
    {
        if (isGamePaused) return;

        isGamePaused = true;

        // 🔥 Sauvegarde du run
        if (distanceTracker != null)
            distanceTracker.SaveAndResetRun();

        // 🎧 Transition audio
        if (gameplayMusic != null)
        {
            StartCoroutine(FadeOut(gameplayMusic, 1f));
        }

        if (gameOverMusic != null)
        {
            StartCoroutine(FadeIn(gameOverMusic, 1f));
        }

        // UI
        if (deathPanel != null)
            deathPanel.SetActive(true);

        if (deathMessageText != null)
            deathMessageText.text = "La foule a tué la sorcière !";

        UpdateRunsUI();

        player.SetSpeed(0);
        Time.timeScale = 0f;
    }

    // 🟢 Continuer
    public void ContinueAfterDeath()
    {
        if (currentLives <= 0) return;

        // 🔴 STOP musique game over
        if (gameOverMusic != null)
        {
            gameOverMusic.Stop();
            gameOverMusic.volume = 0f;
        }

        // 🟢 RELANCE musique gameplay
        if (gameplayMusic != null)
        {
            gameplayMusic.volume = 0f;

            if (!gameplayMusic.isPlaying)
            {
                gameplayMusic.Play();
            }

            StartCoroutine(FadeIn(gameplayMusic, 1f));
        }

        currentLives--;
        UpdateLivesUI();

        if (currentLives > 0)
        {
            Time.timeScale = 1f;
            isGamePaused = false;

            if (deathPanel != null)
                deathPanel.SetActive(false);

            player.ResetPlayerPosition();

            if (crowd != null)
                crowd.ResetCrowd();

            player.SetSpeed(2);
        }
        else
        {
            // 🔥 GAME OVER FINAL

            int finalScore = distanceTracker.GetFinalScore();
            int highScore = PlayerPrefs.GetInt("HighScore", 0);

            if (finalScore > highScore)
            {
                PlayerPrefs.SetInt("HighScore", finalScore);
                PlayerPrefs.Save();
                Debug.Log("🔥 Nouveau High Score : " + finalScore);
            }

            Time.timeScale = 1f;
            SceneManager.LoadScene("Menu");
        }
    }

    // 🎧 Fade Out
    IEnumerator FadeOut(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    // 🎧 Fade In
    IEnumerator FadeIn(AudioSource audioSource, float duration)
    {
        audioSource.volume = 0f;
        audioSource.Play();

        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(0f, 1f, time / duration);
            yield return null;
        }
    }

    // 🔹 UI des runs
    void UpdateRunsUI()
    {
        if (runsDetailText == null || distanceTracker == null)
            return;

        List<int> runs = distanceTracker.GetRuns();

        string detail = "";

        for (int i = 0; i < runs.Count; i++)
        {
            detail += "Run " + (i + 1) + " : " + runs[i] + " m\n";
        }

        runsDetailText.text = detail;
    }

    // 🔵 Quitter
    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
