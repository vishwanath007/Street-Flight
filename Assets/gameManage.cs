using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel; // Assign your Game Over Panel in the inspector
    public Text scoreText; // Assign your score Text component in the inspector
    private bool isGameOver = false;
    private int score = 0; // This would be updated throughout the game
    internal bool isGameStarted = false;

    private Transform playerTransform;

    void Start()
    {
        // Ensure the start menu is visible initially
        gameOverPanel.SetActive(true);
    }

    public void StartGame()
    {
        // Hide the start menu
        gameOverPanel.SetActive(false);
        isGameStarted = true;
        playerTransform = GameObject.FindWithTag("Player").transform;
        // Add any additional logic to start the game, like enabling gameplay objects or starting timers
    }

    private void Update()
    {
        if (!isGameOver)
        {
            // Assuming you have a method to check if the spaceship is off the ground
            if (IsSpaceshipOffTheGround())
            {
                //GameOver();
            }
        }
    }

    private bool IsSpaceshipOffTheGround()
    {
        if (playerTransform.position.y < -2) // Example threshold
        {
            GameOver();
        }
        return false;
    }

    private void GameOver()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true); // Show the game over panel
        scoreText.text = "Score: " + score.ToString(); // Update the score text
        RestartGame();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }

    public void ExitGame()
    {
        // If running in the Unity editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // Quit the game
#endif
    }
}
