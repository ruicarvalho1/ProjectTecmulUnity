using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PieceCreator))]
public abstract class ChessGameController : MonoBehaviour
{
    public enum GameState { Init, Play, Finished }

    [SerializeField] private BoardLayout startingBoardLayout;
    private Board board;
    private ChessUIManager uiManager;
    private CameraSetup cameraSetup;

    private PieceCreator pieceCreator;
    protected ChessPlayer whitePlayer;
    protected ChessPlayer blackPlayer;
    protected ChessPlayer activePlayer;
    protected GameState gameState;

    protected abstract void SetGameState(GameState state);
    public abstract void TryToStartThisGame();
    public abstract bool CanPerformMove();

    private void Awake()
    {
        pieceCreator = GetComponent<PieceCreator>();
    }

    internal void SetDependencies(ChessUIManager uIManager, Board board, CameraSetup cameraSetup)
    {
        this.uiManager = uIManager;
        this.board = board;
        this.cameraSetup = cameraSetup;
    }

    public void InitializeGame()
    {
        CreatePlayers();
    }

    public void CreatePlayers()
    {
        whitePlayer = new ChessPlayer(TeamColor.White, board);
        blackPlayer = new ChessPlayer(TeamColor.Black, board);
    }

    public void StartNewGame()
    {
        uiManager.OnGameStarted();
        SetGameState(GameState.Init);
        CreatePiecesFromLayout(startingBoardLayout);
        activePlayer = whitePlayer;
        GenerateAllPossiblePlayerMoves(activePlayer);
        SetGameState(GameState.Play);
    }

    public void RestartGame()
    {
        DestroyAllPieces();
        board.OnGameRestarted();
        whitePlayer.OnGameRestarted();
        blackPlayer.OnGameRestarted();
        StartNewGame();
        TryToStartThisGame();
    }

    private void DestroyAllPieces()
    {
        whitePlayer.activePieces.ForEach(piece => Destroy(piece.gameObject));
        blackPlayer.activePieces.ForEach(piece => Destroy(piece.gameObject));
    }

    public bool IsGameInProgress()
    {
        return gameState == GameState.Play;
    }

    private void CreatePiecesFromLayout(BoardLayout layout)
    {
        for (int i = 0; i < layout.GetPiecesCount(); i++)
        {
            Vector2Int squareCoords = layout.GetSquareCoordsAtIndex(i);
            TeamColor team = layout.GetSquareTeamColorAtIndex(i);
            string typeName = layout.GetSquarePieceNameAtIndex(i);

            Type type = Type.GetType(typeName);
            CreatePieceAndInitialize(squareCoords, team, type);
        }
    }

    public void CreatePieceAndInitialize(Vector2Int squareCoords, TeamColor team, Type type)
    {
        Piece newPiece = pieceCreator.CreatePiece(type, team).GetComponent<Piece>();
        newPiece.SetData(squareCoords, team, board);

        Material teamMaterial = pieceCreator.GetTeamMaterial(team);
        newPiece.SetMaterial(teamMaterial);

        board.SetPieceOnBoard(squareCoords, newPiece);

        ChessPlayer currentPlayer = team == TeamColor.White ? whitePlayer : blackPlayer;
        currentPlayer.AddPiece(newPiece);
    }

    private void GenerateAllPossiblePlayerMoves(ChessPlayer player)
    {
        player.GenerateAllPossibleMoves();
    }

    public bool IsTeamTurnActive(TeamColor team)
    {
        return activePlayer.team == team;
    }

    public void EndTurn()
    {
        GenerateAllPossiblePlayerMoves(activePlayer);
        GenerateAllPossiblePlayerMoves(GetOpponentToPlayer(activePlayer));
        if (CheckIfGameIsFinnished())
            EndGame();
        else
            ChangeActiveTeam();
    }

    private bool CheckIfGameIsFinnished()
    {
        Piece[] kingAttackingPieces = activePlayer.GetPiecesAttackingOppositePiece<King>();
        if (kingAttackingPieces.Length > 0)
        {
            ChessPlayer opponentPlayer = GetOpponentToPlayer(activePlayer);
            Piece attackedKing = opponentPlayer.GetPiecesOfType<King>().FirstOrDefault();
            opponentPlayer.RemoveMovesEnablingAttackOnPiece<King>(activePlayer, attackedKing);

            int avaliableKingMoves = attackedKing.avaliableMoves.Count;
            if (avaliableKingMoves == 0)
            {
                bool canCoverKing = opponentPlayer.CanHidePieceFromAttacking<King>(activePlayer);
                if (!canCoverKing)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void SetupCamera(TeamColor team)
    {
        if (cameraSetup == null)
        {
            Debug.LogError("❌ cameraSetup não está definido no ChessGameController!");
            return;
        }

        cameraSetup.SetupCamera(team);
    }


    private void EndGame()
    {
        Debug.Log("Game Finished!");
        uiManager.OnGameFinished(activePlayer.team.ToString());
        SetGameState(GameState.Finished);
    }

    private void ChangeActiveTeam()
    {
        activePlayer = activePlayer == whitePlayer ? blackPlayer : whitePlayer;
    }

    private ChessPlayer GetOpponentToPlayer(ChessPlayer player)
    {
        return player == whitePlayer ? blackPlayer : whitePlayer;
    }

    public void RemoveMovesEnablingAttackOnPieceOfType<T>(Piece piece) where T : Piece
    {
        activePlayer.RemoveMovesEnablingAttackOnPiece<T>(GetOpponentToPlayer(activePlayer), piece);
    }

    public void OnPieceRemoved(Piece piece)
    {
        ChessPlayer pieceOwner = (piece.team == TeamColor.White) ? whitePlayer : blackPlayer;
        pieceOwner.RemovePiece(piece);
        Destroy(piece.gameObject);
    }
}