using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject settingsPanel;

    public void NewGame()
    {
        SceneManager.LoadScene("CharacterCustomization");
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("Village");

    }

    public void OpenSettings()
    {

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void CloseSettings()
    {

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting..."); 
    }
}
