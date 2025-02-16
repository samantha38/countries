using UnityEngine;

public class ScreenSizeChecker : MonoBehaviour
{
    void Start()
    {
        float screenHeight = 2f * Camera.main.orthographicSize; // Full world height
        float screenWidth = screenHeight * Camera.main.aspect;  // Full world width

        Debug.Log("World Width: " + screenWidth + ", World Height: " + screenHeight);
    }
}
