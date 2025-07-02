using UnityEngine;
using UnityEngine.SceneManagement;

public class CASMenuController : MonoBehaviour
{
    public void BackToMainMenu()
    {
        // Load main menu after small delay (to allow fade out)
        StartCoroutine(DelayedSceneLoad());
    }

    private System.Collections.IEnumerator DelayedSceneLoad()
    {
        yield return new WaitForSeconds(2f); // match fadeDuration
        SceneManager.LoadScene("MainMenu");  // back to main menu
    }
}
