using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    public float speed = 12f;
    public float gravity = -9.8f * 2;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;

    bool isGround;
    //bool isMoving;

    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if on the ground
        isGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGround && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Getting the inputs
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Creating the moving vector
        Vector3 move = transform.right * x + transform.forward * z;

        // Real move
        controller.Move (move * speed * Time.deltaTime);

        // Check if the player can jump
        if (Input.GetButtonDown("Jump") && isGround)
        {
            // Jumping
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Falling down
        velocity.y += gravity * Time.deltaTime;
        // Apply the Jump
        controller.Move(velocity * Time.deltaTime);

        /*
        if (lastPosition != gameObject.transform.position && isGround == true)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
        */
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue * new Color(1, 1, 1, 0.5f);
        Gizmos.DrawSphere(groundCheck.position, groundDistance);
    }
}
