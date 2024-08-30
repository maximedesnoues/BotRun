using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    // UI elements for the pause menu
    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;

    // Reference to the SceneLoader component
    private SceneLoader sceneLoader;

    // Boolean to track if the game is paused
    private bool isPaused = false;

    private void Start()
    {
        // Find the SceneLoader component in the scene
        sceneLoader = FindObjectOfType<SceneLoader>();
    }

    private void Update()
    {
        // Check if the Escape key is pressed to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                Pause();
            }
        }
    }

    // Pauses the game and shows the pause menu
    private void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    // Resumes the game and hides the pause menu
    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    // Returns to the main menu
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        sceneLoader.LoadSceneAsync("MainMenuScene");
    }
}