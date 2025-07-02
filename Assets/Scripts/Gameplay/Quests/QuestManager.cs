using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum QuestStatus { Locked, Available, InProgress, Completed }

[Serializable]
public class QuestData
{
    [Tooltip("This must match exactly what you type in your FinnFishingInteraction → Quest Id field (we’ll auto-trim & ignore case).")]
    public string id;
    public string title;
    public QuestStatus status;
    public int currentStep;
    public string[] stepDescriptions;
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("All Quests (populate in Inspector)")]
    public List<QuestData> quests = new List<QuestData>();

    Dictionary<string, QuestData> _byId;

    [Header("UI")]
    public TMP_Text activeQuestText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            BuildLookup();

            SceneManager.sceneLoaded += (scene, mode) => StartCoroutine(DelayedUIRebind());
        }
        else Destroy(gameObject);
    }

    IEnumerator DelayedUIRebind()
    {
        yield return null; // wait one frame
        var found = GameObject.Find("ActiveQuestText");
        if (found != null)
            activeQuestText = found.GetComponent<TMP_Text>();

        Debug.Log("[QuestManager] Rebound activeQuestText after scene load.");
    }

    void BuildLookup()
    {
        _byId = new Dictionary<string, QuestData>(StringComparer.OrdinalIgnoreCase);
        foreach (var q in quests)
        {
            if (string.IsNullOrWhiteSpace(q.id)) continue;

            var key = q.id.Trim();
            if (_byId.ContainsKey(key)) continue;

            _byId.Add(key, q);
        }
    }

    public QuestData Get(string id)
    {
        if (_byId == null) BuildLookup();

        if (string.IsNullOrWhiteSpace(id)) return null;

        var key = id.Trim();
        if (_byId.TryGetValue(key, out var q))
            return q;

        q = quests.FirstOrDefault(x =>
            !string.IsNullOrWhiteSpace(x.id) &&
            x.id.Trim().Equals(key, StringComparison.OrdinalIgnoreCase)
        );

        return q;
    }

    public void Unlock(string id)
    {
        var q = Get(id);
        if (q != null && q.status == QuestStatus.Locked)
            q.status = QuestStatus.Available;
    }

    public void StartQuest(string id)
    {
        var q = Get(id);
        if (q != null && q.status == QuestStatus.Available)
        {
            q.status = QuestStatus.InProgress;
            q.currentStep = 0;

            if (activeQuestText != null)
                activeQuestText.gameObject.SetActive(true);

            UpdateUI(q);
        }
    }

    public void AdvanceStep(string id)
    {
        var q = Get(id);
        if (q == null || q.status != QuestStatus.InProgress) return;

        q.currentStep++;
        if (q.currentStep >= q.stepDescriptions.Length)
        {
            q.status = QuestStatus.Completed;
            if (activeQuestText != null && activeQuestText.gameObject.activeInHierarchy)
            {
                activeQuestText.text = $"{q.title} Complete!";
                Invoke(nameof(HideActiveQuestText), 2.5f);
            }
        }
        else
        {
            UpdateUI(q);
        }
    }

    void UpdateUI(QuestData q)
    {
        if (activeQuestText == null) return;
        if (!activeQuestText.gameObject.activeInHierarchy) return;

        activeQuestText.text = $"{q.title}\n— {q.stepDescriptions[q.currentStep]}";
    }

    void HideActiveQuestText()
    {
        if (activeQuestText != null)
        {
            activeQuestText.text = "";
            activeQuestText.gameObject.SetActive(false);
        }
    }
}





