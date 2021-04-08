public class Queen : Piece
{
    public Queen(Board board, PieceColor color) : base(board, color)
    {
        points = 9;
    }

    protected override bool AttackAvailable(int y, int x)
    {
        if (y > Y)
        {
            if (x > X)
                return AttackDiagonal(1, 1, y, x);
            else if (x < X)
                return AttackDiagonal(1, -1, y, x);
            else
                return AttackVertical(1, y, x);
        }
        else if (y < Y)
        {
            if (x > X)
                return AttackDiagonal(-1, 1, y, x);
            else if (x < X)
                return AttackDiagonal(-1, -1, y, x);
            else
                return AttackVertical(-1, y, x);
        }
        else
        {
            if (x > X)
                return AttackHorizontal(1, y, x);
            else if (x < X)
                return AttackHorizontal(-1, y, x);
            else
                return false;
        }
    }

    protected override bool LegalMove(int y, int x)
    {
        if(y > Y)
        {
            if (x > X)
                return LegalDiagonal(1, 1, y, x);
            else if (x < X)
                return LegalDiagonal(1, -1, y, x);
            else
                return LegalVertical(1, y, x);
        }
        else if(y < Y)
        {
            if (x > X)
                return LegalDiagonal(-1, 1, y, x);
            else if (x < X)
                return LegalDiagonal(-1, -1, y, x);
            else
                return LegalVertical(-1, y, x);
        }
        else
        {
            if (x > X)
                return LegalHorizontal(1, y, x);
            else if (x < X)
                return LegalHorizontal(-1, y, x);
            else
                return false;
        }
    }

    protected override void ShowAvailableMoves()
    {
        ShowVertical(1);
        ShowVertical(-1);
        ShowHorizontal(1);
        ShowHorizontal(-1);
        ShowDiagonal(1, -1);
        ShowDiagonal(1, 1);
        ShowDiagonal(-1, -1);
        ShowDiagonal(-1, 1);

    }
    public override bool NoMoves()
    {
        if (!NoLegalVerticalMove(1, true)) return false;
        if (!NoLegalVerticalMove(-1, true)) return false;
        if (!NoLegalHorizontalMove(1, true)) return false;
        if (!NoLegalHorizontalMove(-1, true)) return false;
        if (!NoLegalDiagonalMove(1, 1, true)) return false;
        if (!NoLegalDiagonalMove(-1, 1, true)) return false;
        if (!NoLegalDiagonalMove(1, -1, true)) return false;
        if (!NoLegalDiagonalMove(-1, -1, true)) return false;
        return true;
    }
}