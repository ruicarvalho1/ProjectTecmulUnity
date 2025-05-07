using System;
using System.Collections.Generic;
using UnityEngine;

public class PieceCreator : MonoBehaviour
{
    [SerializeField] private GameObject[] piecesPrefabs;
    [SerializeField] private Material blackMaterial;
    [SerializeField] private Material whiteMaterial;

    private Dictionary<string, GameObject> nameToPieceDict = new Dictionary<string, GameObject>();

    private void Awake()
    {
        foreach (var piece in piecesPrefabs)
        {
            string typeName = piece.GetComponent<Piece>().GetType().ToString();

            if (!nameToPieceDict.ContainsKey(typeName))
            {
                nameToPieceDict.Add(typeName, piece);
            }
            else
            {
                Debug.LogWarning($"Duplicate piece type: {typeName}");
            }
        }
    }

    public GameObject CreatePiece(Type type)
    {
        if (!nameToPieceDict.TryGetValue(type.ToString(), out GameObject prefab))
        {
            Debug.LogError($"No prefab found for type: {type}");
            return null;
        }

        GameObject newPiece = Instantiate(prefab);
        return newPiece;
    }

    public Material GetTeamMaterial(TeamColor team)
    {
        return team == TeamColor.White ? whiteMaterial : blackMaterial;
    }
}