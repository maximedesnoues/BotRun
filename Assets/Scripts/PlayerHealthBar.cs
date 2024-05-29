using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider; // Slider UI pour représenter visuellement la santé
    [SerializeField] private Gradient gradient; // Gradient de couleur pour le remplissage du slider selon la santé

    private Image fill; // L'image de remplissage du slider

    private void Awake()
    {
        // Initialisation du composant Image pour le remplissage du slider
        if (slider != null && slider.fillRect != null)
        {
            fill = slider.fillRect.GetComponent<Image>();
        }
    }

    // Met à jour la couleur du remplissage en fonction de la valeur normalisée du slider
    private void UpdateColor()
    {
        if (fill != null)
        {
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }
    }

    // Définit la santé maximale pour le slider et met à jour la valeur et la couleur
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        UpdateColor();
    }

    // Met à jour la valeur actuelle de la santé sur le slider et ajuste la couleur
    public void SetHealth(int health)
    {
        slider.value = health;
        UpdateColor();
    }
}