using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogicScript : MonoBehaviour
{

    public int playerScore = 0;  // Player's current score
    public Text scoreText;  // Text UI element to display the score
    

    public void AddScore(int scoreToAdd)
    {
        playerScore += scoreToAdd;
        scoreText.text = "Score: " + playerScore.ToString();
    }

    
    public void RestartGame()
    {
        // Reset the game (e.g., reset score, bird position, etc.)
        Time.timeScale = 1f;  // Resume the game
        gameOverPanel.SetActive(false);  // Hide the Game Over Panel
        restartButton.gameObject.SetActive(false);  // Hide the Restart Button
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);  // Restart the game scene

        // Reset other game elements as needed (bird position, score, etc.)
        // For example, resetting bird position and score could be added here
    }
    public GameObject gameOverPanel;  // Reference to your Game Over Panel
    public Button restartButton;      // Reference to the Restart Button

    public void GameOver()
    {
        // Make the Game Over Panel visible
        gameOverPanel.SetActive(true);

        // Make the Restart Button visible
        restartButton.gameObject.SetActive(true);

        // Optionally, set the text message in the Game Over Panel
        Text gameOverText = gameOverPanel.GetComponentInChildren<Text>();
        gameOverText.text = "Game Over"; // Or any custom message

        // Stop the game or disable movement, etc.
        Time.timeScale = 0f;  // Freeze the game
    }

}


