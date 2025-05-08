using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private const string LEVEL = "level";
    private const string TEAM = "team";
    private const byte MAX_PLAYERS = 2;

    [SerializeField] private ChessUIManager uiManager;
    [SerializeField] private GameInitializer gameInitializer;
    private MultiplayerChessGameController chessGameController;

    private ChessLevel playerLevel;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void SetDependencies(MultiplayerChessGameController controller)
    {
        this.chessGameController = controller;
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Already connected to Photon.");
     
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void Update()
    {
        uiManager.SetConnectionStatusText(PhotonNetwork.NetworkClientState.ToString());
    }

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log($"Connected to Master Server. Ready to join/create room manually.");
        PhotonNetwork.JoinLobby(); 
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Joining random room failed: {message}. Creating new room.");
        PhotonNetwork.CreateRoom(null, new RoomOptions
        {
            MaxPlayers = MAX_PLAYERS,
            CustomRoomPropertiesForLobby = new string[] { LEVEL },
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { LEVEL, playerLevel } }
        });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Player {PhotonNetwork.LocalPlayer.ActorNumber} joined room.");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player {newPlayer.ActorNumber} entered the room. Current count: {PhotonNetwork.CurrentRoom.PlayerCount}");

        if (IsRoomFull())
        {
            Debug.Log("Room is full. Starting multiplayer game setup...");
            gameInitializer.CreateMultiplayerBoard();
            PrepareTeamSelectionOptions();
            uiManager?.ShowTeamSelectionScreen();
        }
    }

    #endregion

    private void PrepareTeamSelectionOptions()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            var player = PhotonNetwork.CurrentRoom.GetPlayer(1);
            if (player.CustomProperties.ContainsKey(TEAM))
            {
                var occupiedTeam = player.CustomProperties[TEAM];
                uiManager.RestrictTeamChoice((TeamColor)occupiedTeam);
            }
        }
    }

    public void SetPlayerLevel(ChessLevel level)
    {
        playerLevel = level;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { LEVEL, level } });
    }

    public void SetPlayerTeam(int teamInt)
    {
        if (!IsRoomFull())
        {
            Debug.LogWarning("Room is not full. Cannot set team or start game.");
            return;
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            var player = PhotonNetwork.CurrentRoom.GetPlayer(1);
            if (player.CustomProperties.ContainsKey(TEAM))
            {
                var occupiedTeam = player.CustomProperties[TEAM];
                teamInt = (int)occupiedTeam == 0 ? 1 : 0;
            }
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { TEAM, teamInt } });

        gameInitializer.InitializeMultiplayerController();

        if (chessGameController == null)
        {
            chessGameController = FindObjectOfType<MultiplayerChessGameController>();
            if (chessGameController == null)
            {
                Debug.LogError("MultiplayerChessGameController not found after init.");
                return;
            }
        }

        chessGameController.SetLocalPlayer((TeamColor)teamInt);
        chessGameController.StartNewGame();
        chessGameController.SetupCamera((TeamColor)teamInt);
    }

    public bool IsRoomFull()
    {
        return PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers;
    }

 
    public void JoinOrCreateMultiplayerRoom()
    {
        PhotonNetwork.JoinRandomRoom(new ExitGames.Client.Photon.Hashtable() { { LEVEL, playerLevel } }, MAX_PLAYERS);
    }
}
