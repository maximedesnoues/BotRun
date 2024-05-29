using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider; // Slider UI pour repr�senter visuellement la sant�
    [SerializeField] private Gradient gradient; // Gradient de couleur pour le remplissage du slider selon la sant�

    private Image fill; // L'image de remplissage du slider

    private void Awake()
    {
        // Initialisation du composant Image pour le remplissage du slider
        if (slider != null && slider.fillRect != null)
        {
            fill = slider.fillRect.GetComponent<Image>();
        }
    }

    // Met � jour la couleur du remplissage en fonction de la valeur normalis�e du slider
    private void UpdateColor()
    {
        if (fill != null)
        {
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }
    }

    // D�finit la sant� maximale pour le slider et met � jour la valeur et la couleur
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        UpdateColor();
    }

    // Met � jour la valeur actuelle de la sant� sur le slider et ajuste la couleur
    public void SetHealth(int health)
    {
        slider.value = health;
        UpdateColor();
    }
}