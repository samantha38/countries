using UnityEngine;

public class LetterBehavior : MonoBehaviour
{
    public char Letter { get; private set; }
    private TextMesh textMesh;
    private MeshRenderer meshRenderer;

    void Awake()
    {
        textMesh = GetComponentInChildren<TextMesh>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        if (textMesh == null)
            Debug.LogError("TextMesh not found!");
    }

    public void Initialize(char letter)
    {
        Letter = letter;
        if (textMesh != null)
        {
            textMesh.text = letter.ToString();
            // ✅ Force visible immediately
            if (meshRenderer != null)
                meshRenderer.enabled = true;
        }
    }

    void Update()
    {
        if (transform.position.x < -10f)
            Destroy(gameObject);
    }
}