using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 500f;

    float xRotation = 0f;

    public float topClamp = -90f;
    public float bottomClamp = 90f;

    public Transform cameraHolder;

    void Start()
    {
        // Locking the cursor and making it invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Getting the mouse inputs
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Look up and down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);

        // Apply rotation to camera holder for up/down look
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0, 0);

        // Rotate the entire player object left/right
        transform.Rotate(Vector3.up * mouseX);
    }
}
