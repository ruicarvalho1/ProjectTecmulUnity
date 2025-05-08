using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MultiplayerChessGameController : ChessGameController, IOnEventCallback
{
    
    protected const byte SET_GAME_STATE_EVENT_CODE = 1;
    private NetworkManager networkManager;
    private ChessPlayer localPlayer;

    public void SetMultiplayerDependencies (NetworkManager networkManager)
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
        if (networkManager.IsRoomFull())
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
        object[] content = new object[] { (int)state };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(SET_GAME_STATE_EVENT_CODE, state, raiseEventOptions, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == SET_GAME_STATE_EVENT_CODE)
        {
            object[] data = (object[])photonEvent.CustomData;
            GameState state = (GameState)data[0];
            this.gameState = state;
        }
    }
}