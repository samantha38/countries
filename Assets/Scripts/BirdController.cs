using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BirdController : MonoBehaviour
{
    public Rigidbody2D myRigidbody;
    public float flapStrength = 5f; // Default flap strength
    public bool birdIsAlive = true;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>(); // Ensure Rigidbody2D is assigned
        myRigidbody.gravityScale = 1; // Enable gravity if it was disabled
        Debug.Log("Bird controller started");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && birdIsAlive) // Handles mouse click and touch
        {
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
