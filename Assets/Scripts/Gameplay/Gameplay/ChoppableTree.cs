using UnityEngine;

public class ChoppableTree: MonoBehaviour
{
    [Tooltip("Drag your Tree prefab asset here")]
    public GameObject treePrefab;   // reference to the original prefab

    public int woodAmount = 3;
    public GameObject woodDropPrefab;

    // how many real-seconds before respawn
    const float respawnDelay = 300f;

    public void Chop()
    {
        Debug.Log($"Chop() called on: {gameObject.name}");

        // Spawn multiple planks/logs
        for (int i = 0; i < woodAmount; i++)
        {
            Vector3 spawnOffset = new Vector3(Random.Range(-0.3f, 0.3f), 0, Random.Range(-0.3f, 0.3f));
            Instantiate(woodDropPrefab, transform.position + spawnOffset, Quaternion.identity);
        }

        // Schedule tree to respawn
        TreeRespawner.Instance.ScheduleRespawn(treePrefab, transform.position, transform.rotation, 300f);

        // Destroy the current tree
        Destroy(gameObject);
    }

}
