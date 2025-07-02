using UnityEngine;

public class ProximityTest : MonoBehaviour
{
    public float radius = 3f;
    private Transform playerT;

    void Update()
    {
        // Find the player once
        if (playerT == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go == null)
            {
                Debug.Log("[ProximityTest] No Player tagged object found");
                return;
            }
            playerT = go.transform;
            Debug.Log("[ProximityTest] Found Player at runtime");
        }

        // Log distance every frame
        float d = Vector3.Distance(playerT.position, transform.position);
        Debug.Log($"[ProximityTest] Distance to player = {d:F2}");

        // Indicate when within radius
        if (d <= radius)
            Debug.Log($"[ProximityTest] ENTER radius {radius}");
    }
}
