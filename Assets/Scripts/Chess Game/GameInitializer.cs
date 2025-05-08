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
    [SerializeField] private CameraSetup cameraSetup;

    public void CreateMultiplayerBoard()
    {
        if (!networkManager.IsRoomFull())
        {
            PhotonNetwork.Instantiate(multiplayerBoardPrefab.name, boardAnchor.position, boardAnchor.rotation);
            StartCoroutine(WaitForBoardAndInit());
        }
    }

    private IEnumerator WaitForBoardAndInit()
    {
        yield return new WaitUntil(() => FindObjectOfType<MultiplayerBoard>() != null);

        // Extra frame delay to ensure everything is initialized
        yield return new WaitForEndOfFrame();

        InitializeMultiplayerController();
    }

    public void InitializeMultiplayerController()
    {
        MultiplayerBoard board = FindObjectOfType<MultiplayerBoard>();
        if (board)
        {
            MultiplayerChessGameController controller = Instantiate(multiplayerControllerPrefab);
            controller.SetDependencies(uiManager, board, cameraSetup);
            controller.CreatePlayers();
            controller.SetMultiplayerDependencies(networkManager);
            networkManager.SetDependencies(controller);
            board.SetDependencies(controller);

            controller.StartNewGame();
        }
        else
        {
            Debug.LogError("MultiplayerBoard not found! Ensure it was instantiated correctly.");
        }
    }

    public void CreateSinglePlayerBoard()
    {
        Instantiate(singleplayerBoardPrefab, boardAnchor);
    }

    public void InitializeSingleplayerController()
    {
        SingleplayerBoard board = FindObjectOfType<SingleplayerBoard>();
        if (board)
        {
            SingleplayerChessGameController controller = Instantiate(singleplayerControllerPrefab);
            controller.SetDependencies(uiManager, board, cameraSetup);
            controller.CreatePlayers();
            board.SetDependencies(controller);
            controller.StartNewGame();
        }
        else
        {
            Debug.LogError("SingleplayerBoard not found! Ensure it was instantiated correctly.");
        }
    }
}