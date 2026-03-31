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

    [Header("Speed UI")]
    public TMP_Text speedText;

    [Header("Score")]
    public DistanceTracker distanceTracker;

    [Header("Audio")]
    public AudioSource gameplayMusic;
    public AudioSource gameOverMusic;

    [Header("Chunk")]
    public ChunkManager chunkManager;

    private bool isGamePaused = false;

    private void Start()
    {
        currentLives = maxLives;
        UpdateLivesUI();

        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    private void Update()
    {
        if (chunkManager != null)
        {
            UpdateSpeedUI(chunkManager.GetSpeedMultiplier());
        }
    }

    private void UpdateLivesUI()
    {
        if (livesText != null)
            livesText.text = "Vies : " + currentLives;
    }

    void UpdateSpeedUI(float multiplier)
    {
        if (speedText != null)
        {
            speedText.text = "Vitesse x" + multiplier.ToString("F1");
        }
    }

    public void PlayerCaughtByCrowd()
    {
        if (isGamePaused) return;

        isGamePaused = true;

        if (distanceTracker != null)
            distanceTracker.SaveAndResetRun();

        if (gameplayMusic != null)
            StartCoroutine(FadeOut(gameplayMusic, 1f));

        if (gameOverMusic != null)
            StartCoroutine(FadeIn(gameOverMusic, 1f));

        if (deathPanel != null)
            deathPanel.SetActive(true);

        if (deathMessageText != null)
            deathMessageText.text = "La foule a tué la sorcière !";

        UpdateRunsUI();

        player.SetSpeed(0);
        Time.timeScale = 0f;
    }

    public void ContinueAfterDeath()
    {
        if (currentLives <= 0) return;

        if (gameOverMusic != null)
        {
            gameOverMusic.Stop();
            gameOverMusic.volume = 0f;
        }

        if (gameplayMusic != null)
        {
            gameplayMusic.volume = 0f;

            if (!gameplayMusic.isPlaying)
                gameplayMusic.Play();

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

            if (chunkManager != null)
                chunkManager.ResetSpeed();

            // ✅ RESET FUEL PROPRE
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.ResetFuel();
            }

            player.SetSpeed(2);
        }
        else
        {
            int finalScore = distanceTracker.GetFinalScore();
            int highScore = PlayerPrefs.GetInt("HighScore", 0);

            if (finalScore > highScore)
            {
                PlayerPrefs.SetInt("HighScore", finalScore);
                PlayerPrefs.Save();
            }

            Time.timeScale = 1f;
            SceneManager.LoadScene("Menu");
        }
    }

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

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
