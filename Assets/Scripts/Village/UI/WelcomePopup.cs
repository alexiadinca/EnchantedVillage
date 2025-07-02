using UnityEngine;
using UnityEngine.UI;
using System;

public class WelcomePopup : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The full-screen panel that dims the background and contains the welcome UI")]
    public GameObject panel;

    [Tooltip("The button the player clicks to dismiss the welcome popup")]
    public Button dismissButton;

    /// <summary>
    /// Fired when the popup is dismissed. Subscribe to this to spawn the player & start the first quest.
    /// </summary>
    public event Action OnDismissed;

    void Awake()
    {
        panel.SetActive(false);
        dismissButton.onClick.AddListener(Dismiss);
    }

    /// <summary>
    /// Show the panel (blocks any invisible input) until Continue.
    /// </summary>
    public void Show()
    {
        Debug.Log("[WelcomePopup] Show() called");
        panel.SetActive(true);
    }
    private void Dismiss()
    {
        Debug.Log("[WelcomePopup] Dismiss() called");
        panel.SetActive(false);
        OnDismissed?.Invoke();
    }

}
