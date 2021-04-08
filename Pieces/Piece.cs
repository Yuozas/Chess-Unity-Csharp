using System;
using UnityEngine;

public abstract class Piece
{
    protected Vector2Int position;
    public readonly PieceColor color;
    protected readonly Board board;
    protected bool inPassed;
    protected bool tryOut;
    public int points;
    public Piece(Board board, PieceColor color)
    {
        this.board = board;
        this.color = color;
        tryOut.SetFalse();
    }

    protected abstract bool LegalMove(int y, int x);

    protected abstract bool AttackAvailable(int y, int x);

    protected abstract void ShowAvailableMoves();

    public abstract bool NoMoves();
    public bool TryMove(int y, int x)
    {
        if (!board.EmptySpace(y - 1, x - 1)) return false;
        if (KingUnderAttackIfMove(y, x)) return false;
        if (LegalMove(y, x))
        {
            board.HighlightSquares(Y, X, y, x);
            Move(y, x);
            return true;
        }
        else
            return false;
    }

    public bool TryAttack(int y, int x)
    {
        //Debug.Log("1AT");
        if (board.EmptySpace(y - 1, x - 1))
        {
            //Debug.Log("1BAT");
            if (!(InPassing && PieceName(Y,X) == "Pawn"))
                return false;
        }
        //Debug.Log("2AT");
        if (KingUnderAttackIfMove(y, x)) return false;
        //Debug.Log("3AT");
        if (AttackAvailable(y, x))
        {
            //Debug.Log("AT AVAILABLE");
            if (inPassed)
            {
                inPassed.SetFalse();
                board.HighlightSquares(Y, X, y, x);
                board.AddPieceUI.Invoke(board.GetSquareByPiecePosition(InPassingPiece.y, InPassingPiece.x).GetPiece);
                Attack(InPassingPiece.y, InPassingPiece.x);
                Move(y, x);
                return true;
            }
            board.AddPieceUI.Invoke(board.GetSquareByPiecePosition(y, x).GetPiece);
            board.HighlightSquares(Y, X, y, x);
            Attack(y, x);
            Move(y, x);
            return true;
        }
        else
            return false;
    }
    private bool TryMoveLegal(int y, int x)
    {
        if (!board.EmptySpace(y - 1, x - 1)) return false;
        if (KingUnderAttackIfMove(y, x)) return false;
        tryOut.SetTrue();
        if (LegalMove(y, x))
        {
            tryOut.SetFalse();
            return true;
        }
        tryOut.SetFalse();
        return false;
    }

    private bool TryAttackLegal(int y, int x)
    {
        if (board.EmptySpace(y - 1, x - 1))
        {
            if (!(InPassing && PieceName(Y, X) == "Pawn"))
                return false;
        }
        if (KingUnderAttackIfMove(y, x)) return false;
        tryOut.SetTrue();
        if (AttackAvailable(y, x))
        {
            tryOut.SetFalse();
            return true;
        }
        tryOut.SetFalse();
        return false;
    }
    protected bool MoveLegal(int y, int x)
    {
        if (TryAttackLegal(y, x)) return true;
        if (TryMoveLegal(y, x)) return true;
        return false;
    }
    protected void DisableCastlingRook()
    {
        if(X > 4)
            board.DisableCastlingRook(color, 1);
        else
            board.DisableCastlingRook(color, -1);
    }
    protected void DisableCastlingKing() => board.DisableCastlingKing(color);
    protected void Attack(int y, int x)
    {
        board.DestoryPiece(y, x);
    }
    protected void CastleRook(int side) => board.CastleRook(color, side);
    protected void Move(int y, int x)
    {
        board.MovePiece(y, x);
        SetPosition(y, x);
        if (KingUnderAttack(color.Flip()))
        {
            if (board.NoMoves(color.Flip()))
            {
                board.CheckMate.Invoke(color.ToString());
                return;
            }
        }
        if (!EnemyKingCanMove())
        {
            if (board.NoMoves(color.Flip()))
            {
                board.StaleMate.Invoke(color.ToString());
                return;
            }
        }
    }

