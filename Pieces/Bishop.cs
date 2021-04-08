public class Bishop : Piece
{
    public Bishop(Board board, PieceColor color) : base(board, color)
    {
        points = 3;
    }

    protected override bool AttackAvailable(int y, int x)
    {
        if (y > Y)
        {
            if (x > X)
                return AttackDiagonal(1, 1, y, x);
            else if (x < X)
                return AttackDiagonal(1, -1, y, x);
        }
        else if (y < Y)
        {
            if (x > X)
                return AttackDiagonal(-1, 1, y, x);
            else if (x < X)
                return AttackDiagonal(-1, -1, y, x);
        }
        return false;
    }

    protected override bool LegalMove(int y, int x)
    {
        if (y > Y)
        {
            if (x > X)
                return LegalDiagonal(1, 1, y, x);
            else if (x < X)
                return LegalDiagonal(1, -1, y, x);
        }
        else if (y < Y)
        {
            if (x > X)
                return LegalDiagonal(-1, 1, y, x);
            else if (x < X)
                return LegalDiagonal(-1, -1, y, x);
        }
        return false;
    }

    protected override void ShowAvailableMoves()
    {
        ShowDiagonal(1, -1);
        ShowDiagonal(1, 1);
        ShowDiagonal(-1, -1);
        ShowDiagonal(-1, 1);
    }
    public override bool NoMoves()
    {
        if (!NoLegalDiagonalMove(1, 1, true)) return false;
        if (!NoLegalDiagonalMove(-1, 1, true)) return false;
        if (!NoLegalDiagonalMove(1, -1, true)) return false;
        if (!NoLegalDiagonalMove(-1, -1, true)) return false;
        return true;
    }
}