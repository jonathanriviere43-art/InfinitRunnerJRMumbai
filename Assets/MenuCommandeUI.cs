using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public GameObject commandesPanel;

    public void OuvrirCommandes()
    {
        commandesPanel.SetActive(true);
    }

    public void FermerCommandes()
    {
        commandesPanel.SetActive(false);
    }
}
