using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterSpawner : MonoBehaviour
{
    public GameObject letterPrefab;
    public float spawnRate = 1f;
    public float letterSpeed = 5f;
    private string currentCountry = "CANADA";

    private float minY = -4f;
    private float maxY = 4f;
    private float letterSpawnX = 10f;
    private HashSet<float> usedPositions = new HashSet<float>();

    void Start()
    {
        StartCoroutine(SpawnLetters());
    }

    IEnumerator SpawnLetters()
    {
        while (true)
        {
            char randomLetter = currentCountry[Random.Range(0, currentCountry.Length)];
            GameObject letter = Instantiate(letterPrefab);

            // Ensure no overlapping Y positions
            float randomYPosition = GetUniqueYPosition();
            letter.transform.position = new Vector3(letterSpawnX, randomYPosition, 0f);

            Letter letterScript = letter.GetComponent<Letter>();
            if (letterScript != null)
            {
                letterScript.InitializeLetter(randomLetter);
            }
            else
            {
                Debug.LogError("Letter script missing on prefab!");
            }

            yield return new WaitForSeconds(spawnRate);
        }
    }

    private float GetUniqueYPosition()
    {
        float yPos;
        int maxAttempts = 10;
        int attempt = 0;

        do
        {
            yPos = Random.Range(minY, maxY);
            attempt++;
        } while (usedPositions.Contains(yPos) && attempt < maxAttempts);

        usedPositions.Add(yPos);
        Invoke(nameof(ClearUsedPositions), 1.5f);

        return yPos;
    }

    private void ClearUsedPositions()
    {
        usedPositions.Clear();
    }
}
