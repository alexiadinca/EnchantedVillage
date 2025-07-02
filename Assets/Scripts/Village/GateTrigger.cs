using UnityEngine;

public class GateTrigger : MonoBehaviour
{
    public Transform gateToRotate;
    public float openAngle = -90f;     // 90 (closed) -> 0 (open)
    public float openSpeed = 2f;

    private Quaternion targetRotation;
    private bool isOpen = false;

    void Start()
    {
        if (gateToRotate != null)
        {
            float currentY = gateToRotate.eulerAngles.y;
            targetRotation = Quaternion.Euler(0f, currentY + openAngle, 0f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Gate trigger hit by: {other.name}");

        if (!isOpen && other.CompareTag("Player"))
        {
            Debug.Log("Opening gate!");
            isOpen = true;
        }
    }

    void Update()
    {
        if (isOpen && gateToRotate != null)
        {
            gateToRotate.rotation = Quaternion.Slerp(
                gateToRotate.rotation,
                targetRotation,
                Time.deltaTime * openSpeed
            );
        }
    }
}