/*
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum QuestStatus { Locked, Available, InProgress, Completed }

[Serializable]
public class QuestData
{
    [Tooltip("This must match exactly what you type in your FinnFishingInteraction → Quest Id field (we’ll auto-trim & ignore case).")]
    public string id;
    public string title;
    public QuestStatus status;
    public int currentStep;
    public string[] stepDescriptions;
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("All Quests (populate in Inspector)")]
    public List<QuestData> quests = new List<QuestData>();

    // internal lookup (case-insensitive)
    Dictionary<string, QuestData> _byId;

    [Header("UI")]
    public TMP_Text activeQuestText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            BuildLookup();

            // Rebind activeQuestText on scene load
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                if (activeQuestText == null || !activeQuestText)
                {
                    var found = GameObject.Find("ActiveQuestText");
                    if (found != null)
                        activeQuestText = found.GetComponent<TMP_Text>();
                }
            };
        }
        else Destroy(gameObject);
    }


    void BuildLookup()
    {
        // Use a case-insensitive comparer so “finn” or “Finn” map to the same key
        _byId = new Dictionary<string, QuestData>(StringComparer.OrdinalIgnoreCase);
        foreach (var q in quests)
        {
            if (string.IsNullOrWhiteSpace(q.id))
            {
                Debug.LogWarning("[QuestManager] Skipping quest with empty or whitespace ID");
                continue;
            }

            var key = q.id.Trim();
            if (_byId.ContainsKey(key))
            {
                Debug.LogWarning($"[QuestManager] Duplicate quest ID '{key}' – only the first will be used");
                continue;
            }

            _byId.Add(key, q);
        }

        Debug.Log($"[QuestManager] Registered quest IDs: {string.Join(", ", _byId.Keys)}");
    }

    /// <summary>Finds a quest by ID (auto-trims & ignores case), or logs exactly what’s registered if it fails.</summary>
    public QuestData Get(string id)
    {
        if (_byId == null) BuildLookup();

        if (string.IsNullOrWhiteSpace(id))
        {
            Debug.LogError("[QuestManager] Get() called with empty or whitespace ID");
            return null;
        }

        var key = id.Trim();
        if (_byId.TryGetValue(key, out var q))
            return q;

        // fallback linear search in case something weird slipped by
        q = quests.FirstOrDefault(x =>
            !string.IsNullOrWhiteSpace(x.id) &&
            x.id.Trim().Equals(key, StringComparison.OrdinalIgnoreCase)
        );
        if (q != null)
            return q;

        Debug.LogError(
            $"[QuestManager] No quest found with ID '{id}'.\n" +
            $"Registered IDs are: {string.Join(", ", _byId.Keys)}"
        );
        return null;
    }

    public void Unlock(string id)
    {
        var q = Get(id);
        if (q != null && q.status == QuestStatus.Locked)
            q.status = QuestStatus.Available;
    }

    public void StartQuest(string id)
    {
        var q = Get(id);
        if (q != null && q.status == QuestStatus.Available)
        {
            q.status = QuestStatus.InProgress;
            q.currentStep = 0;

            if (activeQuestText != null)
                activeQuestText.gameObject.SetActive(true);

            UpdateUI(q);
        }
    }


    public void AdvanceStep(string id)
    {
        var q = Get(id);
        if (q == null || q.status != QuestStatus.InProgress) return;

        q.currentStep++;
        if (q.currentStep >= q.stepDescriptions.Length)
        {
            q.status = QuestStatus.Completed;
            if (activeQuestText != null && activeQuestText.gameObject.activeInHierarchy)
            {
                activeQuestText.text = $"{q.title} Complete!";
                Invoke(nameof(HideActiveQuestText), 2.5f);
            }

        }
        else
        {
            UpdateUI(q);
        }
    }

    void UpdateUI(QuestData q)
    {
        if (activeQuestText == null)
        {
            Debug.LogWarning("[QuestManager] activeQuestText not assigned or was destroyed!");
            return;
        }

        if (!activeQuestText.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("[QuestManager] Tried to update a hidden or destroyed quest text.");
            return;
        }

        activeQuestText.text = $"{q.title}\n— {q.stepDescriptions[q.currentStep]}";
    }

    void HideActiveQuestText()
    {
        if (activeQuestText != null)
        {
            activeQuestText.text = "";
            activeQuestText.gameObject.SetActive(false);  // this hides the visual object safely
        }
    }


}
*/