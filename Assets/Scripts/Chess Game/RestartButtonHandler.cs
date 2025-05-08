using UnityEngine;
using Photon.Pun;

public class RestartButtonHandler : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject gameModeSelectionScreen;
    [SerializeField] private GameObject gameOverScreen;

    public void OnRestartButtonClicked()
    {
        Debug.Log("Restart button clicked");

        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Multiplayer: leaving room...");
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            ShowMainMenu();
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left room. Returning to main menu.");
        ShowMainMenu();
    }

    private void ShowMainMenu()
    {
        var gameController = FindObjectOfType<ChessGameController>();
        if (gameController != null)
        {
            Destroy(gameController.gameObject);
        }

        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);

        if (gameModeSelectionScreen != null)
            gameModeSelectionScreen.SetActive(true);
    }
}