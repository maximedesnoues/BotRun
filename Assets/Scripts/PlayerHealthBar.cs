using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Gradient gradient;

    private Image fill;

    private void Awake()
    {
        if (slider != null && slider.fillRect != null)
        {
            fill = slider.fillRect.GetComponent<Image>();
        }
    }

    private void UpdateColor()
    {
        if (fill != null)
        {
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        UpdateColor();
    }

    public void SetHealth(int health)
    {
        slider.value = health;
        UpdateColor();
    }
}