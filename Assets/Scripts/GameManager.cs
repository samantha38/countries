using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
<<<<<<< Updated upstream
=======
using UnityEngine.SceneManagement;

>>>>>>> Stashed changes

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject letterPrefab;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text collectedLettersText;
    [SerializeField] private Text livesText;
<<<<<<< Updated upstream
=======
    [SerializeField] private GameObject gameOverPanel;
>>>>>>> Stashed changes
    
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-8f, -4f);
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(8f, 4f);
<<<<<<< Updated upstream

    private List<string> countries = new List<string> { "FRANCE", "SPAIN", "ITALY", "GERMANY", "BRAZIL", "CANADA", "INDIA", "JAPAN", "CHINA", "RUSSIA", "MEXICO", "ARGENTINA", "EGYPT", "TURKEY", "AUSTRALIA" };
=======
    [SerializeField] private GameObject bird;


    private List<string> countries = new List<string> 
{
    "FRANCE", "SPAIN", "ITALY", "GERMANY", "BRAZIL", "CANADA", "INDIA", "JAPAN", "CHINA", "RUSSIA", 
    "MEXICO", "ARGENTINA", "EGYPT", "TURKEY", "AUSTRALIA", "UNITEDSTATES", "UNITEDKINGDOM", "NETHERLANDS", "SWEDEN", "NORWAY", 
    "FINLAND", "DENMARK", "BELGIUM", "SWITZERLAND", "AUSTRIA", "POLAND", "CZECHIA", "PORTUGAL", "GREECE", "HUNGARY", 
    "UKRAINE", "ROMANIA", "SOUTHAFRICA", "NIGERIA", "KENYA", "MOROCCO", "NEWZEALAND", "SOUTHKOREA", "VIETNAM", "THAILAND", 
    "PHILIPPINES", "INDONESIA", "MALAYSIA", "SINGAPORE", "PAKISTAN", "BANGLADESH", "SAUDIARABIA", "UNITEDARABEMIRATES", "IRAN", "IRAQ"
};

>>>>>>> Stashed changes
    private string currentCountry;
    private HashSet<char> collectedLetters = new HashSet<char>();
    private int score = 0;
    private float spawnTimer;
    private int lives = 3;
    private float correctLetterProbability = 0.5f;
<<<<<<< Updated upstream
=======
    private bool isGameOver = false;

>>>>>>> Stashed changes

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
<<<<<<< Updated upstream
    {
        StartNewRound();
    }
=======
{
    Time.timeScale = 1;  
    isGameOver = false;  
    gameOverPanel.SetActive(false); // Hide Game Over screen at the start
    StartNewRound();
}
>>>>>>> Stashed changes

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
<<<<<<< Updated upstream
=======
        
>>>>>>> Stashed changes
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            SpawnLetter();
            spawnTimer = spawnInterval;
        }

        foreach (GameObject letter in GameObject.FindGameObjectsWithTag("Letter"))
        {
<<<<<<< Updated upstream
            if (letter.transform.position.x < Camera.main.ViewportToWorldPoint(new Vector3(-0.2f, 0, 0)).x)
=======
            
>>>>>>> Stashed changes
            {
                Destroy(letter);
            }
        }
<<<<<<< Updated upstream
=======
         float lowerBoundary = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y - 1.3f; // Extend below the screen
        if (bird.transform.position.y < lowerBoundary) // Adjust -5f based on your game’s ground level
    {
        GameOver();
    }
>>>>>>> Stashed changes
    }

    private void StartNewRound()
    {
        Debug.Log("Starting New Round...");
        currentCountry = countries[Random.Range(0, countries.Count)];
        collectedLetters.Clear();
        UpdateUI();

        foreach (GameObject letter in GameObject.FindGameObjectsWithTag("Letter"))
        {
            Destroy(letter);
        }
    }

    private void SpawnLetter()
    {
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

<<<<<<< Updated upstream
    public void GameOver()
    {
        Debug.Log("Game Over!");
        Time.timeScale = 0;
    }
=======
    private void GameOver()
{
    if (isGameOver) return; // Prevents repeated calls

    isGameOver = true; // Set game over state
    Debug.Log("Game Over!");
    Time.timeScale = 0; // Pause game
    gameOverPanel.SetActive(true); // Show Game Over UI
}


    public void PlayAgain()
{
    Time.timeScale = 1;  // Resume game time
    isGameOver = false;  // Reset game over state
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload scene
}


>>>>>>> Stashed changes

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
        List<char> revealedLetters = new List<char> { currentCountry[0] };

        if (collectedLetters.Count == 0)
        {
            char extraLetter;
            do
            {
                extraLetter = currentCountry[Random.Range(1, currentCountry.Length)];
            } while (revealedLetters.Contains(extraLetter));
            revealedLetters.Add(extraLetter);
        }

        for (int i = 0; i < currentCountry.Length; i++)
        {
            if (revealedLetters.Contains(currentCountry[i]) || collectedLetters.Contains(currentCountry[i]))
                modifiedCountry += currentCountry[i] + " ";
            else
                modifiedCountry += "_ ";
        }

        return modifiedCountry.Trim();
    }
}
