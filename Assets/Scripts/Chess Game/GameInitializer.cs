using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameInitializer : MonoBehaviour
{
    [Header("Game mode dependent objects")]

    [SerializeField] private SingleplayerChessGameController singleplayerControllerPrefab;
    [SerializeField] private MultiplayerChessGameController multiplayerControllerPrefab;
    [SerializeField] private MultiplayerBoard multiplayerBoardPrefab;
    [SerializeField] private SingleplayerBoard singleplayerBoardPrefab;

    [Header("Scene references")]
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private ChessUIManager uiManager;
    [SerializeField] private Transform boardAnchor;

    public void CreateMultiplayerBoard()
    {
        if (!networkManager.IsRoomFull())
            PhotonNetwork.Instantiate(multiplayerBoardPrefab.name, boardAnchor.position, boardAnchor.rotation);
    }

    public void CreateSinglePlayerBoard()
    {
        Instantiate(singleplayerBoardPrefab, boardAnchor);
    }
    
    public void InitializeMultiplayerController()
    {
        MultiplayerBoard board = FindObjectOfType<MultiplayerBoard>();
        if (board)
        {
            MultiplayerChessGameController controller = Instantiate(multiplayerControllerPrefab);
            controller.SetDependencies(uiManager, board);
            controller.CreatePlayers();
            controller.SetMultiplayerDependencies(networkManager);
            networkManager.SetDependencies(controller);
            board.SetDependencies(controller);
        }
    }
    
    public void InitializeSingleplayerController()
    {
        SingleplayerBoard board = FindObjectOfType<SingleplayerBoard>();
        if (board)
        {
            SingleplayerChessGameController controller = Instantiate(singleplayerControllerPrefab);
            controller.SetDependencies(uiManager, board);
            controller.CreatePlayers();
            board.SetDependencies(controller);
            controller.StartNewGame();
        }
    }



  
}