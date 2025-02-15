using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject letterPrefab;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text collectedLettersText;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-8f, -4f);
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(8f, 4f);

    private List<string> countries = new List<string> { "FRANCE", "SPAIN", "ITALY", "GERMANY", "BRAZIL" };
    private string currentCountry;
    private List<char> collectedLetters = new List<char>();
    private int score = 0;
    private float spawnTimer;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartNewRound();
    }

    private void UpdateUI()
{
    scoreText.text = $"Score: {score}";
    collectedLettersText.text = $"Letters: {string.Join("", collectedLetters)}\nTarget: {currentCountry}";
}

    void Update()
{
    spawnTimer -= Time.deltaTime;
    if (spawnTimer <= 0)
    {
        SpawnLetter();
        spawnTimer = spawnInterval;
    }

    // Destroy letters that go off-screen
    foreach (GameObject letter in GameObject.FindGameObjectsWithTag("Letter"))
    {
        if (letter.transform.position.x < Camera.main.ViewportToWorldPoint(new Vector3(-0.2f, 0, 0)).x)
        {
            Destroy(letter);
        }
    }
}

    private void StartNewRound()
    {
        currentCountry = countries[Random.Range(0, countries.Count)];
        collectedLetters.Clear();
        UpdateUI();
        
        // Clear existing letters
        foreach (GameObject letter in GameObject.FindGameObjectsWithTag("Letter"))
        {
            Destroy(letter);
        }
    }

    private void SpawnLetter()
{
    float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y); // Random vertical position
    float x = Camera.main.ViewportToWorldPoint(new Vector3(1.2f, 0, 0)).x; // Off-screen right

    Vector3 spawnPos = new Vector3(x, y, 0);
    
    GameObject letterObj = Instantiate(letterPrefab, spawnPos, Quaternion.identity);

    char letterToSpawn;
    if (Random.value < 0.7f)
    {
        letterToSpawn = currentCountry[Random.Range(0, currentCountry.Length)];
    }
    else
    {
        letterToSpawn = (char)Random.Range('A', 'Z' + 1);
    }

    var letterBehavior = letterObj.GetComponent<LetterBehavior>();
    if (letterBehavior != null)
    {
        letterBehavior.Initialize(letterToSpawn);
    }

    // Move letter left
    Rigidbody2D rb = letterObj.AddComponent<Rigidbody2D>(); // Add Rigidbody2D if not already present
    rb.gravityScale = 0; // Prevent falling
    rb.linearVelocity = new Vector2(-2f, 0); // Move left at a speed of 2 units per second
}


    public void CollectLetter(char letter)
    {
        Debug.Log($"GameManager collecting letter: {letter}");
        collectedLetters.Add(letter);
        UpdateUI();
        CheckWord();
    }

    private IEnumerable<string> GetPermutations(string source, int length)
{
    if (length == 1) return source.Select(x => x.ToString());

    return GetPermutations(source, length - 1)
        .SelectMany(x => source.Where(y => !x.Contains(y)),
            (x, y) => x + y);
}


    private void CheckWord()
    {
        string collected = new string(collectedLetters.ToArray());
        
        // Check if we have enough letters to potentially form the country name
        if (collectedLetters.Count >= currentCountry.Length)
        {
            // Check all possible combinations of collected letters
            var possibleWords = GetPermutations(collected, currentCountry.Length);
            foreach (string word in possibleWords)
            {
                if (word == currentCountry)
                {
                    score += 100;
                    UpdateUI();
                    StartNewRound();
                    return;
                }
            }
            
            // If we have too many letters and still haven't found a match, clear and start over
            if (collectedLetters.Count > currentCountry.Length + 2)
            {
                collectedLetters.Clear();
                UpdateUI();
            }
        }
    }

    

}