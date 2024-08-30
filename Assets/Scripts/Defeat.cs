using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Defeat : MonoBehaviour
{
    // UI elements for displaying defeat screen information
    [SerializeField] private Text totalScoreText;
    [SerializeField] private Text elapsedTimeText;
    [SerializeField] private Text tricksText;
    [SerializeField] private Text objectsCollectedText;
    [SerializeField] private GameObject defeat;

    // References to other components in the scene
    private SceneLoader sceneLoader;
    private ScoreManager scoreManager;

    private void Start()
    {
        // Find the SceneLoader and ScoreManager components in the scene
        sceneLoader = FindObjectOfType<SceneLoader>();
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    // Shows the defeat screen with relevant statistics
    public void ShowDefeatScreen(int totalScore, float elapsedTime, int tricks, string objectsCollected)
    {
        totalScoreText.text = "Total Score: " + totalScore;
        elapsedTimeText.text = "Elapsed Time: " + FormatTime(elapsedTime);
        tricksText.text = "Tricks Performed: " + tricks;
        objectsCollectedText.text = "Objects Collected: " + objectsCollected;
        defeat.SetActive(true);

        Time.timeScale = 0f; // Pause the game
    }

    // Reloads the current level when the Replay button is pressed
    public void ReplayButton()
    {
        Time.timeScale = 1f;
        sceneLoader.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    // Returns to the main menu when the Main Menu button is pressed
    public void MainMenuButton()
    {
        Time.timeScale = 1f;
        sceneLoader.LoadSceneAsync("MainMenuScene");
    }

    // Formats the elapsed time into a string (mm:ss)
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }
}