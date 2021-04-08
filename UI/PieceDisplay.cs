using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class PieceDisplay : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject pieceGameObject;
    [Header("Customizable")]
    [SerializeField] private int side;
    private int pieceCount;
    Vector2 position;

    public void AddPiece(Piece piece)
    {
        GameObject pieceGameObject = Instantiate(this.pieceGameObject, transform);

        position = pieceGameObject.transform.localPosition;
        if(pieceCount < 6)
        {
        position.x += 40 * pieceCount * side;
        }
        else if(pieceCount >= 6  && pieceCount < 12)
        {
            position.x += 40 * (pieceCount - 6) * side;
            position.y -= 40;
        }
        else if(pieceCount >= 12)
        {
            position.x += 40 * (pieceCount - 12) * side;
            position.y -= 80;
        }
        pieceGameObject.transform.localPosition = position;
        Image image = pieceGameObject.GetComponent<Image>();
        image.sprite = Resources.Load<SpriteAtlas>("Pieces").GetSprite(piece.GetType().ToString() + PieceColor.White.ToString());
        pieceCount++;
    }
}
