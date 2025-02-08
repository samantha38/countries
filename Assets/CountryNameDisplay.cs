using UnityEngine;
using UnityEngine.UI;

public class CountryNameDisplay : MonoBehaviour
{
    public Text countryText; // Reference to the UI Text component
    private string currentCountry;
    private string hiddenCountry;

    private string[] countries = { "BRAZIL", "CANADA", "FRANCE", "INDIA", "JAPAN", "MEXICO", "NORWAY", "SPAIN", "SWEDEN", "TURKEY" };

    void Start()
    {
        GenerateNewCountry();
    }

    public void GenerateNewCountry()
    {
        currentCountry = countries[Random.Range(0, countries.Length)];
        hiddenCountry = HideRandomLetters(currentCountry);
        countryText.text = hiddenCountry;

        // Print the actual country name to the Console for reference
        Debug.Log(currentCountry);
    }

    private string HideRandomLetters(string word)
    {
        char[] hiddenArray = word.ToCharArray();
        for (int i = 0; i < hiddenArray.Length; i++)
        {
            if (Random.value > 0.5f) // 50% chance to hide a letter
            {
                hiddenArray[i] = '_';
            }
        }
        return new string(hiddenArray);
    }

    public string GetCurrentCountry()
    {
        return currentCountry;
    }
}
