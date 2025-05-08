using System;
using UnityEngine;

public class BoardInputHandler : MonoBehaviour, IInputHandler
{
    private SingleplayerBoard board;

    private void Awake()
    {
        board = GetComponent<SingleplayerBoard>();
        
    }

    public void ProcessInput(Vector3 inputPosition, GameObject selectedObject, Action onClick)
    {
        if (board != null)
            board.OnSquareSelected(inputPosition);
    }
}