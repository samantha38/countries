using UnityEngine;
using UnityEngine.UI;


public class CountryManager : MonoBehaviour
{
    // Step 1: Store a list of countries
    public string[] countries = { "INDIA", "JAPAN", "BRAZIL", "EGYPT", "CANADA" };

    public GameObject correctLetterPrefab;  
    // Drag the CorrectLetterPrefab here in the Inspector
    public GameObject incorrectLetterPrefab; 
     // Drag the IncorrectLetterPrefab here in the Inspector

    private string currentCountry; // This will hold the selected country
    private string maskedCountry;  // This will hold the country with missing letters

    void Start()
    {
        SelectRandomCountry();  // Pick a random country when the game starts
    }

    void SelectRandomCountry()
    {
        currentCountry = countries[Random.Range(0, countries.Length)]; // Pick a random country
        Debug.Log("Selected Country: " + currentCountry);

        GenerateMaskedWord();  // Create the missing letters version
    }

    void GenerateMaskedWord()
    {
        maskedCountry = "";
        
        // Step 2: Spawn correct and incorrect letters
        for (int i = 0; i < currentCountry.Length; i++)
        {
            if (Random.value > 0.5f)  // 50% chance to hide a letter
            {
                maskedCountry += "_";
                SpawnIncorrectLetter(i);  // Spawn incorrect letters
            }
            else
            {
                maskedCountry += currentCountry[i];
                SpawnCorrectLetter(i);  // Spawn correct letters
            }
        }

        Debug.Log("Guess the country: " + maskedCountry);
    }

    // Spawn Correct Letters
    void SpawnCorrectLetter(int index)
    {
        // Instantiate the correct letter prefab at a random position above the screen
        GameObject correctLetter = Instantiate(correctLetterPrefab);
        correctLetter.transform.position = new Vector3(index * 2, 5, 0); // Adjust position as needed
        correctLetter.GetComponent<Text>().text = currentCountry[index].ToString();
    }

    // Spawn Incorrect Letters (Obstacles)
    void SpawnIncorrectLetter(int index)
    {
        // Instantiate the incorrect letter prefab at a random position above the screen
        GameObject incorrectLetter = Instantiate(incorrectLetterPrefab);
        incorrectLetter.transform.position = new Vector3(Random.Range(-5, 5), 5, 0);  // Random position for obstacles
        incorrectLetter.GetComponent<Text>().text = ((char)Random.Range(65, 91)).ToString();
    }
}
