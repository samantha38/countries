using UnityEngine;
using UnityEngine.InputSystem;

public class BirdController : MonoBehaviour
{
    public Rigidbody2D myRigidbody;
    public float flapStrength = 6f;
    public bool birdIsAlive = true;
    public PlayerInput playerInput;
    public InputAction touchPressAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        touchPressAction = playerInput.actions["TouchPress"];
    }

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myRigidbody.gravityScale = 1;
    }

    private void OnEnable()
    {
        touchPressAction.Enable();
    }

    private void OnDisable()
    {
        touchPressAction.Disable();
    }

    private void Update()
    {
        if (touchPressAction.WasPerformedThisFrame() && birdIsAlive)
            myRigidbody.linearVelocity = Vector2.up * flapStrength;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Letter"))
        {
            LetterBehavior lb = other.gameObject.GetComponent<LetterBehavior>();
            if (lb != null && GameManager.Instance != null)
                GameManager.Instance.CollectLetter(lb.Letter);
            Destroy(other.gameObject);
        }
    }
}