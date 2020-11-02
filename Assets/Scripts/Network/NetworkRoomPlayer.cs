using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class NetworkRoomPlayer : NetworkBehaviour
{

    [Header("UI")]
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private TMP_Text[] _playerNameTexts = new TMP_Text[4];
    [SerializeField] private TMP_Text[] _playerReadyTexts = new TMP_Text[4];
    [SerializeField] private Image[] _playerCharacterImages = new Image[4];
    [SerializeField] private Image _playerCharacterLargeImage = null;
    [SerializeField] private Button startGameButton = null;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string displayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool isReady = false;

    private bool isLeader;

    public bool IsLeader {
        set
        {
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
        }
    }

    private NetworkGameManagerV1 room;

    private NetworkGameManagerV1 Room {
        get
        {

            if(room != null) { return room;}
            return room = NetworkManager.singleton as NetworkGameManagerV1;


        }
    }

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(PlayerNameInput.DISPLAY_NAME);

        lobbyUI.SetActive(true);
    }

    public override void OnStartClient()
    {
        Room.roomPlayers.Add(this);
        Room.NotifyPlayersOfReadyState();
        UpdateDisplay();
    }

    public override void OnNetworkDestroy()
    {
        Room.roomPlayers.Remove(this);

        UpdateDisplay();
        lobbyUI.SetActive(true);
    }

    public void HandleReadyStatusChanged(bool pOldValue, bool pNewValue) => UpdateDisplay();
    public void HandleDisplayNameChanged(string pOldValue, string pNewValue) => UpdateDisplay();

    private void UpdateDisplay() {

        if(!hasAuthority) {
            for (int i=0; i<Room.roomPlayers.Count-1;i++) {
                if (Room.roomPlayers[i].hasAuthority) {
                    Room.roomPlayers[i].UpdateDisplay();
                    break;
                }
            }

            return;

        }

        for(int i=0; i < _playerNameTexts.Length; i++) {
            _playerNameTexts[i].text = "Waiting for player...";
            _playerReadyTexts[i].text = string.Empty;
        }

        for(int i = 0; i < Room.roomPlayers.Count; i++) {
            _playerNameTexts[i].text = Room.roomPlayers[i].displayName;
            _playerReadyTexts[i].text = Room.roomPlayers[i].isReady ?
                "<color=green>Ready</color>" :
                "<color=red>Not Ready</color>";
        }

    }

    public void HandleReadyToStart(bool pReadyToStart) {
        if(!isLeader) {return;}

        startGameButton.interactable = pReadyToStart;

    }

    [Command]
    private void CmdSetDisplayName(string pDisplayName) {
        displayName = pDisplayName;
    }

    [Command]
    public void CmdReadyUp() {
        isReady = !isReady;

        Room.NotifyPlayersOfReadyState();
    }

    [Command]
    public void CmdStartGame() {
        if(Room.roomPlayers[0].connectionToClient != connectionToClient) {return;}

        Room.StartGame();
    }

    public Image[] GetCharacterImages()
    {
        return _playerCharacterImages;
    }

}
