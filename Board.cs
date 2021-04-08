using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject squarePrefab;
    [SerializeField] private GameObject pieceContainerPrefab;
    [SerializeField] private UISideHandler uISideHandler;
    [SerializeField] private GameEnd gameEnd;
    [SerializeField] private InvalidMoveAnim invalidMoveAnim;

    public System.Action<string> CheckMate, StaleMate;
    public System.Action<Piece> AddPieceUI;
    
    public System.Action<int> InvalidMoveStart, invalidMoveEnd;
    public bool InvalidAnimating => invalidMoveAnim.Animating;

    private System.Action<PieceColor> FlipCamera;
    private bool autoRotate;



    private BoardSquare[,] boardSquares = new BoardSquare[8, 8];
    private PieceContainer[] pieceContainers = new PieceContainer[4];

    public PieceColor PieceMove { get; set; }
    private Piece[,] pieces = new Piece[4, 8];
    private Vector2Int focusedPiece = new Vector2Int(-1, -1);
    private List<GameObject> availableMoves = new List<GameObject>();
    
    private bool[] RookAbleToCastle = { true, true, true, true };
    private bool[] KingAbleToCastle = { true, true };

    private Vector2Int inPassingPiece = new Vector2Int(-1, -1);
    private Vector2Int inPassingPieceOld = new Vector2Int(-1, -1);
    private bool inPasingWait = false;
    private (bool ready, bool awaiting, Vector2Int position,int pawnY, int pawnX) pawnUpgrade;

    public readonly Color32 whiteSqaure = new Color32(220, 178, 129, 255);
    public readonly Color32 blackSqaure = new Color32(41, 61, 88, 255);
    public readonly Color32 highLightSqaure = new Color32(192, 192, 192, 76);

    private (int boardY, int boardX)?[] highlightedSquares = { null, null };

    private void Start()
    {
        CheckMate = gameEnd.CheckMate;
        StaleMate = gameEnd.StaleMate;
        
        FlipCamera = Camera.main.GetComponent<CameraFlip>().AutoFlip;
        autoRotate = PlayerPrefs.GetInt("AutoRotation") == 1;

        AddPieceUI = uISideHandler.AddPiece;

        InvalidMoveStart = invalidMoveAnim.StartAnim;
        invalidMoveEnd = invalidMoveAnim.StopAnim;

        PieceMove = PieceColor.White;
        SetPieces();
        SetBoard();
    }
    private void SetPieces()
    {
        SetPawns();
        SetRooks();
        SetHorses();
        SetBishops();
        SetQueens();
        SetKings();
    }

    private void SetPawns()
    {
        for (int x = 0; x < 8; x++)
        {
            pieces[1, x] = new Pawn(this, PieceColor.White);
            pieces[1, x].SetPosition(2, x + 1);
            pieces[2, x] = new Pawn(this, PieceColor.Black);
            pieces[2, x].SetPosition(7, x + 1);
        }
    }

    private void SetRooks()
    {
        pieces[0, 0] = new Rook(this, PieceColor.White);
        pieces[0, 0].SetPosition(1, 1);
        pieces[0, 7] = new Rook(this, PieceColor.White);
        pieces[0, 7].SetPosition(1, 8);
        pieces[3, 0] = new Rook(this, PieceColor.Black);
        pieces[3, 0].SetPosition(8, 1);
        pieces[3, 7] = new Rook(this, PieceColor.Black);
        pieces[3, 7].SetPosition(8, 8);
    }

    private void SetHorses()
    {
        pieces[0, 1] = new Horse(this, PieceColor.White);
        pieces[0, 1].SetPosition(1, 2);
        pieces[0, 6] = new Horse(this, PieceColor.White);
        pieces[0, 6].SetPosition(1, 7);
        pieces[3, 1] = new Horse(this, PieceColor.Black);
        pieces[3, 1].SetPosition(8, 2);
        pieces[3, 6] = new Horse(this, PieceColor.Black);
        pieces[3, 6].SetPosition(8, 7);
    }

    private void SetBishops()
    {
        pieces[0, 2] = new Bishop(this, PieceColor.White);
        pieces[0, 2].SetPosition(1, 3);
        pieces[0, 5] = new Bishop(this, PieceColor.White);
        pieces[0, 5].SetPosition(1, 6);
        pieces[3, 2] = new Bishop(this, PieceColor.Black);
        pieces[3, 2].SetPosition(8, 3);
        pieces[3, 5] = new Bishop(this, PieceColor.Black);
        pieces[3, 5].SetPosition(8, 6);
    }

    private void SetQueens()
    {
        pieces[0, 3] = new Queen(this, PieceColor.White);
        pieces[0, 3].SetPosition(1, 4);
        pieces[3, 3] = new Queen(this, PieceColor.Black);
        pieces[3, 3].SetPosition(8, 4);
    }

    private void SetKings()
    {
        pieces[0, 4] = new King(this, PieceColor.White);
        pieces[0, 4].SetPosition(1, 5);
        pieces[3, 4] = new King(this, PieceColor.Black);
        pieces[3, 4].SetPosition(8, 5);
    }
    private void SetBoard()
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                boardSquares[y, x] = Instantiate(squarePrefab, new Vector3(x - 4, y - 4, 0), Quaternion.identity).GetComponent<BoardSquare>();
                boardSquares[y, x].SetSquare(y + 1, x + 1);
                boardSquares[y, x].SetBoard(this);
                switch (y)
                {
                    case 0:
                        boardSquares[y, x].SetPiece(pieces[y, x]);
                        break;

                    case 1:
                        boardSquares[y, x].SetPiece(pieces[y, x]);
                        break;

                    case 6:
                        boardSquares[y, x].SetPiece(pieces[2, x]);
                        break;

                    case 7:
                        boardSquares[y, x].SetPiece(pieces[3, x]);
                        break;

                    default:
                        break;
                }
            }
        }
    }

    public void UnSetAvailableMoves()
    {
        for (int i = 0; i < availableMoves.Count; i++)
            availableMoves[i].SetActive(false);
        availableMoves = new List<GameObject>();
    }

    public void SetAvailableMove(int y, int x)
    {
        if (GetSquareByPiecePosition(y, x) == null) return;

        availableMoves.Add(GetSquareByPiecePosition(y, x).transform.GetChild(1).gameObject);
        if (availableMoves.Count > 0)
            availableMoves[availableMoves.Count - 1].SetActive(true);
            
    }

    public BoardSquare GetSquareByPiecePosition(int y, int x)
    {
        if (y.InRange(1, boardSquares.GetLength(0)) && x.InRange(1, boardSquares.GetLength(1)))
            return boardSquares[y - 1, x - 1];
        else
            return null;
    }

    public bool EmptySpace(int y, int x)
    {
        if (y.InRange(0, boardSquares.GetLength(0)-1) && x.InRange(0, boardSquares.GetLength(1)-1))
            return boardSquares[y, x].Empty;
        return false;
    }

    public void FocusPiece(int y, int x)
    {
        if (focusedPiece.x != x - 1 || focusedPiece.y != y - 1)
            UnSetAvailableMoves();
        focusedPiece.y = y - 1;
        focusedPiece.x = x - 1;
    }

    public bool PieceFocused() => focusedPiece != new Vector2Int(-1, -1);
    public Piece GetPieceFocused => pieces[focusedPiece.y-1, focusedPiece.x-1];
    private void DeFocus()
    {
        UnSetAvailableMoves();
        focusedPiece.y = -1;
        focusedPiece.x = -1;
    }

    public bool PieceInFocus(int y, int x) => focusedPiece.y == y && focusedPiece.x == x;

    public void DestoryPiece(int y, int x) => boardSquares[y - 1, x - 1].RemovePiece();
    public void MovePiece(int y, int x)
    {
        boardSquares[y - 1, x - 1].SetPiece(boardSquares[focusedPiece.y, focusedPiece.x].GetPiece);
        boardSquares[focusedPiece.y, focusedPiece.x].RemovePiece();
        DeFocus();
    }
    public void MoveOtherPiece(int y, int x, Piece piece)
    {
        boardSquares[y - 1, x - 1].SetPiece(piece);
        boardSquares[piece.Y -1, piece.X -1].RemovePiece();
        piece.SetPosition(y, x);
    }
    public void HandlePieceMovement(int y, int x)
    {
        if (boardSquares[focusedPiece.y, focusedPiece.x].GetPiece.TryAttack(y, x))
            MoveEnd();
        else if (boardSquares[focusedPiece.y, focusedPiece.x].GetPiece.TryMove(y, x))
            MoveEnd();
        else
            InvalidMoveStart.Invoke(PieceMove == PieceColor.White ? -1 : 1);
    }
    public void MoveEnd()
    {
        if (PawnUpgrading)
        {
            if (!AwaitingPawnUpgrade)
            {
                pawnUpgrade.awaiting.SetTrue();
                StartUpgradingPawn();
            }
            return;
        }
        if (InPassing && !inPasingWait)
        {
            inPassingPieceOld = inPassingPiece;
            inPasingWait.SetTrue();
        }
        else if (inPassingPieceOld != inPassingPiece)
            inPassingPieceOld = inPassingPiece;
        else
            DeSetInPassing();
        ChangeTurn();
    }
    public bool OutOfBoard(int y, int x) => !(y.InRange(0, boardSquares.GetLength(0) - 1) && x.InRange(0, boardSquares.GetLength(1) - 1));
    public Vector2Int GetKingPosition(PieceColor color)
    {
        if (color == PieceColor.White)
            return new Vector2Int(pieces[0, 4].X, pieces[0, 4].Y);
        else
            return new Vector2Int(pieces[3, 4].X, pieces[3, 4].Y);
    }
    public void DisableCastlingRook(PieceColor color, int side)
    {
        if(color == PieceColor.White)
            RookAbleToCastle[side < 0 ? 0 : 1].SetFalse();
        else
            RookAbleToCastle[side < 0 ? 2 : 3].SetFalse();
    }
    public bool AbletToCastleRook(PieceColor color, int side)
    {
        if (color == PieceColor.White)
            return RookAbleToCastle[side < 0 ? 0 : 1];
        return RookAbleToCastle[side < 0 ? 2 : 3];
    }
    public void DisableCastlingKing(PieceColor color)
    {
        DisableCastlingRook(color, 1);
        DisableCastlingRook(color, -1);
        KingAbleToCastle[(int)color].SetFalse();
    }
    public void CastleRook(PieceColor color, int side)
    {
        if (color == PieceColor.White)
        {
            if(side < 0)
                MoveOtherPiece(1, 4, pieces[0, 0]);
            else
                MoveOtherPiece(1, 6, pieces[0, 7]);
        }
        else
        {
            if (side < 0)
                MoveOtherPiece(8, 4, pieces[3, 0]);
            else
                MoveOtherPiece(8, 6, pieces[3, 7]);
        }
    }
    public bool AbleToCastle(PieceColor color, int side) => AbletToCastleRook(color, side) && KingAbleToCastle[(int)color];

    public bool InPassing => inPassingPiece.x > -1 || inPassingPiece.y > -1;
    public void DeSetInPassing()
    {
        inPasingWait.SetFalse();
        inPassingPiece = new Vector2Int(-1, -1);
    }
    public void SetInPassing(int y, int x) => inPassingPiece = new Vector2Int(x, y);
    public Vector2Int GetInPassing => inPassingPiece;
    public void ChangeTurn()
    {
        if (InvalidAnimating) invalidMoveEnd(PieceMove == PieceColor.White ? -1 : 1);

        if(autoRotate)
            FlipCamera.Invoke(PieceMove);
        PieceMove = PieceMove == PieceColor.White ? PieceColor.Black : PieceColor.White;
    }
    public bool NoMoves(PieceColor color)
    {
        Piece pieceSpace;
        for(int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (boardSquares[y, x].Empty) continue;

                pieceSpace = boardSquares[y, x].GetPiece;
                if (pieceSpace.color == color)
                {
                    if (!pieceSpace.NoMoves())
                    {
                        //Debug.Log($"Move Available by piece:{pieceSpace.GetType()}, piece position x:{pieceSpace.X}, y:{pieceSpace.Y}");
                        return false;
                    }
                }
            }
        }
        return true;
    }


    public void SetChangingPiece(int y, int x)
    {
        pawnUpgrade.ready.SetTrue();
        pawnUpgrade.position.y = y;
        pawnUpgrade.position.x = x;
    }
    public void TrackPawnIndex()
    {
        for (int x = 0; x < 8; x++)
        {
            if (pieces[1, x].X == pawnUpgrade.position.x && pieces[1, x].Y == pawnUpgrade.position.y)
            {
                pawnUpgrade.pawnX = x;
                pawnUpgrade.pawnY = 1;
                break;
            }
            if (pieces[2, x].X == pawnUpgrade.position.x && pieces[2, x].Y == pawnUpgrade.position.y) {
                pawnUpgrade.pawnX = x;
                pawnUpgrade.pawnY = 2;
                break;
            }
        }
    }
    public bool PawnUpgrading => pawnUpgrade.ready;
    public bool AwaitingPawnUpgrade => pawnUpgrade.awaiting;
    public void UpgradePawn(Piece piece)
    {
        pieces[pawnUpgrade.pawnY, pawnUpgrade.pawnX] = piece;
        pieces[pawnUpgrade.pawnY, pawnUpgrade.pawnX].SetPosition(pawnUpgrade.position.y, pawnUpgrade.position.x);
        GetSquareByPiecePosition(pawnUpgrade.position.y, pawnUpgrade.position.x).SetPiece(piece);
        EndUpgradingPawn();
    }

    public void StartUpgradingPawn()
    {
        TrackPawnIndex();
        pawnUpgrade.awaiting.SetTrue();
        for(int i = 0; i < 4; i++)
        {
            pieceContainers[i] = Instantiate(
                pieceContainerPrefab,
                new Vector3(
                    GetSquareByPiecePosition(pawnUpgrade.position.y, pawnUpgrade.position.x).TransformX + (i - 2) + 0.5f,
                    GetSquareByPiecePosition(pawnUpgrade.position.y, pawnUpgrade.position.x).TransformY
                    ),
                Quaternion.identity
                )
                .GetComponent<PieceContainer>();
            pieceContainers[i].transform.parent = transform;
        }
        pieceContainers[0].Initialize(new Queen(this, pieces[pawnUpgrade.pawnY, pawnUpgrade.pawnX].color));
        pieceContainers[1].Initialize(new Rook(this, pieces[pawnUpgrade.pawnY, pawnUpgrade.pawnX].color));
        pieceContainers[2].Initialize(new Bishop(this, pieces[pawnUpgrade.pawnY, pawnUpgrade.pawnX].color));
        pieceContainers[3].Initialize(new Horse(this, pieces[pawnUpgrade.pawnY, pawnUpgrade.pawnX].color));
    }
    private void EndUpgradingPawn()
    {
        pawnUpgrade.ready.SetFalse();
        pawnUpgrade.awaiting.SetFalse();
        pawnUpgrade.position.y = -1;
        pawnUpgrade.position.x = -1;
        for (int i = 0; i < 4; i++)
            Destroy(pieceContainers[i].gameObject);
        MoveEnd();
    }

    public void HighlightSquares(int fromY, int fromX, int toY, int toX)
    {
        fromY--;
        fromX--;
        toY--;
        toX--;

        for (int i = 0; i < highlightedSquares.Length; i++)
        {
            if (highlightedSquares[i].HasValue)
                boardSquares[highlightedSquares[i].Value.boardY, highlightedSquares[i].Value.boardX].SetDefaultSquareColor();
        }
        highlightedSquares[0] = (fromY, fromX);
        highlightedSquares[1] = (toY, toX);
        for (int i = 0; i < highlightedSquares.Length; i++) 
            boardSquares[highlightedSquares[i].Value.boardY, highlightedSquares[i].Value.boardX].HighlightSquare();
    }
}