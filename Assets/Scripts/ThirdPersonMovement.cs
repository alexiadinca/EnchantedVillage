/*
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Speed in units/sec")]
    public float moveSpeed = 5f;

    [Header("Jump & Gravity")]
    [Tooltip("Height of the jump in units")]
    public float jumpHeight = 2f;
    [Tooltip("Gravity acceleration (negative)")]
    public float gravity = -9.81f;
    [Tooltip("Empty child used to check ground")]
    public Transform groundCheck;
    [Tooltip("Radius of the ground-check sphere")]
    public float groundDistance = 0.2f;
    [Tooltip("Which layers count as ground")]
    public LayerMask groundMask;

    CharacterController controller;
    Vector3 velocity;
    bool isGrounded;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (groundCheck == null)
            groundCheck = transform.Find("GroundCheck");
    }

    void Update()
    {
        // 1) Ground check
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundMask
        );
        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f; // stick to the ground

        // 2) Read input
        float x = Input.GetAxisRaw("Horizontal");   // A/D
        float z = Input.GetAxisRaw("Vertical");     // W/S

        // 3) Build camera-relative movement vector
        Vector3 camF = Camera.main.transform.forward;
        Vector3 camR = Camera.main.transform.right;
        camF.y = 0; camR.y = 0;
        camF.Normalize(); camR.Normalize();

        Vector3 moveDir = camR * x + camF * z;
        if (moveDir.magnitude >= 0.1f)
        {
            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }

        // 4) Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 5) Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
*/
/*
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Speed in units/sec")]
    public float moveSpeed = 5f;
    [Tooltip("How smooth the character turns (higher = slower)")]
    public float turnSmoothTime = 0.1f;

    [Header("Jump & Gravity")]
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    CharacterController controller;
    Vector3 velocity;
    float turnSmoothVel;
    bool isGrounded;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (groundCheck == null)
            groundCheck = transform.Find("GroundCheck");
    }

    void Update()
    {
        // 1) Ground check + stick to ground
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundMask
        );
        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        // 2) Read raw input
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(x, 0, z).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            // 3) Build camera‐relative move direction
            Vector3 camF = Camera.main.transform.forward;
            Vector3 camR = Camera.main.transform.right;
            camF.y = 0; camR.y = 0;
            camF.Normalize(); camR.Normalize();
            Vector3 moveDir = camR * x + camF * z;

            // 4) Smoothly rotate to face that direction
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                turnSmoothTime * Time.deltaTime
            );

            // 5) Move
            controller.Move(moveDir * moveSpeed * Time.deltaTime);
        }

        // 6) Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // 7) Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
*/

using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Speed in units/sec")]
    public float moveSpeed = 5f;
    [Tooltip("How smooth the character turns (lower = faster)")]
    public float turnSmoothTime = 0.1f;

    [Header("Jump & Gravity")]
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    CharacterController controller;
    Vector3 velocity;
    float turnSmoothVel;
    bool isGrounded;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (groundCheck == null)
            groundCheck = transform.Find("GroundCheck");
    }

    void Update()
    {
        // 1) Ground check + stick to ground
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundMask
        );
        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        // 2) Read input
        float x = Input.GetAxisRaw("Horizontal");   // A/D
        float z = Input.GetAxisRaw("Vertical");     // W/S
        Vector3 inputDir = new Vector3(x, 0, z).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            // 3) Camera-relative direction
            Vector3 camF = Camera.main.transform.forward;
            Vector3 camR = Camera.main.transform.right;
            camF.y = 0; camR.y = 0;
            camF.Normalize(); camR.Normalize();
            Vector3 moveDir = camR * x + camF * z;

            // 4) Smoothly rotate toward that direction
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref turnSmoothVel,
                turnSmoothTime
            );
            transform.rotation = Quaternion.Euler(0, angle, 0);

            // 5) Move the character
            controller.Move(moveDir * moveSpeed * Time.deltaTime);
        }

        // 6) Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 7) Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
