using UnityEngine;
using UnityEngine.UI;

public class Letter : MonoBehaviour
{
    public Text letterText;
    private char letterValue;
    private float speed = 5f;

    public void InitializeLetter(char letter)
    {
        letterValue = letter;
        letterText.text = letter.ToString();
    }

    void Update()
    {
        // Move letter from right to left
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // Destroy if it moves off-screen
        if (transform.position.x < -10f)
        {
            Destroy(gameObject);
        }
    }

    public char GetLetterValue()
    {
        return letterValue;
    }
}
