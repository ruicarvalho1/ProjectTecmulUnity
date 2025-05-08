using UnityEngine;
using Photon.Pun;  // necessário para usar PhotonNetwork

public class RestartButtonHandler : MonoBehaviourPunCallbacks
{
    private ChessGameController gameController;

    [SerializeField] private GameObject gameModeSelectionScreen;
    [SerializeField] private GameObject gameOverScreen;

    public void OnRestartButtonClicked()
    {
        Debug.Log("Restart button clicked");

        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Multiplayer detected: leaving room...");
            PhotonNetwork.LeaveRoom(); 
        }
        else
        {
            RestartLocalGame(); 
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left multiplayer room. Showing menu and cleaning up...");
        RestartLocalGame();
    }

    private void RestartLocalGame()
    {
        if (gameController == null)
        {
            gameController = FindObjectOfType<ChessGameController>();
            if (gameController == null)
            {
                Debug.LogWarning("No ChessGameController found.");
                return;
            }
        }

        gameController.RestartGame();

        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);

        if (gameModeSelectionScreen != null)
            gameModeSelectionScreen.SetActive(true);
    }
}