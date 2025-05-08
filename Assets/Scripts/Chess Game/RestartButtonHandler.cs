using UnityEngine;

public class RestartButtonHandler : MonoBehaviour
{
    private ChessGameController gameController;

    [SerializeField] private GameObject gameModeSelectionScreen;
    [SerializeField] private GameObject gameOverScreen;

    public void OnRestartButtonClicked()
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

        Debug.Log("Restart button clicked");
        gameController.RestartGame();  

        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);

        if (gameModeSelectionScreen != null)
            gameModeSelectionScreen.SetActive(true);
    }
}