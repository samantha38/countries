using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private float startPos;
    public GameObject cam;
    public float parallaxEffect; // Speed at which the background moves relative to the camera

    void Start()
    {
        startPos = transform.position.x;
    }
    
    void Update()
    {
        // Calculate distance background moves based on cam movement
        float distance = cam.transform.position.x * parallaxEffect; // 0 = move with cam, 1 = won't move, 0.5 = half 
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
    }
}
