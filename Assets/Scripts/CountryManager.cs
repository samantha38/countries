using UnityEngine;
using UnityEngine.UI;

public class CountryManager : MonoBehaviour
{
    public Text countryText; // UI Text element to display the country with blanks

    private string[] countries = { "CANADA", "BRAZIL", "FRANCE", "GERMANY", "INDIA", "JAPAN", "KENYA", "MEXICO", "NORWAY", "SPAIN" };

    private string selectedCountry;

    void Start()
{
    SelectRandomCountry();
}

void SelectRandomCountry()
{
    int index = Random.Range(0, countries.Length); // Pick a random index
    selectedCountry = countries[index]; // Store the selected country
    Debug.Log(selectedCountry); // Log it to the console

    DisplayCountryWithBlanks(); // Call the method to show blanks
}


void DisplayCountryWithBlanks()
{
    string modifiedCountry = "";
    
    for (int i = 0; i < selectedCountry.Length; i++)
    {
        if (i == 0 || Random.value > 0.5f) // Keep the first letter and randomly reveal others
            modifiedCountry += selectedCountry[i] + " ";
        else
            modifiedCountry += "_ ";
    }

    Debug.Log("Country with Blanks: " + modifiedCountry); // Log it for debugging
    countryText.text = modifiedCountry; // Update UI
}



}
