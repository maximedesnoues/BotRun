using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    // [SerializeField] private AudioMixer audioMixer;

    // [SerializeField] private Slider volumeSlider;
    
    [SerializeField] private Dropdown resolutionsDropdown;
    [SerializeField] private Dropdown qualitiesDropdown;

    private string savePath;
    private SettingsData settingsData;

    private void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "settings.json");
        settingsData = new SettingsData();

        LoadSettings();
        InitializeSettings();
        ApplySettings();

        // volumeSlider.onValueChanged.AddListener(delegate { UpdateSettings(); });
        resolutionsDropdown.onValueChanged.AddListener(delegate { UpdateSettings(); });
        qualitiesDropdown.onValueChanged.AddListener(delegate { UpdateSettings(); });
    }

    private void InitializeSettings()
    {
        // Initialisation du slider de volume
        // volumeSlider.value = settingsData.volume;

        // Initialisation des différentes résolutions d'écran
        Resolution[] resolutions = Screen.resolutions;
        resolutionsDropdown.ClearOptions();

        List<string> resolutionOptions = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " (" + resolutions[i].refreshRate + " Hz)";
            resolutionOptions.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionsDropdown.AddOptions(resolutionOptions);
        resolutionsDropdown.value = settingsData.resolutionIndex;
        resolutionsDropdown.RefreshShownValue();

        // Initialisation des différentes qualités graphiques
        string[] qualities = QualitySettings.names;
        qualitiesDropdown.ClearOptions();

        List<string> qualityOptions = new List<string>();
        for (int i = 0; i < qualities.Length; i++)
        {
            qualityOptions.Add(qualities[i]);
        }

        qualitiesDropdown.AddOptions(qualityOptions);
        qualitiesDropdown.value = settingsData.qualityIndex;
        qualitiesDropdown.RefreshShownValue();
    }

    public void UpdateSettings()
    {
        // settingsData.volume = volumeSlider.value;
        settingsData.resolutionIndex = resolutionsDropdown.value;
        settingsData.qualityIndex = qualitiesDropdown.value;

        ApplySettings();
        SaveSettings();
    }

    private void ApplySettings()
    {
        // Appliquer le volume
        // audioMixer.SetFloat("Volume", settingsData.volume);

        // Appliquer la résolution
        Resolution resolution = Screen.resolutions[settingsData.resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        // Appliquer la qualité graphique
        QualitySettings.SetQualityLevel(settingsData.qualityIndex);
    }

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(settingsData, true);
        File.WriteAllText(savePath, json);
    }

    public void LoadSettings()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            settingsData = JsonUtility.FromJson<SettingsData>(json);
        }
    }
}

[System.Serializable]
public class SettingsData
{
    // public float volume;
    public int resolutionIndex;
    public int qualityIndex;
}