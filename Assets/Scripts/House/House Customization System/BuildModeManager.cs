using UnityEngine;

public class BuildModeManager : MonoBehaviour
{
    public static bool IsInBuildMode { get; private set; }

    public GameObject buildUI;
    public ThirdPersonMovement playerMovement;

    void Start()
    {
        // Make sure we do NOT start in Build Mode
        IsInBuildMode = false;

        if (buildUI != null)
            buildUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleBuildMode(!IsInBuildMode);
        }
    }

    void ToggleBuildMode(bool state)
    {
        IsInBuildMode = state;

        if (buildUI != null)
            buildUI.SetActive(state);

        if (playerMovement != null)
            playerMovement.enabled = !state;

        Cursor.visible = state;
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
