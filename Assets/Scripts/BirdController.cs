using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class BirdController : MonoBehaviour
{
    public Rigidbody2D myRigidbody;
    public float flapStrength = 5f; // Default flap strength
    public bool birdIsAlive = true;
    public PlayerInput playerInput;
    public InputAction touchPressAction;

    //private float lowerBoundary; // Stores the bottom boundary position

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        touchPressAction = playerInput.actions["TouchPress"];
    }

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>(); // Ensure Rigidbody2D is assigned
        myRigidbody.gravityScale = 1; // Enable gravity if it was disabled
        Debug.Log("Bird controller started");
    }

    private void OnEnable()
    {
        touchPressAction.performed += TouchPressed;
        
    }

    private void TouchPressed(InputAction.CallbackContext context) {
        // float value = context.ReadValue<float>();
        // Debug.Log(value);
        myRigidbody.linearVelocity = Vector2.up * flapStrength;

    }

    private void OnDisable()
    {
        touchPressAction.performed -= TouchPressed;
    }

    
    private void Update() {
        if(touchPressAction.WasPerformedThisFrame() && birdIsAlive) {
             myRigidbody.linearVelocity = Vector2.up * flapStrength; // Apply upward force
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Collision detected with: {other.gameObject.name}");
        if (other.CompareTag("Letter"))
        {
            Debug.Log("Letter collected!");
            GameManager.Instance.CollectLetter(other.gameObject.GetComponent<LetterBehavior>().Letter);
            Destroy(other.gameObject);
        }
    }
}
