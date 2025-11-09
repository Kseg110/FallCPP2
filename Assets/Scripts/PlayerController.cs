using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    Vector3 moveInput;
    public float moveSpeed = 8f;
    public float headHeight = 1.8f;

    Enemy lastFrozenEnemy = null;
    public Transform cameraTransform; // Assign camera in the player inspector for synced movement 

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

        // camera direction synced to movement 
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        moveInput = (camForward * vInput + camRight * hInput).normalized;
        //moveInput = new Vector3(hInput, 0f, vInput);

        //Raycast
        Vector3 rayOrigin = transform.position + Vector3.up * headHeight;
        Ray newRay = new Ray(rayOrigin, transform.forward);
        RaycastHit hitInfo;

        Debug.DrawRay(newRay.origin, newRay.direction * 10f, Color.red, 0.2f);

        if (Physics.Raycast(newRay, out hitInfo, 10.0f, LayerMask.GetMask("Enemy")))
        {
            Debug.Log("Enemy in front of player: " + hitInfo.collider.name);
            Enemy enemy = hitInfo.collider.GetComponent<Enemy>();
            if (enemy != null) 
            {
                enemy.Freeze(true);
                lastFrozenEnemy = enemy;
            }
        }
        else
        {
            if (lastFrozenEnemy != null)
            {
                lastFrozenEnemy.Freeze(false);
                lastFrozenEnemy = null;
            }
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
