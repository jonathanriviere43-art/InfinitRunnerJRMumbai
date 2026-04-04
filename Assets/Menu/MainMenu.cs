using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button startButton;
    public Button controlsButton;
    public Button quitButton;
    public Button chaudronButton;
    public Button returnChaudronButton; // ✅ AJOUT

    [Header("Panels")]
    public GameObject controlsPanel;
    public GameObject chaudronPanel;

    private void Start()
    {
        startButton.onClick.AddListener(StartGame);
        controlsButton.onClick.AddListener(OpenControls);
        quitButton.onClick.AddListener(QuitGame);
        chaudronButton.onClick.AddListener(OpenChaudron);

        // ✅ bouton retour
        if (returnChaudronButton != null)
            returnChaudronButton.onClick.AddListener(CloseChaudron);
    }

    private void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    private void OpenControls()
    {
        if (controlsPanel != null)
            controlsPanel.SetActive(true);
    }

    private void OpenChaudron()
    {
        if (chaudronPanel != null)
            chaudronPanel.SetActive(true);
    }

    // ✅ NOUVELLE FONCTION
    private void CloseChaudron()
    {
        if (chaudronPanel != null)
            chaudronPanel.SetActive(false);
    }

    private void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}