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
        
            PhotonNetwork.LeaveRoom();
        }
        else
        {
          
            var gameController = FindObjectOfType<ChessGameController>();
            if (gameController != null)
            {
                gameController.RestartGame();
            }

            ShowMainMenu();
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left room. Returning to menu.");
        var gameController = FindObjectOfType<ChessGameController>();
        if (gameController != null)
        {
            Destroy(gameController.gameObject); // remove completamente o controlador antigo
        }
        ShowMainMenu(); // só mostra o menu, não reinicia nada
    }


    private void ShowMainMenu()
    {
        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);

        if (gameModeSelectionScreen != null)
            gameModeSelectionScreen.SetActive(true);
    }
}