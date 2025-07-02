using System.Collections;
using UnityEngine;

public class FishingSystem : MonoBehaviour
{
    public static FishingSystem Instance { get; private set; }

    [HideInInspector] public bool canFish = false;
    public ItemData fishItem;
    public GameObject bobberPrefab; // optional visual bobber

    bool isFishing = false;
    float biteTime;
    GameObject currentBobber;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!canFish) return;

        // Press F to cast if not already fishing
        if (Input.GetKeyDown(KeyCode.F) && !isFishing)
            TryStartFishing();
        // Press F again to reel in
        else if (isFishing && Input.GetKeyDown(KeyCode.F))
            TryCatch();
    }

    void TryStartFishing()
    {
        Debug.Log("Casting: aim at water and press F to cast your line.");

        // Ray from camera through mouse
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int waterMask = LayerMask.GetMask("Water");

        // Draw a 100-unit cyan ray in the Scene view so you can see your aim
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.cyan, 1f);

        // Infinite‐distance raycast against Water layer
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, waterMask))
        {
            Debug.Log("Casting rod at water...");
            StartCoroutine(FishingRoutine(hit.point));
        }
        else
        {
            Debug.Log(
                "Aim at water and press F to cast.\n" +
                "Ensure your water object has a 3D Collider and is on the \"Water\" layer.\n" +
                "Watch the cyan debug-ray in the Scene view to see where you're pointing."
            );
        }
    }

    IEnumerator FishingRoutine(Vector3 waterPoint)
    {
        isFishing = true;
        biteTime = Random.Range(2f, 5f);

        // Spawn bobber if assigned
        if (bobberPrefab != null)
            currentBobber = Instantiate(bobberPrefab, waterPoint + Vector3.up * 0.2f, Quaternion.identity);

        Debug.Log("Casting... wait for a bite! (press F when it bites)");
        yield return new WaitForSeconds(biteTime);

        if (!isFishing) yield break;

        Debug.Log("Fish is biting! Press F to reel in!");

        float timer = 3f;
        while (timer > 0f && isFishing)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        // timed out
        if (isFishing)
        {
            Debug.Log("Fish got away…");
            isFishing = false;
            if (currentBobber != null) Destroy(currentBobber);
        }
    }

    void TryCatch()
    {
        if (!isFishing) return;

        if (!InventoryManager.I.HasItem(fishItem))
        {
            Debug.Log("Fish caught! Give it to Finn");
            InventoryManager.I.AddItem(fishItem, 1);
            Debug.Log($"[FishingSystem] Added {fishItem.name} to inventory.");
        }
        else
        {
            Debug.Log("You already have a fish! Go give it to Finn.");
        }

        isFishing = false;
        if (currentBobber != null) Destroy(currentBobber);
    }

}










/*
using System.Collections;
using UnityEngine;

public class FishingSystem : MonoBehaviour
{
    public static FishingSystem Instance { get; private set; }

    [HideInInspector] public bool canFish = false;
    public ItemData fishItem;
    public GameObject bobberPrefab; // optional visual bobber

    bool isFishing = false;
    float biteTime;
    GameObject currentBobber;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!canFish) return;

        // Press F to cast rod if not already fishing
        if (Input.GetKeyDown(KeyCode.F) && !isFishing)
        {
            TryStartFishing();
        }
        // Press F again to reel in if a fish is biting or rod is cast
        else if (isFishing && Input.GetKeyDown(KeyCode.F))
        {
            TryCatch();
        }
    }

    void TryStartFishing()
    {
        // Create a ray from the camera through the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int waterMask = LayerMask.GetMask("Water");

        // Draw a debug ray (cyan) in the Scene view so you can see where you're aiming
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.cyan, 1f);

        // Raycast infinitely until it hits an object on the Water layer
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, waterMask))
        {
            StartCoroutine(FishingRoutine(hit.point));
        }
        else
        {
            Debug.Log(
                "Aim at water and press F to cast.\n" +
                "Ensure your water object has a 3D Collider and is on the \"Water\" layer.\n" +
                "Watch the cyan debug-ray in the Scene view to see where you're pointing."
            );
        }
    }

    IEnumerator FishingRoutine(Vector3 waterPoint)
    {
        isFishing = true;
        biteTime = Random.Range(2f, 5f);

        // Spawn bobber if assigned
        if (bobberPrefab != null)
            currentBobber = Instantiate(bobberPrefab, waterPoint + Vector3.up * 0.2f, Quaternion.identity);

        Debug.Log("Casting... wait for a bite! (press F when it bites)");
        yield return new WaitForSeconds(biteTime);

        if (!isFishing)
            yield break;

        Debug.Log("Fish is biting! Press F to reel in!");

        // Wait up to 3 seconds for the player to press F
        float timer = 3f;
        while (timer > 0f && isFishing)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        // If timed out before catching
        if (isFishing)
        {
            Debug.Log("Fish got away…");
            isFishing = false;
            if (currentBobber != null) Destroy(currentBobber);
        }
    }

    void TryCatch()
    {
        if (!isFishing) return;

        Debug.Log("You caught a fish!");
        InventoryManager.I.AddItem(fishItem, 1);
        isFishing = false;
        if (currentBobber != null) Destroy(currentBobber);
    }
}
*/