    public void SetPosition(int y, int x)
    {
        position.y = y;
        position.x = x;
    }
    protected void SetInPassing(int y, int x) => board.SetInPassing(y, x);
    protected bool InPassing => board.InPassing;
    protected Vector2Int InPassingPiece => board.GetInPassing;
    public int X => position.x;
    public int Y => position.y;
    protected bool EmptySpot(int y, int x) => board.EmptySpace(y - 1, x - 1);
    protected void SetAvailableMove(int y, int x) => board.SetAvailableMove(y, x);
    protected void SetAvailableMoveIfNoAlliesNotUnderAttack(int y, int x)
    {
        if (!AllySquare(y, x) && !KingUnderAttackIfMove(y, x))
            SetAvailableMove(y, x);
    }
    protected void SetAvailableMoveIfNoEnemies(int y, int x)
    {
        if (!EnemySquare(y, x))
            SetAvailableMove(y, x);
    }
    protected void SetAvailableMoveIfEmpty(int y, int x)
    {
        if (EmptySpot(y, x))
            SetAvailableMove(y, x);
    }
    protected bool EnemySquare(int y, int x) => board.GetSquareByPiecePosition(y, x) != null && board.GetSquareByPiecePosition(y, x).EnemyPiece(color);
    protected bool EnemySquare(int y, int x, PieceColor color) => board.GetSquareByPiecePosition(y, x) != null && board.GetSquareByPiecePosition(y, x).EnemyPiece(color);
    protected string PieceName(int y, int x) => board.GetSquareByPiecePosition(y, x).PieceName;
    protected bool AllySquare(int y, int x) => board.GetSquareByPiecePosition(y, x) != null && board.GetSquareByPiecePosition(y, x).AllyPiece(color);
    protected bool AllySquare(int y, int x, PieceColor color) => board.GetSquareByPiecePosition(y, x) != null && board.GetSquareByPiecePosition(y, x).AllyPiece(color);

    public void Focus()
    {
        board.FocusPiece(Y, X);
        ShowAvailableMoves();
    }
    protected bool PieceOutOfBoard(int y, int x) => board.OutOfBoard(y - 1, x - 1);
    public bool Focused => board.PieceInFocus(Y, X);
    public string GetColor() => Enum.GetName(typeof(PieceColor), color);

