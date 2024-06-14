using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider loadingProgressBar;
    [SerializeField] private Text loadingProgressText;

    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(AsyncLoadScene(sceneName));
    }

    private IEnumerator AsyncLoadScene(string sceneName)
    {
        Time.timeScale = 1;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingProgressBar.value = progress;
            loadingProgressText.text = $"{progress * 100:0}%";

            yield return null;
        }
    }
}