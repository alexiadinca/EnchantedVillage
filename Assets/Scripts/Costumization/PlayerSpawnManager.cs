using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    [Header("Animal Prefabs (in same order as AnimalIndex)")]
    [SerializeField] private GameObject[] animalPrefabs;

    [Header("Default Spawn Position (used when coming from CAS)")]
    [Tooltip("World-space coordinates where the player should appear")]
    [SerializeField] private Vector3 spawnPosition = new Vector3(126.4f, 8.079f, 491.5774f);

    void Start()
    {
        // Decide final position: use stored teleport data if available, otherwise use default (CAS)
        Vector3 finalSpawnPos = spawnPosition;

        if (PlayerPrefs.HasKey("TeleportFromHouse"))
        {
            finalSpawnPos = new Vector3(
                PlayerPrefs.GetFloat("SpawnX"),
                PlayerPrefs.GetFloat("SpawnY"),
                PlayerPrefs.GetFloat("SpawnZ")
            );

            // One-time use, clean up
            PlayerPrefs.DeleteKey("TeleportFromHouse");
        }

        // Clamp & grab the right prefab
        int idx = Mathf.Clamp(PlayerPrefs.GetInt("AnimalIndex", 0), 0, animalPrefabs.Length - 1);

        // Instantiate at the final position
        var player = Instantiate(animalPrefabs[idx], finalSpawnPos, Quaternion.identity);
        player.tag = "Player";

        // Apply customization if any
        var loader = player.GetComponent<PlayerCustomizationLoader>();
        if (loader != null) loader.LoadFromPrefs();

        // Hook up the camera
        var cam = Camera.main.GetComponent<ThirdPersonCam>();
        if (cam != null) cam.target = player.transform;
    }
}




/*
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    [Header("Animal Prefabs (in same order as AnimalIndex)")]
    [SerializeField] private GameObject[] animalPrefabs;

    [Header("Exact Spawn Position")]
    [Tooltip("World-space coordinates where the player should appear")]
    [SerializeField] private Vector3 spawnPosition = new Vector3(126.4f, 8.079f, 491.5774f);

    void Start()
    {
        // Clamp & grab the right prefab
        int idx = Mathf.Clamp(PlayerPrefs.GetInt("AnimalIndex", 0), 0, animalPrefabs.Length - 1);

        // Instantiate at the fixed world position
        var player = Instantiate(
            animalPrefabs[idx],
            spawnPosition,
            Quaternion.identity
        );
        player.tag = "Player";

        // Apply customization if any
        var loader = player.GetComponent<PlayerCustomizationLoader>();
        if (loader != null) loader.LoadFromPrefs();

        // Hook up the camera
        var cam = Camera.main.GetComponent<ThirdPersonCam>();
        if (cam != null) cam.target = player.transform;
    }
}
*/