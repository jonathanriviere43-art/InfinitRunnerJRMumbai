using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject pauseMenuUI;

    private PlayerControls controls;
    private bool isPaused = false;

    private void Awake()
    {
        controls = new PlayerControls();

        // Quand on appuie sur Échap
        controls.UI.Pause.performed += ctx => TogglePause();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    // 🔁 Toggle pause
    void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    // ▶️ Reprendre
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    // ⏸️ Pause
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    // ❌ Quitter → Menu principal
    public void QuitToMenu()
    {
        Time.timeScale = 1f; // très important
        SceneManager.LoadScene("Menu"); // ⚠️ mets le bon nom ici
    }
}
