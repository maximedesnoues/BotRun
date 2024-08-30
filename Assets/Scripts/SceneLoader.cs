using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // UI elements for displaying loading progress
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider loadingProgressBar;
    [SerializeField] private Text loadingProgressText;

    // Public method to start loading a scene asynchronously
    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(AsyncLoadScene(sceneName));
    }

    // Coroutine to handle the asynchronous scene loading process
    private IEnumerator AsyncLoadScene(string sceneName)
    {
        // Ensure that the game time scale is set to 1
        Time.timeScale = 1;

        // Begin loading the scene asynchronously
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        loadingScreen.SetActive(true); // Show the loading screen

        // Update the loading progress UI while the scene is loading
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingProgressBar.value = progress;
            loadingProgressText.text = $"{progress * 100:0}%";

            yield return null; // Wait for the next frame
        }
    }
}