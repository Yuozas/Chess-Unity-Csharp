public class Horse : Piece
{
    public Horse(Board board, PieceColor color) : base(board, color)
    {
        points = 3;
    }

    protected override bool AttackAvailable(int y, int x)
    {
        if (!EnemySquare(y, x)) return false;
        if (y == Y + 1 || y == Y - 1)
        {
            if (x == X + 2)
                return true;
            if (x == X - 2)
                return true;
        }
        else if (y == Y + 2 || y == Y - 2)
        {
            if (x == X + 1)
                return true;
            if (x == X - 1)
                return true;
        }
        return false;
    }

    protected override bool LegalMove(int y, int x)
    {
        if (!EmptySpot(y, x)) return false;
        if (y == Y + 1 || y == Y - 1)
        {
            if (x == X + 2)
                return true;
            if (x == X - 2)
                return true;
        }
        else if (y == Y + 2 || y == Y - 2)
        {
            if (x == X + 1)
                return true;
            if (x == X - 1)
                return true;
        }
        return false;
    }

    protected override void ShowAvailableMoves()
    {
        SetAvailableMoveIfNoAlliesNotUnderAttack(Y + 1, X + 2);
        SetAvailableMoveIfNoAlliesNotUnderAttack(Y + 1, X - 2);
        SetAvailableMoveIfNoAlliesNotUnderAttack(Y + 2, X + 1);
        SetAvailableMoveIfNoAlliesNotUnderAttack(Y + 2, X - 1);
        SetAvailableMoveIfNoAlliesNotUnderAttack(Y - 1, X + 2);
        SetAvailableMoveIfNoAlliesNotUnderAttack(Y - 1, X - 2);
        SetAvailableMoveIfNoAlliesNotUnderAttack(Y - 2, X + 1);
        SetAvailableMoveIfNoAlliesNotUnderAttack(Y - 2, X - 1);
    }
    public override bool NoMoves()
    {
        if (MoveLegal(Y + 1, X + 2)) return false;
        if (MoveLegal(Y + 1, X - 2)) return false;
        if (MoveLegal(Y + 2, X + 1)) return false;
        if (MoveLegal(Y + 2, X - 1)) return false;
        if (MoveLegal(Y - 1, X + 2)) return false;
        if (MoveLegal(Y - 1, X - 2)) return false;
        if (MoveLegal(Y - 2, X + 1)) return false;
        if (MoveLegal(Y - 2, X - 1)) return false;
        return true;
    }
}