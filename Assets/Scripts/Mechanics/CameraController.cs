using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player; //Assign the player Prefab in the inspector 
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;   
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Vertical Rotation or Pitch
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontal Rotation or Yaw
        player.Rotate(Vector3.up * mouseX);
    }
}
