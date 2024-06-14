using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private SceneLoader sceneLoader;
    private LevelManager levelManager;

    private void Start()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();
        levelManager = FindObjectOfType<LevelManager>();
    }

    public void PlayButton()
    {
        string lastUnlockedLevel = levelManager.GetLastUnlockedLevel();
        if (!string.IsNullOrEmpty(lastUnlockedLevel))
        {
            sceneLoader.LoadSceneAsync(lastUnlockedLevel);
        }
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}