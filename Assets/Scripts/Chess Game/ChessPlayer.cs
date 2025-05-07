using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class ChessPlayer
{
    public TeamColor team { get; set; }
    public Board board { get; set; }
    public List<Piece> activePieces { get; private set; }

    public ChessPlayer(TeamColor team, Board board)
    {
        activePieces = new List<Piece>();
        this.board = board;
        this.team = team;
    }

    public void AddPiece(Piece piece)
    {
        if (!activePieces.Contains(piece))
            activePieces.Add(piece);
    }

    public void RemovePiece(Piece piece)
    {
        if (activePieces.Contains(piece))
            activePieces.Remove(piece);
    }

    public void GenerateAllPossibleMoves()
    {
        foreach (var piece in activePieces)
        {
            if(board.HasPiece(piece))
                piece.SelectAvaliableSquares();
        }
    }

    public Piece[] GetPiecesAttackingOppositePiece<T>() where T : Piece
    {
        return activePieces
            .Where(piece => piece.IsAttackingPiece<T>())
            .ToArray();
    }

    public Piece[] GetPiecesOfType<T>()  where T : Piece
    {
        return activePieces.Where(piece => piece is T)
            .ToArray();
    }

    public void RemoveMovesEnablingAttackOnPiece<T>(ChessPlayer opponent, Piece selectedPiece) where T : Piece
    {
        List<Vector2Int> coordsToRemove = new List<Vector2Int>(); 
        foreach (var coords in selectedPiece.avaliableMoves)
        {
            Piece pieceOnSquare = board.GetPieceOnSquare(coords);
            board.UpdateBoardOnPieceMove(coords, selectedPiece.occupiedSquare, selectedPiece, null);
            opponent.GenerateAllPossibleMoves();
            if(opponent.CheckIfIsAttackingPiece<T>())
            {
                coordsToRemove.Add(coords);
            }
            board.UpdateBoardOnPieceMove(selectedPiece.occupiedSquare, coords, selectedPiece, pieceOnSquare); 
        }
        foreach (var coords in coordsToRemove)
        {
            selectedPiece.avaliableMoves.Remove(coords);
        }
    }

    private bool CheckIfIsAttackingPiece<T>() where T : Piece
    {
        foreach (var piece in activePieces)
        {
            if(board.HasPiece(piece) && piece.IsAttackingPiece<T>())
                return true;
        }
        return false;
    }

    public bool CanHidePieceFromAttacking<T>(ChessPlayer activePlayer) where T : Piece
    {
        foreach (var piece in activePieces)
        {
            foreach (var coords in piece.avaliableMoves)
            {
                Piece pieceOnCoords = board.GetPieceOnSquare(coords);
                board.UpdateBoardOnPieceMove(coords, piece.occupiedSquare, piece, null);
                activePlayer.GenerateAllPossibleMoves();
                if(!activePlayer.CheckIfIsAttackingPiece<T>()){
                    board.UpdateBoardOnPieceMove(piece.occupiedSquare, coords, piece, pieceOnCoords);
                    return true;
                }
                board.UpdateBoardOnPieceMove(piece.occupiedSquare, coords, piece, pieceOnCoords);
            }
        }
        return false;
    }

    public void OnGameRestarted()
    {
        activePieces.Clear();
    }
}