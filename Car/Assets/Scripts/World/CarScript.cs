    using UnityEngine;
using TMPro;
public class CarController : MonoBehaviour
{
    public float speedChange = 10f;
    public float MaxSpeed = 40f;
    public float backwardSpeed = 5f;
    public float rotationSpeed = 100f;
    public float actualspeed;
    public TMP_Text SpeedText;
    private Rigidbody rb;

    void Start()
    {
        SpeedText.text = "Speed: 0";
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -1, 0); // Adjust the center of mass for better stability
    }

    void FixedUpdate()
    {
        // Get input from player
        float moveInput = Input.GetAxis("Vertical");
        float rotationInput = Input.GetAxis("Horizontal");

        // Calculate movement based on input
        Vector3 movement = Vector3.zero;

            actualspeed = rb.velocity.magnitude;
            SpeedText.text = "Speed: " + (Mathf.Round(actualspeed)).ToString();
        if (actualspeed <= MaxSpeed)
        {
            if (Input.GetKey(KeyCode.W))
            {
                // Move the car forward

                rb.AddForce(transform.forward * speedChange * Time.deltaTime, ForceMode.Impulse);

            }
            else if (Input.GetKey(KeyCode.S))
            {
                // Move the car backward

                rb.AddForce(transform.forward * speedChange * -1 * Time.deltaTime, ForceMode.Impulse);

            }
        }

        // Apply force for movement
        rb.AddForce(movement);

        // Rotate the car left/right
        float rotation = rotationInput * rotationSpeed * Time.fixedDeltaTime;
        Quaternion deltaRotation = Quaternion.Euler(Vector3.up * rotation);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }
}
