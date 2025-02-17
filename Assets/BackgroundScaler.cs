using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
    private void Start()
    {
        ScaleBackground();
    }

    private void ScaleBackground()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr == null)
        {
            Debug.LogError("No SpriteRenderer found on the GameObject!");
            return;
        }

        // Get background sprite size
        float spriteWidth = sr.sprite.bounds.size.x;
        float spriteHeight = sr.sprite.bounds.size.y;

        // Get screen dimensions in world units
        float worldScreenHeight = Camera.main.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight * Screen.width / Screen.height;

        // Calculate scale factors to fit screen
        Vector3 newScale = transform.localScale;
        newScale.x = worldScreenWidth / spriteWidth;
        newScale.y = worldScreenHeight / spriteHeight;

        transform.localScale = newScale;
    }
}
