using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // References to other components needed in the main menu
    private SceneLoader sceneLoader;
    private LevelManager levelManager;

    private void Start()
    {
        // Find the SceneLoader and LevelManager components in the scene
        sceneLoader = FindObjectOfType<SceneLoader>();
        levelManager = FindObjectOfType<LevelManager>();
    }

    // Called when the Play button is pressed
    public void PlayButton()
    {
        // Get the last unlocked level and load it
        string lastUnlockedLevel = levelManager.GetLastUnlockedLevel();

        if (!string.IsNullOrEmpty(lastUnlockedLevel))
        {
            sceneLoader.LoadSceneAsync(lastUnlockedLevel);
        }
    }

    // Called when the Quit button is pressed
    public void QuitButton()
    {
        // Quit the application
        Application.Quit();
    }
}