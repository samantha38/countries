using UnityEngine; // Add this line at the top!

public class BirdController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        targetPosition = transform.position;
        Debug.Log("Bird controller started");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition = new Vector3(mousePos.x, mousePos.y, 0);
            isMoving = true;
            Debug.Log($"Moving to position: {targetPosition}");
        }

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
            }
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