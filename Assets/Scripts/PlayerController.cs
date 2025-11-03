using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    Vector3 moveInput;
    public float moveSpeed = 8f;

    // void Awake()
    //{
    //    input = new InputSystem_Actions();
    //    input.Player.SetCallbacks(this);
    //}


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

        Ray newRay = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;

        Debug.DrawRay(newRay.origin, newRay.direction * 10f, Color.red, 0.1f);

        if (Physics.Raycast(newRay, out hitInfo, 10.0f, LayerMask.GetMask("Enemy")))
        {
            Debug.Log("Enemy in front of player: " + hitInfo.collider.name);
        }


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
