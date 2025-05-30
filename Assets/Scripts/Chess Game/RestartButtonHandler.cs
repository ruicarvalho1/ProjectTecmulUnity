using UnityEngine;
using Photon.Pun;


public class RestartButtonHandler : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject gameModeSelectionScreen;
    [SerializeField] private GameObject gameOverScreen;

    public void OnRestartButtonClicked()
    {
        Debug.Log("Restart button clicked");
        var gameController = FindFirstObjectByType<ChessGameController>();
        
            if (gameController != null)
            {
                gameController.RestartGame();
            }

            ShowMainMenu();
        
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left room. Returning to menu.");
        var gameController = FindFirstObjectByType<ChessGameController>();
        if (gameController != null)
        {
            Destroy(gameController.gameObject); 
        }
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