using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    // List of level buttons in the UI
    public List<LevelButton> levelsButton;

    // Colors for unlocked and locked levels
    [SerializeField] private Color unlockedColor;
    [SerializeField] private Color lockedColor;

    // Path to save/load level data
    private string savePath;
    private LevelsDataList levelsDataList;
    private SceneLoader sceneLoader;

    private void Start()
    {
        // Set the save path and initialize the level data list
        savePath = Path.Combine(Application.persistentDataPath, "levelsData.json");
        levelsDataList = new LevelsDataList();
        sceneLoader = FindObjectOfType<SceneLoader>();

        // Load and update level statuses
        LoadLevelsStatus();
        UpdateLevelsButton();
    }

    // Load the status of levels from a JSON file
    private void LoadLevelsStatus()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            levelsDataList = JsonUtility.FromJson<LevelsDataList>(json);
        }
        else
        {
            // Initialize level data if no save file exists
            for (int i = 0; i < levelsButton.Count; i++)
            {
                LevelData levelData = new LevelData
                {
                    sceneName = levelsButton[i].sceneName,
                    isUnlocked = i == 0, // Unlock the first level by default
                    bestScore = 0,
                    bestTime = 0f
                };

                levelsDataList.levels.Add(levelData);
            }

            SaveLevelsStatus(); // Save the initialized data
        }

        // Update level buttons with loaded data
        for (int i = 0; i < levelsButton.Count; i++)
        {
            levelsButton[i].isUnlocked = levelsDataList.levels[i].isUnlocked;
            levelsButton[i].bestScore = levelsDataList.levels[i].bestScore;
            levelsButton[i].bestTime = levelsDataList.levels[i].bestTime;
        }
    }

    // Save the current status of levels to a JSON file
    private void SaveLevelsStatus()
    {
        for (int i = 0; i < levelsButton.Count; i++)
        {
            levelsDataList.levels[i].isUnlocked = levelsButton[i].isUnlocked;
            levelsDataList.levels[i].bestScore = levelsButton[i].bestScore;
            levelsDataList.levels[i].bestTime = levelsButton[i].bestTime;
        }

        string json = JsonUtility.ToJson(levelsDataList, true);
        File.WriteAllText(savePath, json);
    }

    // Update the appearance and functionality of level buttons based on their status
    private void UpdateLevelsButton()
    {
        for (int i = 0; i < levelsButton.Count; i++)
        {
            if (levelsButton[i].isUnlocked)
            {
                if (levelsButton[i].lockImage != null)
                {
                    levelsButton[i].lockImage.SetActive(false); // Hide lock icon for unlocked levels
                }

                levelsButton[i].button.image.color = unlockedColor;
                levelsButton[i].button.interactable = true;

                // Display the best score and time if available
                levelsButton[i].scoreText.text = levelsButton[i].bestScore > 0 ? "Best Score: " + levelsButton[i].bestScore : "";
                levelsButton[i].timeText.text = levelsButton[i].bestTime > 0 ? "Best Time: " + FormatTime(levelsButton[i].bestTime) : "";

                int levelIndex = i;
                levelsButton[i].button.onClick.AddListener(() => LoadLevel(levelsButton[levelIndex].sceneName)); // Assign button action
            }
            else
            {
                if (levelsButton[i].lockImage != null)
                {
                    levelsButton[i].lockImage.SetActive(true); // Show lock icon for locked levels
                }

                levelsButton[i].button.interactable = false;
                levelsButton[i].button.image.color = lockedColor;
            }
        }
    }

    // Format the elapsed time into a string (mm:ss)
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    // Load the specified level
    private void LoadLevel(string sceneName)
    {
        sceneLoader.LoadSceneAsync(sceneName);
    }

    // Unlock the next level and update the current level's score and time
    public void UnlockNextLevel(int currentLevel, int score, float elapsedTime)
    {
        if (currentLevel < levelsButton.Count - 1)
        {
            levelsButton[currentLevel + 1].isUnlocked = true; // Unlock the next level
        }

        if (score > levelsButton[currentLevel].bestScore)
        {
            levelsButton[currentLevel].bestScore = score;
        }

        if (elapsedTime > 0 && (levelsButton[currentLevel].bestTime == 0 || elapsedTime < levelsButton[currentLevel].bestTime))
        {
            levelsButton[currentLevel].bestTime = elapsedTime;
        }

        SaveLevelsStatus();
        UpdateLevelsButton();
    }

    // Get the name of the last unlocked level
    public string GetLastUnlockedLevel()
    {
        for (int i = levelsButton.Count - 1; i >= 0; i--)
        {
            if (levelsButton[i].isUnlocked)
            {
                return levelsButton[i].sceneName;
            }
        }

        return levelsButton[0].sceneName;
    }
}

[System.Serializable]
public class LevelButton
{
    // Data structure for each level button
    public Button button;
    public Text nameText;
    public Text scoreText;
    public Text timeText;
    public GameObject lockImage;
    public string sceneName;
    public bool isUnlocked;
    public int bestScore;
    public float bestTime;
}

[System.Serializable]
public class LevelData
{
    // Data structure for level information
    public string sceneName;
    public bool isUnlocked;
    public int bestScore;
    public float bestTime;
}

[System.Serializable]
public class LevelsDataList
{
    // List to hold multiple LevelData objects
    public List<LevelData> levels = new List<LevelData>();
}