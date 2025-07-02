using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject bagPanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
            ToggleInventory();
    }

    // called from the UI button
    public void ToggleInventory()
    {
        bool nowOn = !bagPanel.activeSelf;
        bagPanel.SetActive(nowOn);
    }
}
