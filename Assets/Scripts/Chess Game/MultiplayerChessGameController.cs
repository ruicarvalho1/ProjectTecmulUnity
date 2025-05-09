using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MultiplayerChessGameController : ChessGameController, IOnEventCallback
{
    protected const byte SET_GAME_STATE_EVENT_CODE = 1;

    private NetworkManager networkManager;
    private ChessPlayer localPlayer;

    public void SetMultiplayerDependencies(NetworkManager networkManager)
    {
        this.networkManager = networkManager;
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void SetLocalPlayer(TeamColor team)
    {
        localPlayer = team == TeamColor.White ? whitePlayer : blackPlayer;
    }

    public bool IsLocalPlayersTurn()
    {
        return localPlayer == activePlayer;
    }

    public override void TryToStartThisGame()
    {
        if (networkManager != null && networkManager.IsRoomFull())
        {
            SetGameState(GameState.Play);
        }
    }

    public override bool CanPerformMove()
    {
        return IsGameInProgress() && IsLocalPlayersTurn();
    }

    protected override void SetGameState(GameState state)
    {
        this.gameState = state;

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All
        };

        PhotonNetwork.RaiseEvent(
            SET_GAME_STATE_EVENT_CODE,
            (int)state,
            raiseEventOptions,
            SendOptions.SendReliable
        );
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == SET_GAME_STATE_EVENT_CODE)
        {
            if (photonEvent.CustomData is int stateInt && Enum.IsDefined(typeof(GameState), stateInt))
            {
                this.gameState = (GameState)stateInt;
                Debug.Log($"Game state synchronized: {gameState}");
            }
            else
            {
                Debug.Log("Invalid data received in SET_GAME_STATE_EVENT_CODE.");
            }
        }
    }
}
