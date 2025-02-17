using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject mainMenuPanel;

    [SerializeField] private GameObject letterPrefab;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text collectedLettersText;
    [SerializeField] private Text livesText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-8f, -4f);
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(8f, 4f);
    [SerializeField] private GameObject bird;

    private List<string> countries = new List<string> 
    {
        "FRANCE", "SPAIN", "ITALY", "GERMANY", "BRAZIL", "CANADA", "INDIA", "JAPAN", "CHINA", "RUSSIA", 
        "MEXICO", "ARGENTINA", "EGYPT", "TURKEY", "AUSTRALIA", "UNITEDSTATES", "UNITEDKINGDOM", "NETHERLANDS", "SWEDEN", "NORWAY", 
        "FINLAND", "DENMARK", "BELGIUM", "SWITZERLAND", "AUSTRIA", "POLAND", "CZECHIA", "PORTUGAL", "GREECE", "HUNGARY", 
        "UKRAINE", "ROMANIA", "SOUTHAFRICA", "NIGERIA", "KENYA", "MOROCCO", "NEWZEALAND", "SOUTHKOREA", "VIETNAM", "THAILAND", 
        "PHILIPPINES", "INDONESIA", "MALAYSIA", "SINGAPORE", "PAKISTAN", "BANGLADESH", "SAUDIARABIA", "UNITEDARABEMIRATES", "IRAN", "IRAQ"
    };

    private string currentCountry;
    private HashSet<char> collectedLetters = new HashSet<char>();
    private int score = 0;
    private float spawnTimer;
    private int lives = 3;
    private float correctLetterProbability = 0.5f;
    private bool isGameOver = false;

    public void PlayGame()
    {
        Debug.Log("Play button clicked! Starting the game...");

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false); // Hide the main menu panel
        }

        Time.timeScale = 1; // Unpause the game

        // Start the game and hide the menu
        StartNewRound();
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 0;  
        isGameOver = false;  
        gameOverPanel.SetActive(false); // Hide Game Over screen at the start
    }

    private void UpdateUI()
    {
        if (scoreText == null || collectedLettersText == null || livesText == null)
        {
            Debug.LogError("UI Text references are missing in the Inspector!");
            return;
        }

        // Ensure Text is enabled
        scoreText.enabled = true;
        livesText.enabled = true;
        collectedLettersText.enabled = true;

        // Update UI text
        scoreText.text = $"Score: {score}";
        collectedLettersText.text = $"Target: {GetCountryWithBlanks()}";
        livesText.text = $"Lives: {lives}";

        Debug.Log($"Updated UI -> Score: {score}, Lives: {lives}, Target: {currentCountry}");

        // Force UI Refresh
        scoreText.enabled = false;
        scoreText.enabled = true;
    }

    void Update()
    {
        if (isGameOver) return; // Stop execution if game over

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            SpawnLetter();
            spawnTimer = spawnInterval;
        }

        float lowerBoundary = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y - 1.5f;
        if (bird.transform.position.y < lowerBoundary)  
        {
            if (!isGameOver) // Ensure GameOver is only called once
            {
                GameOver();
            }
        }

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
        Debug.Log("Starting New Round...");
        currentCountry = countries[Random.Range(0, countries.Count)];
        collectedLetters.Clear();
        UpdateUI();
    }

    private void SpawnLetter()
    {
        Debug.Log("SpawnLetter called!");
        float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        float x = Camera.main.ViewportToWorldPoint(new Vector3(1.2f, 0, 0)).x;
        Vector3 spawnPos = new Vector3(x, y, 0);

        GameObject letterObj = Instantiate(letterPrefab, spawnPos, Quaternion.identity);

        char letterToSpawn;
        if (Random.value < correctLetterProbability)
            letterToSpawn = currentCountry[Random.Range(0, currentCountry.Length)];
        else
            letterToSpawn = (char)Random.Range('A', 'Z' + 1);

        var letterBehavior = letterObj.GetComponent<LetterBehavior>();
        if (letterBehavior != null)
            letterBehavior.Initialize(letterToSpawn);

        Rigidbody2D rb = letterObj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(-2f, 0);
    }

    public void GameOver()
    {
        if (isGameOver) return; // Prevents multiple calls

        isGameOver = true; // Set game over state

        Debug.Log("Game Over Triggered!"); // Use Debug.LogWarning to highlight it
        Time.timeScale = 0; // Stop game physics
        gameOverPanel.SetActive(true); // Show Game Over UI
    }

     public void PlayAgain()
{
    Time.timeScale = 1;  // Resume game time
    isGameOver = false;  // Reset game over state
    SceneManager.LoadScene("Two"); // Reload scene
}


    public void CollectLetter(char letter)
    {
        if (currentCountry.Contains(letter))
        {
            collectedLetters.Add(letter);
            Debug.Log($"Collected: {new string(collectedLetters.OrderBy(c => c).ToArray())}");
            CheckWord();
        }
        else
        {
            lives--;
            if (lives <= 0)
            {
                GameOver();
                return;
            }
        }
        UpdateUI();
    }

    private void CheckWord()
    {
        bool allLettersCollected = currentCountry.All(c => collectedLetters.Contains(c));

        if (allLettersCollected)
        {
            Debug.Log("Correct word collected! Score awarded.");
            score += 100;
            if (lives < 3)
                lives++;

            correctLetterProbability = Mathf.Max(correctLetterProbability - 0.03f, 0.2f);

            UpdateUI();
            Invoke(nameof(StartNewRound), 1f);
        }
    }

    private string GetCountryWithBlanks()
    {
        string modifiedCountry = "";
        
        // A list to store letters that are revealed (including collected ones)
        List<char> revealedLetters = new List<char>();

        // Add collected letters to revealed letters
        revealedLetters.AddRange(collectedLetters);

        // Ensure that the first letter is always revealed
        revealedLetters.Add(currentCountry[0]);

        // Add one more random letter if no letters have been collected yet
        if (collectedLetters.Count == 0)
        {
            char extraLetter;
            do
            {
                extraLetter = currentCountry[Random.Range(1, currentCountry.Length)];
            } while (revealedLetters.Contains(extraLetter)); // Avoid revealing the same letter twice
            revealedLetters.Add(extraLetter);
        }

        // Build the display string for the country name
        for (int i = 0; i < currentCountry.Length; i++)
        {
            if (revealedLetters.Contains(currentCountry[i]))
                modifiedCountry += currentCountry[i] + " "; // Show the letter if revealed
            else
                modifiedCountry += "_ "; // Show blank if not revealed
        }

        return modifiedCountry.Trim();
    }
}