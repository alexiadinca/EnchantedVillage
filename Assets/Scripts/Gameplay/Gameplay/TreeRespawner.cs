// TreeRespawner.cs
using UnityEngine;
using System.Collections;

public class TreeRespawner : MonoBehaviour
{
    public static TreeRespawner Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Schedule a new tree to appear after <delaySeconds>.
    /// </summary>
    public void ScheduleRespawn(GameObject treePrefab, Vector3 position, Quaternion rotation, float delaySeconds)
    {
        StartCoroutine(RespawnCoroutine(treePrefab, position, rotation, delaySeconds));
    }

    IEnumerator RespawnCoroutine(GameObject prefab, Vector3 pos, Quaternion rot, float delay)
    {
        yield return new WaitForSeconds(delay);          // 300s = 5 real-minutes
        Instantiate(prefab, pos, rot);
    }
}
