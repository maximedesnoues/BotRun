using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{
    [SerializeField] private Text totalScoreText;
    [SerializeField] private Text elapsedTimeText;
    [SerializeField] private Text tricksText;
    [SerializeField] private Text objectsCollectedText;
    [SerializeField] private GameObject levelComplete;

    private SceneLoader sceneLoader;
    private LevelManager levelManager;
    private ScoreManager scoreManager;
    
    private int currentLevel;
    private int totalScore;
    private float elapsedTime;

    private void Start()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();
        levelManager = FindObjectOfType<LevelManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        currentLevel = GetCurrentLevelIndex();
    }

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

    public void ShowLevelComplete(int totalScore, float elapsedTime, int tricks, string objectsCollected)
    {
        this.totalScore = totalScore;
        this.elapsedTime = elapsedTime;
        
        totalScoreText.text = "Total Score: " + totalScore;
        elapsedTimeText.text = "Elapsed Time: " + FormatTime(elapsedTime);
        tricksText.text = "Tricks Performed: " + tricks;
        objectsCollectedText.text = "Objects Collected: " + objectsCollected;
        levelComplete.SetActive(true);
        
        Time.timeScale = 0f;
    }

    public void ReplayButton()
    {
        Time.timeScale = 1f;
        sceneLoader.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

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

    public void MainMenuButton()
    {
        Time.timeScale = 1f;
        levelManager.UnlockNextLevel(currentLevel, scoreManager.GetTotalScore(), elapsedTime);
        sceneLoader.LoadSceneAsync("MainMenuScene");
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }
}