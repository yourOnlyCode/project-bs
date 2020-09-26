using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField nameInputField = null;
    [SerializeField] private Button continueButton = null;

    public static string DISPLAY_NAME { get; private set; }
    private const string _PLAYER_PREFS_NAME_KEY = "PlayerName";

    private void Start() => SetUpInputField();

    private void SetUpInputField()
    {
        if (!PlayerPrefs.HasKey(_PLAYER_PREFS_NAME_KEY)) { return; }

        string defaultName = PlayerPrefs.GetString(_PLAYER_PREFS_NAME_KEY);

        nameInputField.text = defaultName;

        SetPlayerName(defaultName);
    }

    public void SetPlayerName(string name)
    {
        continueButton.interactable = !string.IsNullOrEmpty(name);
    }

    public void SavePlayerName()
    {
        DISPLAY_NAME = nameInputField.text;
        PlayerPrefs.SetString(_PLAYER_PREFS_NAME_KEY, DISPLAY_NAME);
    }
}
