using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject letterPrefab;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text collectedLettersText;
    [SerializeField] private Text livesText;
    [SerializeField] private Text gameOverText; // Assign in Unity
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-8f, -4f);
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(8f, 4f);

    private List<string> countries = new List<string> { "FRANCE", "SPAIN", "ITALY", "GERMANY", "BRAZIL" };
    private string currentCountry;
    private List<char> collectedLetters = new List<char>();
    private int score = 0;
    private int lives = 3;
    private float spawnTimer;
    private float correctLetterProbability = 70f; // Start at 70%

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        gameOverText.gameObject.SetActive(false); // Hide "Game Over" at the start
        StartNewRound();
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
        float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        float x = Camera.main.ViewportToWorldPoint(new Vector3(1.2f, 0, 0)).x;
        Vector3 spawnPos = new Vector3(x, y, 0);

        GameObject letterObj = Instantiate(letterPrefab, spawnPos, Quaternion.identity);

        char letterToSpawn;
        if (Random.value * 100 < correctLetterProbability)
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

        Rigidbody2D rb = letterObj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.velocity = new Vector2(-2f, 0);
    }

    private void GameOver()
    {
        gameOverText.text = "GAME OVER";
        gameOverText.gameObject.SetActive(true); // Show "Game Over"
        Time.timeScale = 0; // Stop the game
    }

    public void CollectLetter(char letter)
    {
        if (currentCountry.Contains(letter))
        {
            collectedLetters.Add(letter);
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
        string collected = new string(collectedLetters.ToArray());

        if (collectedLetters.Count >= currentCountry.Length)
        {
            var possibleWords = GetPermutations(collected, currentCountry.Length);
            foreach (string word in possibleWords)
            {
                if (word == currentCountry)
                {
                    score += 10; // Update score
                    if (lives < 3)
                        lives++; // Reward 1 life if not max

                    correctLetterProbability = Mathf.Clamp(correctLetterProbability - 3, 30, 70); // Adjust probability

                    StartCoroutine(TransitionToNextCountry()); // Transition immediately
                    return;
                }
            }
        }
    }

    private IEnumerator TransitionToNextCountry()
    {
        yield return new WaitForSeconds(0.5f); // Short delay before next round
        StartNewRound();
    }

    private IEnumerable<string> GetPermutations(string source, int length)
    {
        if (length == 1) return source.Select(x => x.ToString());

        return GetPermutations(source, length - 1)
            .SelectMany(x => source.Where(y => !x.Contains(y)), (x, y) => x + y);
    }

    private string GetCountryWithBlanks()
    {
        string modifiedCountry = "";
        int revealedLetters = 0;

        for (int i = 0; i < currentCountry.Length; i++)
        {
            if (i == 0 || collectedLetters.Contains(currentCountry[i]))
            {
                modifiedCountry += currentCountry[i] + " ";
                revealedLetters++;
            }
            else if (revealedLetters < 2) // Ensure at least 2 letters are shown
            {
                modifiedCountry += currentCountry[i] + " ";
                revealedLetters++;
            }
            else
            {
                modifiedCountry += "_ ";
            }
        }

        return modifiedCountry;
    }

    private void UpdateUI()
    {
        scoreText.text = $"Score: {score}";
        collectedLettersText.text = $"Target: {GetCountryWithBlanks()}";
        livesText.text = $"Lives: {lives}";
    }
}
