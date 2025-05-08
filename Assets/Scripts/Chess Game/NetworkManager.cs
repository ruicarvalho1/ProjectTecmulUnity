using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
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

    private bool shouldJoinAfterConnect = false; // <- novo flag

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
        if (PhotonNetwork.IsConnected && PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
        {
            Debug.Log("Already connected to Master Server — joining room.");
            JoinRoom();
        }
        else
        {
            Debug.Log("Connecting to Photon...");
            shouldJoinAfterConnect = true; // <- ativar flag
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void JoinRoom()
    {
        Debug.Log($"Trying to join random room with level: {playerLevel}");
        PhotonNetwork.JoinRandomRoom(
            new ExitGames.Client.Photon.Hashtable() { { LEVEL, playerLevel } },
            MAX_PLAYERS
        );
    }

    private void Update()
    {
        uiManager.SetConnectionStatusText(PhotonNetwork.NetworkClientState.ToString());
    }

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.LogError($"Connected to Master Server.");
        if (shouldJoinAfterConnect)
        {
            shouldJoinAfterConnect = false;
            JoinRoom();
        }

        PhotonNetwork.JoinLobby(); // opcional, útil se usares lobby info na UI
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogError($"Joining random room failed: {message}. Creating new room.");
        PhotonNetwork.CreateRoom(null, new RoomOptions
        {
            CustomRoomPropertiesForLobby = new string[] { LEVEL },
            MaxPlayers = MAX_PLAYERS,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { LEVEL, playerLevel } }
        });
    }

    public override void OnJoinedRoom()
    {
        Debug.LogError($"Player {PhotonNetwork.LocalPlayer.ActorNumber} joined room with level: {(ChessLevel)PhotonNetwork.CurrentRoom.CustomProperties[LEVEL]}");

        if (gameInitializer == null)
            return;

        gameInitializer.CreateMultiplayerBoard();
        PrepareTeamSelectionOptions();
        uiManager?.ShowTeamSelectionScreen();
    }

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

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogError($"Player {newPlayer.ActorNumber} entered the room");
    }

    #endregion

    public void SetPlayerLevel(ChessLevel level)
    {
        playerLevel = level;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { LEVEL, level } });
    }

    public void SetPlayerTeam(int teamInt)
    {
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
                return;
        }

        chessGameController.SetLocalPlayer((TeamColor)teamInt);
        chessGameController.StartNewGame();
        chessGameController.SetupCamera((TeamColor)teamInt);
    }

    public bool IsRoomFull()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers;
    }
}
