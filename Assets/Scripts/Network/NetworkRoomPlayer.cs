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
    [SerializeField] private CharacterSelection _characterSelection = null;

    private Sprite[] _characterSprites = null;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string displayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool isReady = false;
    [SyncVar(hook = nameof(HandleCharacterSelectionChanged))]
    public int characterIndex = -1;

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

    public override void OnStopClient()
    {
        Room.roomPlayers.Remove(this);

        UpdateDisplay();
        lobbyUI.SetActive(true);
    }

    public void HandleReadyStatusChanged(bool pOldValue, bool pNewValue) => UpdateDisplay();
    public void HandleDisplayNameChanged(string pOldValue, string pNewValue) => UpdateDisplay();
    public void HandleCharacterSelectionChanged(int pOldValue, int pNewValue) => UpdateDisplay();

    private void UpdateDisplay() {

        if(!hasAuthority) {
            for (int i=0; i<Room.roomPlayers.Count;i++) {
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
            _playerCharacterImages[i].sprite = null;
        }

        for(int i = 0; i < Room.roomPlayers.Count; i++) {
            _playerNameTexts[i].text = Room.roomPlayers[i].displayName;
            _playerReadyTexts[i].text = Room.roomPlayers[i].isReady ?
                "<color=green>Ready</color>" :
                "<color=red>Not Ready</color>";
            if (Room.roomPlayers[i].characterIndex > -1)
            {
                Image charImage = _characterSelection.GetCharacterOptions()[Room.roomPlayers[i].characterIndex].transform.GetChild(0).GetComponent<Image>();
                _playerCharacterImages[i].sprite = charImage.sprite;
            }
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

    [Command]
    public void CmdSetCharacterIndex(int pIndex)
    {
        characterIndex = pIndex;
    }

    public int GetCharacterIndex()
    {
        return characterIndex;
    }

}
