using UnityEngine;

public class VillageBootstrap : MonoBehaviour
{
    [Header("Drag in your WelcomePopup component here")]
    public WelcomePopup welcomePopup;

    void Start()
    {
        Debug.Log("[VillageBootstrap] Start() called");
        if (welcomePopup == null)
        {
            Debug.LogError("[VillageBootstrap] welcomePopup reference is NULL!");
            return;
        }
        welcomePopup.Show();
    }
}
