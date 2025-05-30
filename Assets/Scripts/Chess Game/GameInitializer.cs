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
            GameObject boardObj = PhotonNetwork.Instantiate(multiplayerBoardPrefab.name, boardAnchor.position, boardAnchor.rotation);
            StartCoroutine(WaitForBoardAndInit());
        }
    }

    private IEnumerator WaitForBoardAndInit()
    {
        yield return new WaitUntil(() => FindFirstObjectByType<MultiplayerBoard>() != null);
        InitializeMultiplayerController();
    }

    public void CreateSinglePlayerBoard()
    {
        Instantiate(singleplayerBoardPrefab, boardAnchor);
    }

    public void InitializeMultiplayerController()
    {
        MultiplayerBoard board = FindFirstObjectByType<MultiplayerBoard>();
        if (board)
        {
            MultiplayerChessGameController controller = Instantiate(multiplayerControllerPrefab);
            controller.SetDependencies(uiManager, board, cameraSetup);
            controller.CreatePlayers();
            controller.SetMultiplayerDependencies(networkManager);
            networkManager.SetDependencies(controller);
            board.SetDependencies(controller);

            // Garantir que o jogo inicia corretamente se necessário
            // controller.StartNewGame(); // Descomente se o jogo não estiver a iniciar automaticamente
        }
        else
        {
            Debug.Log("MultiplayerBoard not found! Certifique-se que foi instanciado corretamente.");
        }
    }

    public void InitializeSingleplayerController()
    {
        SingleplayerBoard board = FindFirstObjectByType<SingleplayerBoard>();
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
            Debug.Log("SingleplayerBoard not found! Certifique-se que foi instanciado corretamente.");
        }
    }
}
