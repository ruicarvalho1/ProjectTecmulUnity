using UnityEngine;

public class SingleplayerBoard : Board
{
    public override void SetSelectedPiece(Vector2 coords)
    {
        Vector2Int intCoords = new Vector2Int(Mathf.RoundToInt(coords.x), Mathf.RoundToInt(coords.y));
        Debug.Log($"[SetSelectedPiece] coords: {coords}, intCoords: {intCoords}");
        OnSetSelectedPiece(intCoords); 
    }

    public override void SelectedPieceMoved(Vector2 coords)
    {
        Vector2Int intCoords = new Vector2Int(Mathf.RoundToInt(coords.x), Mathf.RoundToInt(coords.y));
        Debug.Log($"[SelectedPieceMoved] coords: {coords}, intCoords: {intCoords}");
        OnSelectedPieceMoved(intCoords);
    }


}
