using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MaterialSetter))]
[RequireComponent(typeof(IObjectTweener))]
public abstract class Piece : MonoBehaviour
{
    [SerializeField] private MaterialSetter materialSetter;
    public Board board { protected get; set; }
    public Vector2Int occupiedSquare { get; set; }
    public TeamColor team { get; set; }
    public bool hasMoved { get; private set; }
    public List<Vector2Int> avaliableMoves;

    private IObjectTweener tweener;

    public abstract List<Vector2Int> SelectAvaliableSquares();

    private void Awake()
    {
        avaliableMoves = new List<Vector2Int>();
        tweener = GetComponent<IObjectTweener>();
        materialSetter = GetComponent<MaterialSetter>();
        hasMoved = false;
    }

    public void SetMaterial(Material selectedMaterial)
    {
        materialSetter.SetSingleMaterial(selectedMaterial);
    }

    public bool IsFromSameTeam(Piece piece)
    {
        return team == piece.team;
    }

    public bool CanMoveTo(Vector2Int coords)
    {
        return avaliableMoves.Contains(coords);
    }

    public virtual void MovePiece(Vector2Int coords)
    {
        Debug.Log("MovePiece chamado para: " + name + " para " + coords);

        Vector3 targetPosition = board.CalculatePositionFromCoords(coords);
        occupiedSquare = coords;
        hasMoved = true;
        tweener.MoveTo(transform, targetPosition);

        FindObjectOfType<AudioInputHandler>()?.PlayMoveSound();
    }




    protected void TryToAddMove(Vector2Int coords)
    {
        avaliableMoves.Add(coords);
    }

    public void SetData(Vector2Int coords, TeamColor team, Board board)
    {
        this.team = team;
        occupiedSquare = coords;
        this.board = board;
        transform.position = board.CalculatePositionFromCoords(coords);
    }

    public bool IsAttackingPiece<T>() where T : Piece
    {
        foreach (var square in avaliableMoves)
        {
           if(board.GetPieceOnSquare(square) is T)
                return true;
        }
        return false;
    }

    protected Piece GetPieceInDirection<T>(TeamColor team, Vector2Int direction) where T : Piece
    {
        for (int i = 1; i <= Board.BOARD_SIZE; i++)
        {
            Vector2Int nextCoords = occupiedSquare + direction * i;
            Piece piece = board.GetPieceOnSquare(nextCoords);
            if(!board.CheckIfCoordinatesAreOnBoard(nextCoords))
                return null;
            if (piece != null){
                if(piece.team != team || !(piece is T))
                    return null;
                else if(piece.team == team && piece is T)
                    return piece;
            }
        }
        return null;
    }

}