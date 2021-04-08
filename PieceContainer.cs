using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PieceContainer : MonoBehaviour
{
    public Piece Piece { get; private set; }
    private SpriteRenderer pieceSpriteRenderer;
    private void Awake() => pieceSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    public void Initialize(Piece piece)
    {
        Piece = piece;
        if (piece == null)
            pieceSpriteRenderer.sprite = null;
        else
            pieceSpriteRenderer.sprite = Resources.Load<SpriteAtlas>("Pieces").GetSprite(piece.GetType().ToString() + piece.GetColor());
    }
}
