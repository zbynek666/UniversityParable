using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;



public class Movement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -10f;
    public float jumpHeight = 2f;

    InputAction movement;
    InputAction jump;


    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    

    




    public float mouseSensitivity = 100f;

    public Transform playerBody;
    public Transform playerHead;


    float xRotation = 0f;

    public CapsuleCollider capsuleCollider;


    Vector3 velocity;
    bool isGrounded;
    private float distToGround;

    private void Start()
    {
        movement = new InputAction("PlayerMovement", binding: "<Gamepad>/leftStick");

        movement.AddCompositeBinding("Dpad")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        jump = new InputAction("PlayerJump", binding: "<Gamepad>/a");
        jump.AddBinding("<Keyboard>/space");

        movement.Enable();
        jump.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        distToGround = capsuleCollider.bounds.extents.y;
    }
    void OnCollisionEnter(Collision col)
    {

        Debug.Log("kys");

    }
    void Update()
    {
        float x;
        float z;
        bool jumpPressed = false;

        var delta = movement.ReadValue<Vector2>();

        //add vr movement and cup at 1

        
        delta.x = Mathf.Min(delta.x, 1);
        delta.y = Mathf.Min(delta.y, 1);



        x = delta.x;
        z = delta.y;
        jumpPressed = Mathf.Approximately(jump.ReadValue<float>(), 1);

        //isGrounded = Physics.BoxCast(groundCheck.position, new Vector3(1, 1, 1), Vector3.forward,new Quaternion(),1.0f);
        isGrounded = IsGrounded();
        
        Debug.Log(isGrounded);
        /*  
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        */


        Vector3 move = transform.right * x + transform.forward * z;
        move *= isGrounded ? 1: 0.5f;
        controller.Move(move * speed * Time.deltaTime);


        if (jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        Looking();
    }



    private void Looking()
    {
#if ENABLE_INPUT_SYSTEM
        float mouseX = 0, mouseY = 0;

        if (Mouse.current != null)
        {
            var delta = Mouse.current.delta.ReadValue() / 15.0f;
            mouseX += delta.x;
            mouseY += delta.y;
        }
        if (Gamepad.current != null)
        {
            var value = Gamepad.current.rightStick.ReadValue() * 2;
            mouseX += value.x;
            mouseY += value.y;
        }

        mouseX *= mouseSensitivity * Time.deltaTime;
        mouseY *= mouseSensitivity * Time.deltaTime;
#else
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
#endif




        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerHead.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);
    }
    private bool IsGrounded() {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.3f);
    }
}
