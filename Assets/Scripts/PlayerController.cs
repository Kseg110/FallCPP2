using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    Vector3 moveInput;
    public float moveSpeed = 8f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // input axes
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");
        moveInput = new Vector3(hInput, 0f, vInput);

    }

    private void FixedUpdate()
    {
        Vector3 moveVelocity = moveInput * moveSpeed;
        rb.linearVelocity = new Vector3(moveVelocity.x, rb.angularVelocity.y, moveVelocity.z);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BOX"))
        {
            // End the game: reload the scene (or implement your own game over logic)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    }
}
