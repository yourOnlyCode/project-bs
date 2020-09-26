using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private NetworkGameManagerV1 networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private TMP_InputField ipAddressInputField = null;
    [SerializeField] private Button joinButton = null;

    private void OnEnable()
    {
        NetworkGameManagerV1.OnClientConnected += HandleClientConnected;
        NetworkGameManagerV1.OnClientDisconnected += HandleClientDisconnected;
    }

    public void JoinLobby()
    {
        string ipAddress = ipAddressInputField.text;

        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();

        joinButton.interactable = false;
    }

    private void HandleClientConnected()
    {
        joinButton.interactable = true;

        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);

    }

    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
    }

}
