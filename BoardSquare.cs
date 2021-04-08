using System;
using UnityEngine;
using UnityEngine.U2D;

public class BoardSquare : MonoBehaviour
{
    private Piece piece;
    private Vector2Int square = new Vector2Int();
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer pieceSpriteRenderer;
    private Board board;
    private Color32 squareColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        pieceSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    private void Start() => StartSquareColor();
    private void StartSquareColor()
    {
        if (square.y.Even() && square.x.Even())
            squareColor = board.blackSqaure;
        else if (square.y.Odd() && square.x.Even())
            squareColor = board.whiteSqaure;
        else if (square.x.Odd() && square.y.Odd())
            squareColor = board.blackSqaure;
        else if (square.y.Even() && square.x.Odd())
            squareColor = board.whiteSqaure;
        SetDefaultSquareColor();
    }
    public void SetDefaultSquareColor() => spriteRenderer.color = squareColor;
    public void HighlightSquare() => spriteRenderer.color = squareColor.Blend(board.highLightSqaure, 30);
    public void SetBoard(Board board) => this.board = board;
    public Board GetBoard => board;
    public void SetSquare(int y, int x)
    {
        square.y = y;
        square.x = x;
    }

    public void SetPiece(Piece piece = null)
    {
        this.piece = piece;
        if (piece == null)
            pieceSpriteRenderer.sprite = null;
        else
            pieceSpriteRenderer.sprite = Resources.Load<SpriteAtlas>("Pieces").GetSprite(piece.GetType().ToString() + piece.GetColor());
    }

    public bool Empty => piece == null;

    public void FocusPiece() => piece.Focus();

    public bool EnemyPiece(PieceColor pieceColor)
    {
        if (Empty) return false;
        return piece.color != pieceColor;
    }
    public bool AllyPiece(PieceColor pieceColor)
    {
        if (Empty) return false;
        return piece.color == pieceColor;
    }
    public bool AllyPiece() => AllyPiece(board.PieceMove);
    public string PieceName => piece.GetType().ToString();
    public bool ReFocus => board.PieceMove == piece.color;
    public void HandlePieceMovement() => board.HandlePieceMovement(square.y, square.x);

    public bool Focused => board.PieceFocused();
    public bool PieceFocused => !Empty && piece.Focused;

    public void RemovePiece()
    {
        piece = null;
        pieceSpriteRenderer.sprite = null;
    }
    public Piece GetPiece => piece;
    public float TransformX => transform.position.x;
    public float TransformY => transform.position.y;
}