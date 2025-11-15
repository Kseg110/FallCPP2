using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    Vector3 moveInput;
    public float moveSpeed = 8f;
    public float headHeight = 1.8f;
    private Pickup nearbyPickup;

    Enemy lastFrozenEnemy = null;
    public Transform cameraTransform; // Assign camera in the player inspector for synced movement 

    private CharacterController cc;
    private Camera mainCamera;
    private Animator anim;
    private WeaponBase curWeapon = null;

    [SerializeField] private Transform weaponAttachPoint;

    LayerMask weaponLayerMask;
    LayerMask enemyLayerMask;

    private float curSpeed = 2.0f;
    [Header("Movement Settings")]
    [SerializeField] private float initSpeed = 2.0f;
    [SerializeField] private float maxSpeed = 15.0f;
    [SerializeField] private float moveAccel = 2f;
    [SerializeField] private float rotationSpeed = 5.0f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] private float timeToJumpApex = 0.4f;

    //Movement variables
    float gravity;
    float initialJumpVelocity;

    Vector2 direction; //direction of movement - no gravity is applied here
    Vector3 velocity;

    bool jumpPressed = false;

    void Start()
    {
        //rb = GetComponent<Rigidbody>();
        try
        {
            cc = GetComponent<CharacterController>();
            anim = GetComponentInChildren<Animator>();
            if (cc == null) throw new UnassignedReferenceException("CharacterController component is not assigned!");
        }
        catch (UnassignedReferenceException e)
        {
            //do something here
            UnityEngine.Application.Quit();
        }
        finally
        {
            //this code always runs after the try-catch block no matter if an exeption was thrown or not
        }

        mainCamera = Camera.main;
        CalculateJumpVariables();

        //Layer 6 is Weapons
        //LayerMask.GetMask("Weapon") seems to be inconsistent (in testing it was pulling layer 64) - setting it directly works better
        weaponLayerMask = 6;
        enemyLayerMask = 3;

        InputManager.Instance.OnMoveEvent += OnMove;
        InputManager.Instance.OnJumpEvent += OnJump;
        InputManager.Instance.OnDropEvent += OnDrop;
    }

    void OnValidate()
    {
        CalculateJumpVariables();
    }

    public void OnJump(bool pressed) => jumpPressed = pressed;
    public void OnMove(Vector2 movementDir) => direction = movementDir;

    public void OnDrop(bool pressed)
    {
        if (pressed && curWeapon != null)
        {
            curWeapon.Drop(GetComponent<Collider>());
            curWeapon = null;
        }
    }
    // Update is called once per frame
    void Update()
    {
        // input axes
        //float hInput = Input.GetAxis("Horizontal");
        //float vInput = Input.GetAxis("Vertical");

        // camera direction synced to movement 
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        //moveInput = (camForward * vInput + camRight * hInput).normalized;
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

        if (nearbyPickup != null && Input.GetKeyDown(KeyCode.E))
        {
            nearbyPickup.OnPickup();
            Destroy(nearbyPickup.gameObject);
            nearbyPickup = null;
        }

    }

    private void FixedUpdate()
    {
        //Vector3 moveVelocity = moveInput * moveSpeed;
        //rb.linearVelocity = new Vector3(moveVelocity.x, rb.angularVelocity.y, moveVelocity.z);
        //apply movement
        Vector3 projectedMoveDir = ProjectedMoveDirection();
        UpdateCharacterVelocity(projectedMoveDir);

        cc.Move(velocity * Time.fixedDeltaTime);
        anim.SetFloat("speed", curSpeed / maxSpeed);

        //apply rotation
        if (direction != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(projectedMoveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BOX"))
        {
            // End the game: reload the scene (or implement your own game over logic)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        // Detect Pickup
        Pickup pickup = other.GetComponent<Pickup>();
        if (pickup != null)
        {
            nearbyPickup = pickup;
        }

    }
    private void OnTriggerExit(Collider other)
    {
        Pickup pickup = other.GetComponent<Pickup>();
        if (pickup != null && pickup == nearbyPickup)
        {
            nearbyPickup = null;
        }
    }

    #region MovementCalculations
    private void UpdateCharacterVelocity(Vector3 projectedMoveDir)
    {
        if (direction == Vector2.zero) curSpeed = 0.0f;
        else if (curSpeed == 0.0f) curSpeed = initSpeed;
        else curSpeed = Mathf.MoveTowards(curSpeed, maxSpeed, moveAccel * Time.fixedDeltaTime);

        velocity.x = projectedMoveDir.x * curSpeed;
        velocity.z = projectedMoveDir.z * curSpeed;

        if (!cc.isGrounded) velocity.y += gravity * Time.fixedDeltaTime;
        else velocity.y = CheckJump();

    }
    private Vector3 ProjectedMoveDirection()
    {
        Vector3 cameraFwd = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        cameraFwd.y = 0;
        cameraRight.y = 0;

        cameraFwd.Normalize();
        cameraRight.Normalize();

        return cameraFwd * direction.y + cameraRight * direction.x;
    }
    #endregion


    #region JumpCalculations
    float CheckJump() => jumpPressed ? initialJumpVelocity : -cc.skinWidth;
    void CalculateJumpVariables()
    {
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        initialJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }
    #endregion
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Debug.Log("Hit Object:" + hit.gameObject.name);

        if (hit.gameObject.layer == enemyLayerMask)
        {
            Debug.Log("Hit an enemy!");
        }

        if (hit.gameObject.layer == weaponLayerMask && curWeapon == null)
        {
            WeaponBase weapon = hit.collider.GetComponent<WeaponBase>();
            if (weapon != null)
            {
                curWeapon = weapon;
                curWeapon.Equip(GetComponent<Collider>(), weaponAttachPoint);
            }

            Debug.Log($"Picked up a weapon! {weapon}");
        }
    }
}
