using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;      // Velocidad de movimiento
    public float gravity = -9.81f;    // Gravedad
    public float jumpHeight = 1.5f;   // Altura de salto opcional

    private CharacterController controller;
    private Vector3 velocity;
    private Camera mainCam;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCam = Camera.main;
    }

    void Update()
    {
        // --- Movimiento en plano XZ ---
        float h = Input.GetAxis("Horizontal"); // A-D / Flechas
        float v = Input.GetAxis("Vertical");   // W-S / Flechas

        // Dirección según cámara
        Vector3 forward = mainCam.transform.forward;
        Vector3 right   = mainCam.transform.right;
        forward.y = 0;
        right.y   = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * v + right * h;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // --- Salto opcional ---
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // --- Gravedad ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
