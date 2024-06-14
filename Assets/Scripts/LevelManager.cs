using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public List<LevelButton> levelsButton;
    
    [SerializeField] private Color unlockedColor;
    [SerializeField] private Color lockedColor;

    private string savePath;
    private LevelsDataList levelsDataList;
    private SceneLoader sceneLoader;

    private void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "levelsData.json");
        levelsDataList = new LevelsDataList();
        sceneLoader = FindObjectOfType<SceneLoader>();

        LoadLevelsStatus();
        UpdateLevelsButton();
    }

    private void LoadLevelsStatus()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            levelsDataList = JsonUtility.FromJson<LevelsDataList>(json);
        }
        else
        {
            for (int i = 0; i < levelsButton.Count; i++)
            {
                LevelData levelData = new LevelData
                {
                    sceneName = levelsButton[i].sceneName,
                    isUnlocked = i == 0,
                    bestScore = 0,
                    bestTime = 0f
                };
                
                levelsDataList.levels.Add(levelData);
            }
            
            SaveLevelsStatus();
        }

        for (int i = 0; i < levelsButton.Count; i++)
        {
            levelsButton[i].isUnlocked = levelsDataList.levels[i].isUnlocked;
            levelsButton[i].bestScore = levelsDataList.levels[i].bestScore;
            levelsButton[i].bestTime = levelsDataList.levels[i].bestTime;
        }
    }

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

    private void UpdateLevelsButton()
    {
        for (int i = 0; i < levelsButton.Count; i++)
        {
            if (levelsButton[i].isUnlocked)
            {
                if (levelsButton[i].lockImage != null)
                {
                    levelsButton[i].lockImage.SetActive(false);
                }

                levelsButton[i].button.image.color = unlockedColor;
                levelsButton[i].button.interactable = true;
                
                if (levelsButton[i].bestScore > 0)
                {
                    levelsButton[i].scoreText.text = "Best Score: " + levelsButton[i].bestScore;
                }
                else
                {
                    levelsButton[i].scoreText.text = "";
                }

                if (levelsButton[i].bestTime > 0)
                {
                    levelsButton[i].timeText.text = "Best Time: " + FormatTime(levelsButton[i].bestTime);
                }
                else
                {
                    levelsButton[i].timeText.text = "";
                }

                int levelIndex = i;
                levelsButton[i].button.onClick.AddListener(() => LoadLevel(levelsButton[levelIndex].sceneName));
            }
            else
            {
                if (levelsButton[i].lockImage != null)
                {
                    levelsButton[i].lockImage.SetActive(true);
                }

                levelsButton[i].button.interactable = false;
                levelsButton[i].button.image.color = lockedColor;
            }
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    private void LoadLevel(string sceneName)
    {
        sceneLoader.LoadSceneAsync(sceneName);
    }

    public void UnlockNextLevel(int currentLevel, int score, float elapsedTime)
    {
        if (currentLevel < levelsButton.Count - 1)
        {
            levelsButton[currentLevel + 1].isUnlocked = true;
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
    public string sceneName;
    public bool isUnlocked;
    public int bestScore;
    public float bestTime;
}

[System.Serializable]
public class LevelsDataList
{
    public List<LevelData> levels = new List<LevelData>();
}