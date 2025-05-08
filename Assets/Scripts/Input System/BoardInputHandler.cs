using System;
using UnityEngine;
using Photon.Pun;

public class BoardInputHandler : MonoBehaviour, IInputHandler
{
    private Board board;

    private void Awake()
    {
        board = GetComponent<Board>();
    }

    public void ProcessInput(Vector3 inputPosition, GameObject selectedObject, Action onClick)
    {
        if (board == null)
        {
           
            return;
        }
        
        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            return;
        }

        board.OnSquareSelected(inputPosition);
    }
}