/*
using UnityEngine;

[AddComponentMenu("Camera/Mouse Orbit 3rd Person Cam")]
public class ThirdPersonCam : MonoBehaviour
{
    [Tooltip("The character to orbit around")]
    public Transform target;

    [Header("Distance")]
    [Tooltip("How far behind the target the camera stays")]
    public float distance = 8f;

    [Header("Mouse Look")]
    [Tooltip("Mouse X sensitivity")]
    public float sensitivityX = 10f;
    [Tooltip("Mouse Y sensitivity")]
    public float sensitivityY = 10f;
    [Tooltip("Lowest vertical angle (degrees)")]
    public float minPitch = -20f;
    [Tooltip("Highest  vertical angle (degrees)")]
    public float maxPitch = 80f;

    float yaw = 0f;
    float pitch = 20f;

    void Start()
    {
        if (target == null)
            target = GameObject.FindWithTag("Player")?.transform;

        // Initialize angles so we start at a sensible spot
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
        // Snap camera into position
        UpdateCameraPosition();
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1) Read mouse input
        yaw += Input.GetAxis("Mouse X") * sensitivityX;
        pitch -= Input.GetAxis("Mouse Y") * sensitivityY;

        // 2) Clamp vertical look
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // 3) Reposition & rotate
        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        // Build a rotation from our yaw/pitch
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0);

        // Position the camera at: target + (rotated back-vector * distance)
        Vector3 pos = target.position + rot * new Vector3(0, 0, -distance);

        transform.rotation = rot;
        transform.position = pos;
    }
}
*/

using UnityEngine;

[AddComponentMenu("Camera/Mouse Orbit 3rd Person Cam")]
public class ThirdPersonCam : MonoBehaviour
{
    [Tooltip("The character to orbit around")]
    public Transform target;

    [Header("Distance")]
    [Tooltip("How far behind the target the camera stays")]
    public float distance = 8f;

    [Header("Mouse Look")]
    [Tooltip("Mouse X sensitivity")]
    public float sensitivityX = 10f;
    [Tooltip("Mouse Y sensitivity")]
    public float sensitivityY = 10f;
    [Tooltip("Lowest vertical angle (degrees)")]
    public float minPitch = -20f;
    [Tooltip("Highest  vertical angle (degrees)")]
    public float maxPitch = 80f;

    float yaw = 0f;
    float pitch = 20f;

    void Start()
    {
        // Try auto‐assign by tag if nothing set in Inspector
        if (target == null)
        {
            var go = GameObject.FindWithTag("Player");
            if (go != null)
                target = go.transform;
        }

        if (target == null)
        {
            Debug.LogWarning("[ThirdPersonCam] No target assigned or found with tag 'Player'. Please assign one in the Inspector.", this);
            return;
        }

        // Initialize yaw/pitch from current rotation
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        // Snap camera into initial position
        UpdateCameraPosition();
    }

    void LateUpdate()
    {
        if (target == null) return;  // bail out early

        // 1) Mouse look deltas
        yaw += Input.GetAxis("Mouse X") * sensitivityX;
        pitch -= Input.GetAxis("Mouse Y") * sensitivityY;

        // 2) Clamp vertical angle
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // 3) Move & rotate the camera
        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        if (target == null) return;  // extra safety guard

        // Build rotation from yaw/pitch
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);

        // Offset camera behind the target
        Vector3 offset = rot * Vector3.back * distance;
        transform.position = target.position + offset;
        transform.rotation = rot;
    }
}
