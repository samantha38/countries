using UnityEngine;

public class LetterBehavior : MonoBehaviour
{
    public char Letter { get; private set; }
    private TextMesh textMesh;

    void Awake()
    {
        textMesh = GetComponentInChildren<TextMesh>();
        if (textMesh == null)
        {
            Debug.LogError("TextMesh component not found!");
        }
    }

    public void Initialize(char letter)
    {
        Letter = letter;
        if (textMesh != null)
        {
            textMesh.text = letter.ToString();
            Debug.Log($"Setting letter to: {letter}");
        }
        else
        {
            Debug.LogError("TextMesh is null in Initialize!");
        }
    }
}