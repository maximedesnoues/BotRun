using UnityEngine;
using UnityEngine.UI;

public class BoostManager : MonoBehaviour
{
    // UI slider to display the boost amount
    [SerializeField] private Slider boostSlider;

    // Maximum boost value
    [SerializeField] private float maxBoost;

    // Current boost value
    private float currentBoost;

    private void Start()
    {
        // Initialize boost values and update the UI slider
        currentBoost = 0f;
        boostSlider.maxValue = maxBoost;
        boostSlider.value = currentBoost;
    }

    // Method to add boost, ensuring it doesn't exceed the maximum
    public void AddBoost(float boostAmount)
    {
        if (currentBoost < maxBoost)
        {
            currentBoost = Mathf.Min(currentBoost + boostAmount, maxBoost);
            boostSlider.value = currentBoost;
        }
    }

    // Method to consume (reset) the boost
    public void ConsumeBoost()
    {
        currentBoost = 0f;
        boostSlider.value = currentBoost;
    }

    // Method to check if the boost is full
    public bool IsBoostFull()
    {
        return currentBoost >= maxBoost;
    }
}