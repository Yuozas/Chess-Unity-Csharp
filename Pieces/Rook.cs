public class Rook : Piece
{
    bool moved;
    public Rook(Board board, PieceColor color) : base(board, color)
    {
        moved.SetFalse();
        points = 5;
    }

    protected override bool AttackAvailable(int y, int x)
    {
        bool attackAvailable = AttackAvailableRook(y, x);
        if (!moved && !tryOut)
        {
            if (attackAvailable)
            {
                moved.SetTrue();
                DisableCastlingRook();
            }
        }
        return attackAvailable;
    }
    private bool AttackAvailableRook(int y, int x)
    {
        if (y > Y && x == X)
        {
            return AttackVertical(1, y, x);
        }
        else if (y < Y && x == X)
        {
            return AttackVertical(-1, y, x);
        }
        else
        {
            if (x > X)
                return AttackHorizontal(1, y, x);
            else if (x < X)
                return AttackHorizontal(-1, y, x);
        }
        return false;
    }

    protected override bool LegalMove(int y, int x)
    {
        bool moveAvailable = LegalMoveRook(y, x);
        if (!moved && !tryOut)
        {
            if (moveAvailable)
            {
                moved.SetTrue();
                DisableCastlingRook();
            }
        }
        return moveAvailable;
    }
    private bool LegalMoveRook(int y, int x)
    {
        if (y > Y && x == X)
        {
            return LegalVertical(1, y, x);
        }
        else if (y < Y && x == X)
        {
            return LegalVertical(-1, y, x);
        }
        else
        {
            if (x > X)
                return LegalHorizontal(1, y, x);
            else if (x < X)
                return LegalHorizontal(-1, y, x);
        }
        return false;
    }
    protected override void ShowAvailableMoves()
    {
        ShowVertical(1);
        ShowVertical(-1);
        ShowHorizontal(1);
        ShowHorizontal(-1);
    }
    public override bool NoMoves()
    {
        if (!NoLegalVerticalMove(1, true)) return false;
        if (!NoLegalVerticalMove(-1, true)) return false;
        if (!NoLegalHorizontalMove(1, true)) return false;
        if (!NoLegalHorizontalMove(-1, true)) return false;
        return true;
    }
}