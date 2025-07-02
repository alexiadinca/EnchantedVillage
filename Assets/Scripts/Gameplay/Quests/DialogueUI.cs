/*
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

[DefaultExecutionOrder(-1000)]
public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    [Header("UI References")]
    public GameObject panel;
    public TMP_Text messageText;

    IEnumerator WaitUntilActiveThenRelink()
    {
        while (!gameObject.activeInHierarchy)
        {
            yield return null;
        }
        yield return null; // wait an additional frame

        yield return StartCoroutine(WaitAndRelinkUI());
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            StartCoroutine(WaitUntilActiveThenRelink());

            SceneManager.sceneLoaded += (scene, mode) =>
            {
                StartCoroutine(WaitUntilActiveThenRelink());
            };
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(WaitAndRelinkUI());
    }

    IEnumerator WaitAndRelinkUI()
    {
        while (panel == null || messageText == null)
        {
            if (panel == null)
            {
                panel = GameObject.Find("PanelDialogue");
                if (panel == null)
                    Debug.LogWarning("[DialogueUI] Waiting for 'PanelDialogue' GameObject in scene...");
            }

            if (messageText == null)
            {
                var foundText = GameObject.Find("MessageText");
                if (foundText != null)
                    messageText = foundText.GetComponent<TMP_Text>();
                else
                    Debug.LogWarning("[DialogueUI] Waiting for 'MessageText' GameObject in scene...");
            }

            yield return null;
        }

        panel.SetActive(false);
        Debug.Log("[DialogueUI] UI references re-linked successfully.");
    }

    public void Show(string msg)
    {
        if (panel == null || messageText == null)
        {
            Debug.LogError("[DialogueUI] Cannot show dialogue – panel or messageText is missing!");
            return;
        }

        messageText.text = msg;
        panel.SetActive(true);
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}
*/




using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

[DefaultExecutionOrder(-1000)]
public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    [Header("UI References")]
    public GameObject panel;
    public TMP_Text messageText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            gameObject.SetActive(true);  // Ensure active for coroutines

            SceneManager.sceneLoaded += (scene, mode) =>
            {
                if (gameObject.activeInHierarchy)
                    StartCoroutine(DelayedRelink());
                else
                    Debug.LogWarning("[DialogueUI] Cannot start coroutine because DialogueUI is inactive.");
            };
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        TryRelinkUI();
    }

    System.Collections.IEnumerator DelayedRelink()
    {
        yield return null;  // Wait one frame for UI to be ready
        TryRelinkUI();
    }



    void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= (scene, mode) => StartCoroutine(DelayedRelink());
    }

    void TryRelinkUI()
    {
        if (panel == null || !panel)
            panel = GameObject.Find("PanelDialogue");

        if (messageText == null || !messageText)
        {
            var foundText = GameObject.Find("MessageText");
            if (foundText != null)
                messageText = foundText.GetComponent<TMP_Text>();
        }

        if (panel != null)
            panel.SetActive(false);

        Debug.Log("[DialogueUI] UI references re-linked after scene load.");
    }

    public void Show(string msg)
    {
        if (panel == null || messageText == null)
        {
            Debug.LogError("[DialogueUI] Cannot show dialogue – panel or messageText is missing!");
            return;
        }

        messageText.text = msg;
        panel.SetActive(true);
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}
