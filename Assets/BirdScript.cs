using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BirdScript : MonoBehaviour
{
    public Rigidbody2D myRigidbody;  // Assign this in the Unity Inspector
    public float flapStrength = 10f;
    private bool birdIsAlive = true;

    public LogicScript gameLogic;
    public CountryNameDisplay countryManager;

    void Start()
    {   

        // Make sure the bird starts at a visible position
        transform.position = new Vector3(0f, 0f, 0f);  // Set initial position at (0, 0)

        if (myRigidbody == null)
        {
            Debug.LogError("Rigidbody2D is not assigned in the inspector!");
        }
        if (gameLogic == null)
        {
            Debug.LogError("LogicScript reference is missing!");
        }
        if (countryManager == null)
        {
            Debug.LogError("CountryNameDisplay reference is missing!");
        }
    }

    void Update()
    {
        if (birdIsAlive)
        {
            float tiltInput = Input.acceleration.x;
            myRigidbody.velocity = new Vector2(tiltInput * flapStrength, myRigidbody.velocity.y);

            // Prevent the bird from flying off the screen
            float clampedX = Mathf.Clamp(transform.position.x, -5f, 5f);  // X position boundary
            float clampedY = Mathf.Clamp(transform.position.y, -4f, 4f);  // Y position boundary
            transform.position = new Vector3(clampedX, clampedY, 0f);
        }

        if (Input.GetMouseButtonDown(0))
        {
            myRigidbody.velocity = Vector2.up * flapStrength;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Letter"))
        {
            Text letterText = collision.gameObject.GetComponentInChildren<Text>();
            if (letterText != null)
            {
                string letter = letterText.text;
                string currentCountry = countryManager.GetCurrentCountry();

                if (currentCountry.Contains(letter)) // Correct letter
                {
                    gameLogic.AddScore(5);
                    Destroy(collision.gameObject);
                    CheckIfCountryCompleted(currentCountry);
                }
                else // Incorrect letter
                {
                    gameLogic.GameOver();
                    birdIsAlive = false;
                    Destroy(collision.gameObject);
                }
            }
        }
    }

    private void CheckIfCountryCompleted(string currentCountry)
    {
        bool allLettersFound = !countryManager.countryText.text.Contains("_");
        if (allLettersFound)
        {
            countryManager.GenerateNewCountry(); // Generate a new country name
        }
    }
}
