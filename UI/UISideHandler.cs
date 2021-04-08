using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISideHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PieceDisplay pieceDisplayWhite;
    [SerializeField] private PieceDisplay pieceDisplayBlack;
    [SerializeField] private Text pointsWhite, pointsBlack, leadWhite, leadBlack;
    private int pointsW, pointsB;
    public void AddPiece(Piece piece)
    {
        if (piece.color == PieceColor.White)
            pieceDisplayBlack.AddPiece(piece);
        else
            pieceDisplayWhite.AddPiece(piece);
        SetPoints(piece);
    }
    private void SetPoints(Piece piece)
    {
        if (piece.color == PieceColor.White)
        {
            pointsB += piece.points;
            pointsBlack.text = $"Score - {pointsB}";
        }
        else
        {
            pointsW += piece.points;
            pointsWhite.text = $"Score - {pointsW}";
        }
        leadBlack.text = $"{pointsB - pointsW}";
        leadWhite.text = $"{pointsW - pointsB}";
    }
}
