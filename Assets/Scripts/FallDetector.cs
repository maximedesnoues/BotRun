using UnityEngine;

public class FallDetector : MonoBehaviour
{
    // References to other components in the scene
    private Timer timer;
    private ScoreManager scoreManager;
    private Defeat defeat;

    private void Start()
    {
        // Initialize references to other components
        timer = FindObjectOfType<Timer>();
        scoreManager = FindObjectOfType<ScoreManager>();
        defeat = FindObjectOfType<Defeat>();
    }

    // Trigger detection when the player falls
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Show the defeat screen with relevant stats
            defeat.ShowDefeatScreen(scoreManager.GetTotalScore(), timer.elapsedTime, scoreManager.GetTricksPerformed(), scoreManager.GetCollectablesCollected().ToString());
        }
    }
}