    protected bool KingUnderAttack(PieceColor? color)
    {
        if (!color.HasValue) color = this.color;
        Vector2Int kingPosition = board.GetKingPosition(color.GetValueOrDefault());
        //Debug.Log("KingUnderAttack CHECK");
        return UnderAttack(kingPosition.y, kingPosition.x, color: color.GetValueOrDefault());
    }
    protected bool EnemyKingCanMove()
    {
        PieceColor pieceColor = color.Flip();
        Vector2Int kingPosition = board.GetKingPosition(pieceColor);
        if (CheckAvailableMove(kingPosition.y + 1, kingPosition.x, pieceColor)) return true;
        if (CheckAvailableMove(kingPosition.y + 1, kingPosition.x + 1, pieceColor)) return true;
        if (CheckAvailableMove(kingPosition.y, kingPosition.x +1, pieceColor)) return true;
        if (CheckAvailableMove(kingPosition.y - 1, kingPosition.x + 1, pieceColor)) return true;
        if (CheckAvailableMove(kingPosition.y - 1, kingPosition.x, pieceColor)) return true;
        if (CheckAvailableMove(kingPosition.y - 1, kingPosition.x - 1, pieceColor)) return true;
        if (CheckAvailableMove(kingPosition.y, kingPosition.x -1, pieceColor)) return true;
        if (CheckAvailableMove(kingPosition.y + 1, kingPosition.x - 1, pieceColor)) return true;
        return false;
    }
    protected bool CheckAvailableMove(int y, int x, PieceColor color)
    {
        if (PieceOutOfBoard(y, x)) return false;
        if(EmptySpot(y, x))
        {
            if (UnderAttack(y, x, color: color))
                return false;
            return true;
        }
        if(EnemySquare(y, x, color))
        {
            if (UnderAttack(y, x, color: color))
                return false;
            return true;
        }
        return false;
    }
    protected bool KingUnderAttackIfMove(int y, int x, bool ignorePassing = false)
    {
        Vector2Int kingPosition = board.GetKingPosition(color);
        //Debug.Log("Self KingUnderAttackIfMove Checking piece:" + PieceName(Y, X));
        if (kingPosition.y == Y && kingPosition.x == X)
            return UnderAttack(y, x, true, Y, X, ignorePassing: ignorePassing, kingIgnore: true);
        else
            return UnderAttack(kingPosition.y, kingPosition.x, true, y, x, ignorePassing: ignorePassing);
    }
    protected bool UnderAttack(int y, int x, bool ignore = false, int desiredY = 0, int desiredX = 0, PieceColor? color = null, bool ignorePassing = false, bool kingIgnore = false)
    {
        if (!color.HasValue)
            color = this.color;
        if (UnderAttackDiagonal(y, x, ignore, desiredY, desiredX, color.GetValueOrDefault(), ignorePassing, kingIgnore))
        {
            //Debug.Log(1);
            //Debug.Log($"y:{y}, x:{x}, ignore:{ignore}, desiredY:{desiredY}, desiredX:{desiredX}, color.GetValueOrDefault():{color.GetValueOrDefault()}, ignorePassing:{ignorePassing}");
            return true;
        }
        if (UnderAttackHorizontal(y, x, ignore, desiredY, desiredX, color.GetValueOrDefault(), ignorePassing, kingIgnore))
        {
            //Debug.Log(2);
            //Debug.Log($"y:{y}, x:{x}, ignore:{ignore}, desiredY:{desiredY}, desiredX:{desiredX}, color.GetValueOrDefault():{color.GetValueOrDefault()}, ignorePassing:{ignorePassing}");
            return true;
        }
        if (UnderAttackVertical(y, x, ignore, desiredY, desiredX, color.GetValueOrDefault(), ignorePassing, kingIgnore))
        {
            //Debug.Log(3);
            //Debug.Log($"y:{y}, x:{x}, ignore:{ignore}, desiredY:{desiredY}, desiredX:{desiredX}, color.GetValueOrDefault():{color.GetValueOrDefault()}, ignorePassing:{ignorePassing}");
            return true;
        }
        if (UnderAttackHorse(y, x, desiredY, desiredX, color.GetValueOrDefault()))
        {
            //Debug.Log(4);

            //Debug.Log($"y:{y}, x:{x}, color.GetValueOrDefault():{color.GetValueOrDefault()}");
            return true;
        }
        return false;
    }
    private bool UnderAttackDiagonal(int targetY, int targetX, int directionY, int directionX, bool ignore, int desiredY, int desiredX, PieceColor color, bool ignorePassing, bool kingIgnore)
    {
        int i = 0;
        int tX, tY;
        while (true)
        {

            i++;
            tY = targetY + i * directionY;
            tX = targetX + i * directionX;


            if (PieceOutOfBoard(tY, tX)) return false;
            if (ignore)
            {
                if (tY == desiredY && tX == desiredX)
                {
                    if (kingIgnore) continue;
                    else return false;
                }
                if (tY == Y && tX == X) continue;
                if (ignorePassing && InPassingPiece.y == tY && InPassingPiece.x == tX) continue;
            }
            if (EmptySpot(tY, tX)) continue;
            if (AllySquare(tY, tX, color))
                return false;
            if (EnemySquare(tY, tX, color))
            {
                /*
                if(i == 1 && (PieceName(tY, tX) == "King" || PieceName(tY, tX) == "Pawn") || !(PieceName(tY, tX) == "Horse" || PieceName(tY, tX) == "Rook"))
                    Debug.Log($"PieceName(tY, tX):{PieceName(tY, tX)}, piece location y:{tY}, x:{tX}, targetY:{targetY}, i * directionY:{i * directionY}, targetX:{targetX}, i * directionY:{i * directionX}");
                */
                if(PieceName(tY, tX) == "King")
                    return i == 1;
                if (PieceName(tY, tX) == "Pawn")
                {
                    //Debug.Log($"ty < targetY :{tY < targetY}, ty > targetY :{tY > targetY}, tY:{tY},targetY:{targetY}, color.Flip():{color.Flip()}");
                    if(tY < targetY && color.Flip() == PieceColor.White)
                        return i == 1;
                    if (tY > targetY && color.Flip() == PieceColor.Black)
                        return i == 1;
                    return false;
                }

                return !(PieceName(tY, tX) == "Horse" || PieceName(tY, tX) == "Rook");
            }
        }
    }
    private bool UnderAttackDiagonal(int targetY, int targetX, bool ignore, int desiredY, int desiredX, PieceColor color, bool ignorePassing, bool kingIgnore)
    {
        if (UnderAttackDiagonal(targetY, targetX, 1, -1, ignore, desiredY, desiredX, color, ignorePassing, kingIgnore))
            return true;
        if (UnderAttackDiagonal(targetY, targetX, 1, 1, ignore, desiredY, desiredX, color, ignorePassing, kingIgnore))
            return true;
        if (UnderAttackDiagonal(targetY, targetX, -1, -1, ignore, desiredY, desiredX, color, ignorePassing, kingIgnore))
            return true;
        if (UnderAttackDiagonal(targetY, targetX, -1, 1, ignore, desiredY, desiredX, color, ignorePassing, kingIgnore))
            return true;
        return false;

    }
    private bool UnderAttackHorizontal(int targetY, int targetX, int directionX, bool ignore, int desiredY, int desiredX, PieceColor color, bool ignorePassing, bool kingIgnore)
    {
        int i = 0;
        int tX;
        while (true)
        {
            i++;
            tX = targetX + i * directionX;

            if (PieceOutOfBoard(targetY, tX)) return false;

            if (ignore)
            {
                if (targetY == desiredY && tX == desiredX)
                {
                    if (kingIgnore) continue;
                    else return false;
                }
                if (targetY == Y && tX == X) continue;
                if (ignorePassing && InPassingPiece.y == targetY && InPassingPiece.x == tX) continue;
            }
            if (EmptySpot(targetY, tX)) continue;
            if (AllySquare(targetY, tX, color))
                return false;
            if (EnemySquare(targetY, tX, color))
            {
                if (PieceName(targetY, tX) == "King")
                    return i == 1;
                return PieceName(targetY, tX) == "Queen" || PieceName(targetY, tX) == "Rook";
            }
        }
    }
    private bool UnderAttackHorizontal(int targetY, int targetX, bool ignore, int desiredY, int desiredX, PieceColor color, bool ignorePassing, bool kingIgnore)
    {
        if (UnderAttackHorizontal(targetY, targetX, 1, ignore, desiredY, desiredX, color, ignorePassing, kingIgnore))
            return true;
        if (UnderAttackHorizontal(targetY, targetX, -1, ignore, desiredY, desiredX, color, ignorePassing, kingIgnore))
            return true;
        return false;
    }
    private bool UnderAttackVertical(int targetY, int targetX, int directionY, bool ignore, int desiredY, int desiredX, PieceColor color, bool ignorePassing, bool kingIgnore)
    {
        int i = 0;
        int tY;
        while (true)
        {
            i++;
            tY = targetY + i * directionY;


            if (PieceOutOfBoard(tY, targetX)) return false;
            if (ignore)
            {
                if (tY == desiredY && targetX == desiredX)
                {
                    if (kingIgnore) continue;
                    else return false;
                }
                if (tY == Y && targetX == X) continue;
                if (ignorePassing && InPassingPiece.y == tY && InPassingPiece.x == targetX) continue;
            }
            if (EmptySpot(tY, targetX)) continue;
            if (AllySquare(tY, targetX, color))
                return false;
            if (EnemySquare(tY, targetX, color))
            {
                if (PieceName(tY, targetX) == "King")
                    return i == 1;
                return PieceName(tY, targetX) == "Queen" || PieceName(tY, targetX) == "Rook";
            }
        }
    }
    private bool UnderAttackVertical(int targetY, int targetX, bool ignore, int desiredY, int desiredX, PieceColor color, bool ignorePassing, bool kingIgnore)
    {
        if (UnderAttackVertical(targetY, targetX, 1, ignore, desiredY, desiredX, color, ignorePassing, kingIgnore))
            return true;
        if (UnderAttackVertical(targetY, targetX, -1, ignore, desiredY, desiredX, color, ignorePassing, kingIgnore))
            return true;
        return false;
    }
    private bool UnderAttackHorse(int targetY, int targetX, int desiredY, int desiredX, PieceColor color)
    {
        (bool attacking, int y, int x) = HorseAttacking(targetY, targetX, color);
        if (attacking)
        {
            //Debug.Log($"desiredY:{desiredY}, y:{y}, int desiredX:{desiredX}, x:{x}");
            if (desiredY == y && desiredX == x && AttackAvailable(desiredY, desiredX))
                return false;
            return true;
        }
        return false;

    }
    private (bool, int, int) HorseAttacking(int targetY, int targetX, PieceColor color)
    {
        if (EnemySquare(targetY + 1, targetX + 2, color))
        {
            if (PieceName(targetY + 1, targetX + 2) == "Horse")
                return (true, targetY + 1, targetX + 2);
        }
        if (EnemySquare(targetY - 1, targetX + 2, color))
        {
            if (PieceName(targetY - 1, targetX + 2) == "Horse")
                return (true, targetY - 1, targetX + 2);
        }
        if (EnemySquare(targetY + 1, targetX - 2, color))
        {
            if (PieceName(targetY + 1, targetX - 2) == "Horse")
                return (true, targetY + 1, targetX - 2);
        }
        if (EnemySquare(targetY - 1, targetX - 2, color))
        {
            if (PieceName(targetY - 1, targetX - 2) == "Horse")
                return (true, targetY - 1, targetX - 2);
        }
        if (EnemySquare(targetY + 2, targetX + 1, color))
        {
            if (PieceName(targetY + 2, targetX + 1) == "Horse")
                return (true, targetY + 2, targetX + 1);
        }
        if (EnemySquare(targetY - 2, targetX + 1, color))
        {
            if (PieceName(targetY - 2, targetX + 1) == "Horse")
                return (true, targetY - 2, targetX + 1);
        }
        if (EnemySquare(targetY + 2, targetX - 1, color))
        {
            if (PieceName(targetY + 2, targetX - 1) == "Horse")
                return (true, targetY + 2, targetX - 1);
        }
        if (EnemySquare(targetY - 2, targetX - 1, color))
        {
            if (PieceName(targetY - 2, targetX - 1) == "Horse")
                return (true, targetY - 2, targetX - 1);
        }
        return (false, 0, 0);
    }
    protected void ShowVertical(int direction)
    {
        int i = 0;
        while (true)
        {
            i++;
            tryOut.SetTrue();
            if (EmptySpot(Y + direction * i, X))
            {
                if (!KingUnderAttackIfMove(Y + direction * i, X))
                    SetAvailableMove(Y + direction * i, X);
                tryOut.SetFalse();
            }
            else if(AttackAvailable(Y + direction * i, X))
            {
                if (!KingUnderAttackIfMove(Y + direction * i, X))
                    SetAvailableMove(Y + direction * i, X);
                tryOut.SetFalse();
                break;
            }
            else
            {
                tryOut.SetFalse();
                break;
            }
        }
    }
    protected void ShowHorizontal(int direction)
    {
        int i = 0;
        while (true)
        {
            i++;
            tryOut.SetTrue();
            if (EmptySpot(Y, X + direction * i))
            {
                if (!KingUnderAttackIfMove(Y, X + direction * i))
                    SetAvailableMove(Y, X + direction * i);
                tryOut.SetFalse();
            }
            else if (AttackAvailable(Y, X + direction * i))
            {
                if (!KingUnderAttackIfMove(Y, X + direction * i))
                    SetAvailableMove(Y, X + direction * i);
                tryOut.SetFalse();
                break;
            }
            else
            {
                tryOut.SetFalse();
                break;
            }
        }
    }
    protected void ShowDiagonal(int directionY, int directionX)
    {
        int i = 0;
        while (true)
        {
            i++;
            tryOut.SetTrue();
            if (EmptySpot(Y + i * directionY, X + i * directionX))
            {
                if (!KingUnderAttackIfMove(Y + i * directionY, X + i * directionX))
                    SetAvailableMove(Y + i * directionY, X + i * directionX);
                tryOut.SetFalse();
            }
            else if (AttackAvailable(Y + i * directionY, X + i * directionX))
            {
                if (!KingUnderAttackIfMove(Y + i * directionY, X + i * directionX))
                    SetAvailableMove(Y + i * directionY, X + i * directionX);
                tryOut.SetFalse();
                break;
            }
            else
            {
                tryOut.SetFalse();
                break;
            }
        }
    }
    protected bool NoLegalVerticalMove(int direction, bool continuous = false)
    {
        if (!continuous) return !MoveLegal(Y + direction * 1, X);
        int y = 1;
        while (EmptySpot(Y + direction * y, X) || !PieceOutOfBoard(Y + direction * y, X))
        {
            if (MoveLegal(Y + direction * 1, X)) return false;
            y++;
        }
        return true;
    }
    protected bool NoLegalHorizontalMove(int direction, bool continuous = false)
    {
        if (!continuous) return !MoveLegal(Y, X + direction * 1);
        int x = 1;
        while (EmptySpot(Y, X + direction * x) || !PieceOutOfBoard(Y, X + direction * x))
        {
            if (MoveLegal(Y, X + direction * x)) return false;
            x++;
        }
        return true;
    }
    protected bool NoLegalDiagonalMove(int directionY, int directionX, bool continuous = false)
    {
        if (!continuous) return !MoveLegal(Y + 1 * directionY, X + 1 * directionX);
        int step = 1;
        while (EmptySpot(Y + step * directionY, X + step * directionX) || !PieceOutOfBoard(Y + step * directionY, X + step * directionX))
        {
            if (MoveLegal(Y + step * directionY, X + step * directionX)) return false;
            step++;
        }
        return true;
    }

