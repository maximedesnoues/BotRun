using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Defeat : MonoBehaviour
{
    [SerializeField] private Text totalScoreText;
    [SerializeField] private Text elapsedTimeText;
    [SerializeField] private Text tricksText;
    [SerializeField] private Text objectsCollectedText;
    [SerializeField] private GameObject defeat;

    private SceneLoader sceneLoader;
    private ScoreManager scoreManager;

    private void Start()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    public void ShowDefeatScreen(int totalScore, float elapsedTime, int tricks, string objectsCollected)
    {
        totalScoreText.text = "Total Score: " + totalScore;
        elapsedTimeText.text = "Elapsed Time: " + FormatTime(elapsedTime);
        tricksText.text = "Tricks Performed: " + tricks;
        objectsCollectedText.text = "Objects Collected: " + objectsCollected;
        defeat.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ReplayButton()
    {
        Time.timeScale = 1f;
        sceneLoader.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1f;
        sceneLoader.LoadSceneAsync("MainMenuScene");
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }
}