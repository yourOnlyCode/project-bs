using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetworkGameManagerV1 networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;

    public void HostLobby()
    {
        networkManager.StartHost();
        landingPagePanel.SetActive(false);
    }
}
