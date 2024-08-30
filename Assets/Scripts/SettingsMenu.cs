using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    // References to the audio mixer and UI elements for settings
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider soundEffectsVolumeSlider;

    [SerializeField] private Dropdown resolutionsDropdown;
    [SerializeField] private Dropdown qualitiesDropdown;

    // Paths and data for saving/loading settings
    private string savePath;
    private SettingsData settingsData;

    private void Start()
    {
        // Set the save path and initialize settings data
        savePath = Path.Combine(Application.persistentDataPath, "settings.json");
        settingsData = new SettingsData();

        // Load and apply settings, and set up listeners for UI changes
        LoadSettings();
        InitializeSettings();
        ApplySettings();

        // Add listeners to UI elements to handle changes
        masterVolumeSlider.onValueChanged.AddListener(delegate { UpdateSettings(); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { UpdateSettings(); });
        soundEffectsVolumeSlider.onValueChanged.AddListener(delegate { UpdateSettings(); });
        resolutionsDropdown.onValueChanged.AddListener(delegate { UpdateSettings(); });
        qualitiesDropdown.onValueChanged.AddListener(delegate { UpdateSettings(); });
    }

    private void InitializeSettings()
    {
        // Initialize volume sliders
        masterVolumeSlider.value = settingsData.masterVolume;
        musicVolumeSlider.value = settingsData.musicVolume;
        soundEffectsVolumeSlider.value = settingsData.soundEffectsVolume;

        // Initialize screen resolutions dropdown
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

        // Initialize graphics quality dropdown
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
        // Update settings data based on UI changes
        settingsData.masterVolume = masterVolumeSlider.value;
        settingsData.musicVolume = musicVolumeSlider.value;
        settingsData.soundEffectsVolume = soundEffectsVolumeSlider.value;
        settingsData.resolutionIndex = resolutionsDropdown.value;
        settingsData.qualityIndex = qualitiesDropdown.value;

        // Apply and save the updated settings
        ApplySettings();
        SaveSettings();
    }

    private void ApplySettings()
    {
        // Apply audio settings
        SetVolume("MasterVolume", settingsData.masterVolume);
        SetVolume("MusicVolume", settingsData.musicVolume);
        SetVolume("SoundEffectsVolume", settingsData.soundEffectsVolume);

        // Apply screen resolution
        Resolution resolution = Screen.resolutions[settingsData.resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        // Apply graphics quality settings
        QualitySettings.SetQualityLevel(settingsData.qualityIndex);
    }

    private void SetVolume(string parameter, float value)
    {
        float dB;

        if (value == 0)
        {
            dB = -80; // Mute when slider is at minimum
        }
        else
        {
            dB = Mathf.Log10(value) * 20; // Convert linear value to decibels
        }

        audioMixer.SetFloat(parameter, dB);
    }

    public void SaveSettings()
    {
        // Save settings data to a JSON file
        string json = JsonUtility.ToJson(settingsData, true);
        File.WriteAllText(savePath, json);
    }

    public void LoadSettings()
    {
        // Load settings data from a JSON file, if it exists
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
    // Serializable fields for storing settings data
    public float masterVolume = 0.5f;
    public float musicVolume = 0.5f;
    public float soundEffectsVolume = 0.5f;
    public int resolutionIndex;
    public int qualityIndex;
}