    protected bool AttackVertical(int direction, int targetY, int targetX)
    {
        int i = 0;
        while (targetY != Y + direction * i)
        {
            i++;
            if (AllySquare(Y + direction * i, X))
                return false;
            if (EnemySquare(Y + direction * i, X))
            {
                if ((Y + direction * i == targetY && targetX == X))
                    return true;
                else
                    return false;
            }
        }
        return false;

    }
    protected bool AttackHorizontal(int direction, int targetY, int targetX)
    {
        int i = 0;
        while (targetX != X + direction * i)
        {
            i++;
            if (AllySquare(Y, X + direction * i))
                return false;
            if (EnemySquare(Y, X + direction * i))
            {
                if ((X + direction * i == targetX && targetY == Y))
                    return true;
                else
                    return false;
            }
        }
        return false;
    }
    protected bool AttackDiagonal(int directionY, int directionX, int targetY, int targetX)
    {
        int i = 0;
        while (Y + i * directionY != targetY && X + i * directionX != targetX)
        {
            i++;
            if (AllySquare(Y + i * directionY, X + i * directionX))
                return false;
            if (EnemySquare(Y + i * directionY, X + i * directionX))
            {
                if ((targetY == Y + i * directionY && targetX == X + i * directionX))
                    return true;
                else
                    return false;
            }
        }
        return false;
    }
    protected bool LegalVertical(int direction, int targetY, int targetX)
    {
        int i = 0;
        while (targetY != Y + direction * i)
        {
            i++;
            if (!EmptySpot(Y + direction * i, X))
                return false;
            if (EmptySpot(Y + direction * i, X) && (Y + direction * i == targetY && targetX == X))
                return true;
        }
        return false;

    }
    protected bool LegalHorizontal(int direction, int targetY, int targetX)
    {
        int i = 0;
        while (targetX != X + direction * i)
        {
            i++;
            if (!EmptySpot(Y, X + direction * i))
                return false;
            if (EmptySpot(Y, X + direction * i) && (X + direction * i == targetX && targetY == Y))
                return true;
        }
        return false;
    }
    protected bool LegalDiagonal(int directionY, int directionX, int targetY, int targetX)
    {
        int i = 0;
        while (Y + i * directionY != targetY && X + i * directionX != targetX)
        {
            i++;
            if (!EmptySpot(Y + i * directionY, X + i * directionX))
                return false;
            if (EmptySpot(Y + i * directionY, X + i * directionX) && (targetY == Y + i * directionY && targetX == X + i * directionX))
                return true;
        }
        return false;
    }
}