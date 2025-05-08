using UnityEngine;
using Photon.Pun;

public class RestartButtonHandler : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject gameModeSelectionScreen;
    [SerializeField] private GameObject gameOverScreen;

    public void OnRestartButtonClicked()
    {
        Debug.Log("Restart button clicked");

        var gameController = FindObjectOfType<ChessGameController>();
        if (gameController != null)
        {
            gameController.RestartGame();
        }
        else
        {
            Debug.LogWarning("No ChessGameController found.");
        }

        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Multiplayer: leaving room after restart.");
            PhotonNetwork.LeaveRoom(); 
        }
        else
        {
            ShowMainMenu(); 
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Multiplayer: left room. Showing main menu.");
        ShowMainMenu();
    }

    private void ShowMainMenu()
    {
        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);

        if (gameModeSelectionScreen != null)
            gameModeSelectionScreen.SetActive(true);
    }
}