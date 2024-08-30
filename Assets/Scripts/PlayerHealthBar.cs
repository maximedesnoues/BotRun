using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    // References to UI components
    [SerializeField] private Slider slider;
    [SerializeField] private Gradient gradient;

    private Image fill;

    private void Awake()
    {
        // Get the fill image component from the slider
        if (slider != null && slider.fillRect != null)
        {
            fill = slider.fillRect.GetComponent<Image>();
        }
    }

    // Update the color of the health bar based on current health
    private void UpdateColor()
    {
        if (fill != null)
        {
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }
    }

    // Set the maximum health and update the health bar
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        UpdateColor();
    }

    // Set the current health and update the health bar
    public void SetHealth(int health)
    {
        slider.value = health;
        UpdateColor();
    }
}