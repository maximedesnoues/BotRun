using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoostManager : MonoBehaviour
{
    [SerializeField] private Slider boostSlider;
    [SerializeField] private float maxBoost;

    private float currentBoost;

    private void Start()
    {
        currentBoost = 0f;
        boostSlider.maxValue = maxBoost;
        boostSlider.value = currentBoost;
    }

    public void AddBoost(float boostAmount)
    {
        if (currentBoost < maxBoost)
        {
            currentBoost = Mathf.Min(currentBoost + boostAmount, maxBoost);
            boostSlider.value = currentBoost;
        }
    }

    public void ConsumeBoost()
    {
        currentBoost = 0f;
        boostSlider.value = currentBoost;
    }

    public bool IsBoostFull()
    {
        return currentBoost >= maxBoost;
    }
}