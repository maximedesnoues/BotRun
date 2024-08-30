using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{
    // UI elements for displaying level completion information
    [SerializeField] private Text totalScoreText;
    [SerializeField] private Text elapsedTimeText;
    [SerializeField] private Text tricksText;
    [SerializeField] private Text objectsCollectedText;
    [SerializeField] private GameObject levelComplete;

    // References to other components needed
    private SceneLoader sceneLoader;
    private LevelManager levelManager;
    private ScoreManager scoreManager;

    // Variables to track the current level, score, and time
    private int currentLevel;
    private int totalScore;
    private float elapsedTime;

    private void Start()
    {
        // Find the necessary components in the scene
        sceneLoader = FindObjectOfType<SceneLoader>();
        levelManager = FindObjectOfType<LevelManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        currentLevel = GetCurrentLevelIndex();
    }

    // Get the index of the current level from the level manager
    private int GetCurrentLevelIndex()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        for (int i = 0; i < levelManager.levelsButton.Count; i++)
        {
            if (levelManager.levelsButton[i].sceneName == sceneName)
            {
                return i;
            }
        }

        return -1;
    }

    // Show the level complete screen with relevant statistics
    public void ShowLevelComplete(int totalScore, float elapsedTime, int tricks, string objectsCollected)
    {
        this.totalScore = totalScore;
        this.elapsedTime = elapsedTime;

        totalScoreText.text = "Total Score: " + totalScore;
        elapsedTimeText.text = "Elapsed Time: " + FormatTime(elapsedTime);
        tricksText.text = "Tricks Performed: " + tricks;
        objectsCollectedText.text = "Objects Collected: " + objectsCollected;
        levelComplete.SetActive(true);

        Time.timeScale = 0f; // Pause the game
    }

    // Reload the current level when the Replay button is pressed
    public void ReplayButton()
    {
        Time.timeScale = 1f;
        sceneLoader.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    // Load the next level when the Next Level button is pressed
    public void NextLevelButton()
    {
        Time.timeScale = 1f;
        levelManager.UnlockNextLevel(currentLevel, scoreManager.GetTotalScore(), elapsedTime);

        if (currentLevel + 1 < levelManager.levelsButton.Count)
        {
            sceneLoader.LoadSceneAsync(levelManager.levelsButton[currentLevel + 1].sceneName);
        }
        else
        {
            sceneLoader.LoadSceneAsync("MainMenuScene");
        }
    }

    // Return to the main menu when the Main Menu button is pressed
    public void MainMenuButton()
    {
        Time.timeScale = 1f;
        levelManager.UnlockNextLevel(currentLevel, scoreManager.GetTotalScore(), elapsedTime);
        sceneLoader.LoadSceneAsync("MainMenuScene");
    }

    // Format the elapsed time into a string (mm:ss)
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }
}