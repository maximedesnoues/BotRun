using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Controls : MonoBehaviour
{
    [SerializeField] private Dropdown moveForwardDropdown;
    [SerializeField] private Dropdown moveBackwardDropdown;
    [SerializeField] private Dropdown turnLeftDropdown;
    [SerializeField] private Dropdown turnRightDropdown;
    [SerializeField] private Dropdown jumpDropdown;
    [SerializeField] private Dropdown climbDropdown;
    [SerializeField] private Dropdown wallRunDropdown;
    [SerializeField] private Dropdown boostDropdown;
    [SerializeField] private Dropdown miniMapDropdown;

    [HideInInspector] public Dictionary<string, KeyCode> controls;

    private string savePath;

    private int previousMoveForwardValue;
    private int previousMoveBackwardValue;
    private int previousTurnLeftValue;
    private int previousTurnRightValue;
    private int previousJumpValue;
    private int previousClimbValue;
    private int previousWallRunValue;
    private int previousBoostValue;
    private int previousMiniMapValue;

    private void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "controls.json");
        controls = new Dictionary<string, KeyCode>
        {
            {"MoveForward", KeyCode.UpArrow},
            {"MoveBackward", KeyCode.DownArrow},
            {"TurnLeft", KeyCode.LeftArrow},
            {"TurnRight", KeyCode.RightArrow},
            {"Jump", KeyCode.Space},
            {"Climb", KeyCode.C},
            {"WallRun", KeyCode.LeftShift},
            {"Boost", KeyCode.B},
            {"MiniMap", KeyCode.V}
        };

        LoadSettings();
        PopulateDropdowns();

        SavePreviousDropdownValues();

        moveForwardDropdown.onValueChanged.AddListener(delegate { ValidateAndSaveControls(moveForwardDropdown, ref previousMoveForwardValue); });
        moveBackwardDropdown.onValueChanged.AddListener(delegate { ValidateAndSaveControls(moveBackwardDropdown, ref previousMoveBackwardValue); });
        turnLeftDropdown.onValueChanged.AddListener(delegate { ValidateAndSaveControls(turnLeftDropdown, ref previousTurnLeftValue); });
        turnRightDropdown.onValueChanged.AddListener(delegate { ValidateAndSaveControls(turnRightDropdown, ref previousTurnRightValue); });
        jumpDropdown.onValueChanged.AddListener(delegate { ValidateAndSaveControls(jumpDropdown, ref previousJumpValue); });
        climbDropdown.onValueChanged.AddListener(delegate { ValidateAndSaveControls(climbDropdown, ref previousClimbValue); });
        wallRunDropdown.onValueChanged.AddListener(delegate { ValidateAndSaveControls(wallRunDropdown, ref previousWallRunValue); });
        boostDropdown.onValueChanged.AddListener(delegate { ValidateAndSaveControls(boostDropdown, ref previousBoostValue); });
        miniMapDropdown.onValueChanged.AddListener(delegate { ValidateAndSaveControls(miniMapDropdown, ref previousMiniMapValue); });
    }

    private void PopulateDropdowns()
    {
        List<string> options = new List<string>();
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            options.Add(key.ToString());
        }

        InitializeDropdown(moveForwardDropdown, options, controls["MoveForward"]);
        InitializeDropdown(moveBackwardDropdown, options, controls["MoveBackward"]);
        InitializeDropdown(turnLeftDropdown, options, controls["TurnLeft"]);
        InitializeDropdown(turnRightDropdown, options, controls["TurnRight"]);
        InitializeDropdown(jumpDropdown, options, controls["Jump"]);
        InitializeDropdown(climbDropdown, options, controls["Climb"]);
        InitializeDropdown(wallRunDropdown, options, controls["WallRun"]);
        InitializeDropdown(boostDropdown, options, controls["Boost"]);
        InitializeDropdown(miniMapDropdown, options, controls["MiniMap"]);
    }

    private void InitializeDropdown(Dropdown dropdown, List<string> options, KeyCode currentKey)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        dropdown.value = options.IndexOf(currentKey.ToString());
        dropdown.RefreshShownValue();
    }

    private void SavePreviousDropdownValues()
    {
        previousMoveForwardValue = moveForwardDropdown.value;
        previousMoveBackwardValue = moveBackwardDropdown.value;
        previousTurnLeftValue = turnLeftDropdown.value;
        previousTurnRightValue = turnRightDropdown.value;
        previousJumpValue = jumpDropdown.value;
        previousClimbValue = climbDropdown.value;
        previousWallRunValue = wallRunDropdown.value;
        previousBoostValue = boostDropdown.value;
        previousMiniMapValue = miniMapDropdown.value;
    }

    public void ValidateAndSaveControls(Dropdown changedDropdown, ref int previousValue)
    {
        Dictionary<string, KeyCode> newControls = new Dictionary<string, KeyCode>
        {
            {"MoveForward", (KeyCode)System.Enum.Parse(typeof(KeyCode), moveForwardDropdown.options[moveForwardDropdown.value].text)},
            {"MoveBackward", (KeyCode)System.Enum.Parse(typeof(KeyCode), moveBackwardDropdown.options[moveBackwardDropdown.value].text)},
            {"TurnLeft", (KeyCode)System.Enum.Parse(typeof(KeyCode), turnLeftDropdown.options[turnLeftDropdown.value].text)},
            {"TurnRight", (KeyCode)System.Enum.Parse(typeof(KeyCode), turnRightDropdown.options[turnRightDropdown.value].text)},
            {"Jump", (KeyCode)System.Enum.Parse(typeof(KeyCode), jumpDropdown.options[jumpDropdown.value].text)},
            {"Climb", (KeyCode)System.Enum.Parse(typeof(KeyCode), climbDropdown.options[climbDropdown.value].text)},
            {"WallRun", (KeyCode)System.Enum.Parse(typeof(KeyCode), wallRunDropdown.options[wallRunDropdown.value].text)},
            {"Boost", (KeyCode)System.Enum.Parse(typeof(KeyCode), boostDropdown.options[boostDropdown.value].text)},
            {"MiniMap", (KeyCode)System.Enum.Parse(typeof(KeyCode), miniMapDropdown.options[miniMapDropdown.value].text)}
        };

        if (IsUnique(newControls))
        {
            controls = newControls;
            SaveSettings();
            SavePreviousDropdownValues();

            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.UpdateControlKeys(controls);
            }
        }
        else
        {
            changedDropdown.value = previousValue;
            changedDropdown.RefreshShownValue();
        }
    }

    private bool IsUnique(Dictionary<string, KeyCode> newControls)
    {
        HashSet<KeyCode> usedKeys = new HashSet<KeyCode>();

        foreach (KeyCode key in newControls.Values)
        {
            if (!usedKeys.Add(key))
            {
                return false;
            }
        }

        return true;
    }

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(new SerializableDictionary<string, KeyCode>(controls), true);
        File.WriteAllText(savePath, json);
    }

    public void LoadSettings()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SerializableDictionary<string, KeyCode> loadedControls = JsonUtility.FromJson<SerializableDictionary<string, KeyCode>>(json);
            controls = new Dictionary<string, KeyCode>(loadedControls);
        }
    }

    public void CancelSettings()
    {
        PopulateDropdowns();
        SavePreviousDropdownValues();
    }
}

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    public SerializableDictionary() : base() { }
    public SerializableDictionary(IDictionary<TKey, TValue> dict) : base(dict) { }

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();

        if (keys.Count != values.Count)
        {
            throw new System.Exception("There are " + keys.Count + " keys and " + values.Count + " values after deserialization. Make sure that both key and value types are serializable.");
        }

        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }
}