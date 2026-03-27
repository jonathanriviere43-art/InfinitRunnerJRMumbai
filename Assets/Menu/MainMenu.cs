using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button startButton;
    public Button controlsButton;
    public Button quitButton;

    private void Start()
    {
        // Associer les fonctions aux boutons
        startButton.onClick.AddListener(StartGame);
        controlsButton.onClick.AddListener(OpenControls);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void StartGame()
    {
        // Charger la scène de jeu (change "GameScene" par le nom exact de ta scène)
        SceneManager.LoadScene("SampleScene");
    }

    private void OpenControls()
    {
        // Ici juste un debug, pas de panel
        Debug.Log("Bouton Commandes cliqué !");
    }

    private void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop dans l'éditeur
        #else
        Application.Quit(); // Quitte le jeu buildé
        #endif
    }
}
