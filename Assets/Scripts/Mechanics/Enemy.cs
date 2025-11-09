using JetBrains.Annotations;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    Rigidbody rb;
    Vector3 movement;
    public float moveSpeed = 1.0f;
    private float headHeight = 1.8f;

    private Transform targetPlayer;
    private bool isFrozen = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");

        //Raycast for targeting 
        Vector3 rayOrigin = transform.position + Vector3.up * headHeight;
        Ray newRay = new Ray(rayOrigin, transform.forward);
        RaycastHit hitInfo;

        Debug.DrawRay(newRay.origin, newRay.direction * 20f, Color.yellow, 0.5f);
        if (Physics.Raycast(newRay, out hitInfo, 10.0f, LayerMask.GetMask("Player")))
        {
            Debug.Log("Player in enemy line of sight: " + hitInfo.collider.name);
            targetPlayer = hitInfo.collider.transform;
        }

        //Move towards the player if locked on
        if (targetPlayer != null)
        {
            Vector3 direction = (targetPlayer.position - transform.position).normalized;
            Vector3 moveStep = direction * moveSpeed * Time.deltaTime;
            rb.MovePosition(transform.position + moveStep);
        }

        
        if (isFrozen)
        { 
            //prevent movement and other actions while frozen
            rb.linearVelocity = Vector3.zero;
            return;
        }
        //normalize enemy behaviour (lock on, follow, etc)
        rb.linearVelocity = transform.forward * moveSpeed;
    }

        public void Freeze(bool freeze)
    {
        isFrozen = freeze;
        if (freeze)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
