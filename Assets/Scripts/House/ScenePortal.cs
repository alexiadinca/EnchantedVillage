using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePortal : MonoBehaviour
{
    public string targetScene;
    public Vector3 spawnInTargetScene;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered trigger: " + other.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected, loading scene: " + targetScene);
            PlayerPrefs.SetFloat("SpawnX", spawnInTargetScene.x);
            PlayerPrefs.SetFloat("SpawnY", spawnInTargetScene.y);
            PlayerPrefs.SetFloat("SpawnZ", spawnInTargetScene.z);
            PlayerPrefs.SetInt("TeleportFromHouse", 1);  // important flag
            PlayerPrefs.Save();

            SceneManager.LoadScene(targetScene);
        }
    }
}
