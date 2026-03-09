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
    [SerializeField] private GameObject bird;

    private float spawnInterval = 1.5f;
    private Vector2 spawnAreaMin = new Vector2(-4f, -4f);
    private Vector2 spawnAreaMax = new Vector2(4f, 4f);
    private float lastSpawnY = float.NaN;
    private float[] spawnIntervalCycle = { 1.5f, 1.5f, 1.4f, 1.3f, 1.3f, 1.3f, 1.5f, 1.4f, 1.5f, 1.3f, 1.4f, 1.3f };
    private float[] probabilityCycle   = { 0.7f, 0.6f, 0.6f, 0.6f, 0.46f, 0.65f, 0.55f, 0.6f, 0.45f, 0.5f, 0.45f, 0.5f };
    private float gracePeriod = 1.5f;
    private float graceTimer = 0f;

    private List<string> countries = new List<string>
    {
        "FRANCE", "SPAIN", "ITALY", "GERMANY", "BRAZIL", "CANADA", "INDIA", "JAPAN", "CHINA", "RUSSIA",
        "MEXICO", "ARGENTINA", "EGYPT", "TURKEY", "AUSTRALIA", "UNITEDSTATES", "NETHERLANDS", "SWEDEN", "NORWAY",
        "FINLAND", "DENMARK", "BELGIUM", "SWITZERLAND", "AUSTRIA", "POLAND", "CZECHIA", "PORTUGAL", "GREECE", "HUNGARY",
        "UKRAINE", "ROMANIA", "NIGERIA", "KENYA", "MOROCCO", "NEWZEALAND", "SOUTHKOREA", "VIETNAM", "THAILAND",
        "PHILIPPINES", "INDONESIA", "MALAYSIA", "SINGAPORE", "PAKISTAN", "BANGLADESH", "IRAN", "IRAQ",
        "CHILE", "PERU", "OMAN", "MALI", "LAOS", "FIJI", "CUBA", "TOGO", "BENIN", "QATAR", "HAITI", "SYRIA",
        "ICELAND", "MONACO", "MALTA", "ANDORRA", "SANMARINO", "VATICAN",
        "BHUTAN", "NEPAL", "SRILANKA", "BRUNEI", "EASTTIMOR", "MALDIVES",
        "SEYCHELLES", "COMOROS", "DJIBOUTI", "ESWATINI", "LESOTHO", "MAURITIUS",
        "TONGA", "SAMOA", "VANUATU", "KIRIBATI", "NAURU", "TUVALU",
        "BELIZE", "SURINAME", "GUYANA", "BARBADOS", "GRENADA", "SAINTLUCIA", "SAINTKITTS", "DOMINICA"
    };

    private List<string> usedCountries = new List<string>();
    private string currentCountry;
    private HashSet<char> collectedLetters = new HashSet<char>();
    private int score = 0;
    private float spawnTimer;
    private int lives = 3;
    private float correctLetterProbability = 0.75f;
    private int probabilityIndex = 0;
    private bool isGameOver = false;
    private List<char> neededLetterQueue = new List<char>();

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
        score = 0;
        lives = 3;
        probabilityIndex = 0;
        spawnInterval = spawnIntervalCycle[0];
        spawnTimer = spawnInterval;
        lastSpawnY = float.NaN;
        graceTimer = 0f;
        usedCountries.Clear();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void PlayGame()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        Time.timeScale = 1;
        isGameOver = false;
        score = 0;
        lives = 3;
        probabilityIndex = 0;
        spawnInterval = spawnIntervalCycle[0];
        spawnTimer = spawnInterval;
        lastSpawnY = float.NaN;
        graceTimer = gracePeriod; // ✅ grace period
        usedCountries.Clear();
        StartNewRound();
    }

    void Update()
    {
        if (isGameOver) return;

        if (graceTimer > 0f)
            graceTimer -= Time.deltaTime;

        spawnTimer -= Time.deltaTime;
        while (spawnTimer <= 0f)
        {
            SpawnLetter();
            spawnTimer += spawnInterval;
        }

        if (Camera.main != null && bird != null)
        {
            float lowerBoundary = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y - 1.5f;
            float upperBoundary = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y + 1.3f;

            if (graceTimer <= 0f &&
                (bird.transform.position.y < lowerBoundary ||
                 bird.transform.position.y > upperBoundary))
                GameOver();
        }
    }

    private void UpdateUI()
    {
        if (scoreText == null || collectedLettersText == null || livesText == null)
        {
            Debug.LogError("UI Text references are missing in the Inspector!");
            return;
        }

        scoreText.text = $"Score: {score}";
        collectedLettersText.text = $"Target: {GetCountryWithBlanks()}";
        livesText.text = $"Lives: {lives}";
    }

    private void StartNewRound()
    {
        int idx = Mathf.Min(probabilityIndex, spawnIntervalCycle.Length - 1);
        spawnInterval = spawnIntervalCycle[idx];
        correctLetterProbability = probabilityCycle[idx];

        List<string> available = countries.Where(c => !usedCountries.Contains(c)).ToList();
        if (available.Count == 0)
        {
            usedCountries.Clear();
            available = new List<string>(countries);
        }

        currentCountry = available[Random.Range(0, available.Count)];
        usedCountries.Add(currentCountry);

        collectedLetters.Clear();
        collectedLetters.Add(currentCountry[0]);

        int extraHints = currentCountry.Length > 5 ? 2 : 1;
        for (int h = 0; h < extraHints; h++)
        {
            char hint = '\0';
            int safety = 50;
            do {
                hint = currentCountry[Random.Range(1, currentCountry.Length)];
                safety--;
            } while (collectedLetters.Contains(hint) && safety > 0);

            if (hint != '\0' && !collectedLetters.Contains(hint))
                collectedLetters.Add(hint);
        }

        neededLetterQueue = currentCountry.Distinct()
            .Where(c => !collectedLetters.Contains(c))
            .OrderBy(_ => Random.value).ToList();

        spawnTimer = spawnInterval;
        lastSpawnY = float.NaN;

        // ✅ Spawn first letter immediately
        SpawnLetter();

        UpdateUI();
    }

    private void SpawnLetter()
    {
        if (letterPrefab == null || Camera.main == null) return;

        if (string.IsNullOrEmpty(currentCountry))
        {
            StartNewRound();
            if (string.IsNullOrEmpty(currentCountry)) return;
        }

        float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        if (!float.IsNaN(lastSpawnY))
        {
            int tries = 0;
            while (Mathf.Abs(y - lastSpawnY) < 1.5f && tries < 15)
            {
                y = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
                tries++;
            }
        }
        lastSpawnY = y;

        float x = Camera.main.ViewportToWorldPoint(new Vector3(1.2f, 0, 0)).x;
        Vector3 spawnPos = new Vector3(x, y, 0);

        GameObject letterObj = Instantiate(letterPrefab, spawnPos, Quaternion.identity);

        char letterToSpawn;
        if (Random.value < correctLetterProbability)
        {
            if (neededLetterQueue.Count == 0)
                neededLetterQueue = currentCountry.Distinct()
                    .Where(c => !collectedLetters.Contains(c))
                    .OrderBy(_ => Random.value).ToList();

            if (neededLetterQueue.Count > 0)
            {
                letterToSpawn = neededLetterQueue[0];
                neededLetterQueue.RemoveAt(0);
            }
            else
                letterToSpawn = currentCountry[Random.Range(0, currentCountry.Length)];
        }
        else
            letterToSpawn = (char)Random.Range('A', 'Z' + 1);

        var letterBehavior = letterObj.GetComponent<LetterBehavior>();
        if (letterBehavior != null)
            letterBehavior.Initialize(letterToSpawn);

        // ✅ GetComponent NOT AddComponent
        Rigidbody2D rb = letterObj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.linearVelocity = new Vector2(-4f, 0);
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void PlayAgain()
    {
        Time.timeScale = 1;
        isGameOver = false;
        SceneManager.LoadScene("Two");
    }

    public void CollectLetter(char letter)
    {
        if (!string.IsNullOrEmpty(currentCountry) && currentCountry.Contains(letter))
        {
            collectedLetters.Add(letter);
            neededLetterQueue.Remove(letter);
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
            UpdateUI();
        }
    }

    private void CheckWord()
    {
        if (string.IsNullOrEmpty(currentCountry)) return;

        bool allLettersCollected = currentCountry.All(c => collectedLetters.Contains(c));

        if (allLettersCollected)
        {
            score += 100;
            if (lives < 3)
                lives++;
            probabilityIndex++;
            UpdateUI();
            Invoke(nameof(StartNewRound), 1f);
        }
        else
        {
            UpdateUI();
        }
    }

    private string GetCountryWithBlanks()
    {
        string modifiedCountry = "";
        for (int i = 0; i < currentCountry.Length; i++)
        {
            if (collectedLetters.Contains(currentCountry[i]))
                modifiedCountry += currentCountry[i] + " ";
            else
                modifiedCountry += "_ ";
        }
        return modifiedCountry.Trim();
    }
}