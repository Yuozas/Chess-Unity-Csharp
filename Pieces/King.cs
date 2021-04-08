public class King : Piece
{
    private bool moved;

    public King(Board board, PieceColor color) : base(board, color) => moved.SetFalse();

    protected override bool AttackAvailable(int y, int x)
    {
        //if (UnderAttack(y, x)) return false;
        bool attackAvailable = AttackAvailableKing(y, x);
        if (!moved && !tryOut)
        {
            if (attackAvailable)
            {
                moved.SetTrue();
                DisableCastlingKing();
            }
        }
        return attackAvailable;
    }

    private bool AttackAvailableKing(int y, int x)
    {
        if (y > Y)
        {
            if (x > X)
                return AttackDiagonalK(1, 1, y, x);
            else if (x < X)
                return AttackDiagonalK(1, -1, y, x);
            else
                return AttackVerticalK(1, y, x);
        }
        else if (y < Y)
        {
            if (x > X)
                return AttackDiagonalK(-1, 1, y, x);
            else if (x < X)
                return AttackDiagonalK(-1, -1, y, x);
            else
                return AttackVerticalK(-1, y, x);
        }
        else
        {
            if (x > X)
                return AttackHorizontalK(1, y, x);
            else if (x < X)
                return AttackHorizontalK(-1, y, x);
            else
                return false;
        }
    }

    protected override bool LegalMove(int y, int x)
    {
        //if (UnderAttack(y, x)) return false;
        bool moveAvailable = LegalMoveKing(y, x);
        if (!moved && !tryOut)
        {
            if (moveAvailable)
            {
                moved.SetTrue();
                DisableCastlingKing();
            }
        }
        return moveAvailable;
    }

    private bool LegalMoveKing(int y, int x)
    {
        if (!moved)
        {
            if (y == Y && (x == X + 2 || x == X - 2))
            {
                int side = x > X ? 1 : -1;
                bool ableToCastle = AbleToCastle(side);
                if (ableToCastle && !tryOut)
                    CastleRook(side);
                return ableToCastle;
            }
        }

        if (y > Y)
        {
            if (x > X)
                return LegalDiagonalK(1, 1, y, x);
            else if (x < X)
                return LegalDiagonalK(1, -1, y, x);
            else
                return LegalVerticalK(1, y, x);
        }
        else if (y < Y)
        {
            if (x > X)
                return LegalDiagonalK(-1, 1, y, x);
            else if (x < X)
                return LegalDiagonalK(-1, -1, y, x);
            else
                return LegalVerticalK(-1, y, x);
        }
        else
        {
            if (x > X)
                return LegalHorizontalK(1, y, x);
            else if (x < X)
                return LegalHorizontalK(-1, y, x);
            else
                return false;
        }
    }
    public override bool NoMoves()
    {
        if (!NoLegalVerticalMove(1)) return false;
        if (!NoLegalVerticalMove(-1)) return false;
        if (!NoLegalHorizontalMove(1)) return false;
        if (!NoLegalHorizontalMove(-1)) return false;
        if (!NoLegalDiagonalMove(1, 1)) return false;
        if (!NoLegalDiagonalMove(-1, 1)) return false;
        if (!NoLegalDiagonalMove(1, -1)) return false;
        if (!NoLegalDiagonalMove(-1, -1)) return false;
        return true;
    }
    protected override void ShowAvailableMoves()
    {
        if (!moved)
        {
            if(AbleToCastle(-1))
                SetAvailableMove(Y, X - 2);
            if (AbleToCastle(1))
                SetAvailableMove(Y, X + 2);
        }
        ShowVerticalK(1);
        ShowVerticalK(-1);
        ShowHorizontalK(1);
        ShowHorizontalK(-1);
        ShowDiagonalK(1, -1);
        ShowDiagonalK(1, 1);
        ShowDiagonalK(-1, -1);
        ShowDiagonalK(-1, 1);
    }

    private bool AttackDiagonalK(int directionY, int directionX, int targetY, int targetX)
    {
        if (EnemySquare(Y + 1 * directionY, X + 1 * directionX) && (targetY == Y + 1 * directionY && targetX == X + 1 * directionX))
            return true;
        return false;
    }

    private bool AttackVerticalK(int direction, int targetY, int targetX)
    {
        if (EnemySquare(Y + direction * 1, X) && (Y + direction * 1 == targetY && targetX == X))
            return true;
        return false;
    }

    private bool AttackHorizontalK(int direction, int targetY, int targetX)
    {
        if (EnemySquare(Y, X + direction * 1) && (X + direction * 1 == targetX && targetY == Y))
            return true;
        return false;
    }

    private bool LegalDiagonalK(int directionY, int directionX, int targetY, int targetX)
    {
        if (EmptySpot(Y + 1 * directionY, X + 1 * directionX) && (targetY == Y + 1 * directionY && targetX == X + 1 * directionX))
            return true;
        return false;
    }

    private bool LegalVerticalK(int direction, int targetY, int targetX)
    {
        if (EmptySpot(Y + direction * 1, X) && (Y + direction * 1 == targetY && targetX == X))
            return true;
        return false;
    }

    private bool LegalHorizontalK(int direction, int targetY, int targetX)
    {
        if (EmptySpot(Y, X + direction * 1) && (X + direction * 1 == targetX && targetY == Y))
            return true;
        return false;
    }

    private void ShowVerticalK(int direction)
    {

        if (KingUnderAttackIfMove(Y + direction * 1, X)) return;

            if (EmptySpot(Y + direction * 1, X))
            SetAvailableMove(Y + direction * 1, X);
        else if (EnemySquare(Y + direction * 1, X))
            SetAvailableMove(Y + direction * 1, X);

    }

    private void ShowHorizontalK(int direction)
    {
        if (KingUnderAttackIfMove(Y, X + direction * 1)) return;

        if (EmptySpot(Y, X + direction * 1))
            SetAvailableMove(Y, X + direction * 1);
        else if (EnemySquare(Y, X + direction * 1))
            SetAvailableMove(Y, X + direction * 1);
    }

    private void ShowDiagonalK(int directionY, int directionX)
    {
        if (KingUnderAttackIfMove(Y + 1 * directionY, X + 1 * directionX)) return;

        if (EmptySpot(Y + 1 * directionY, X + 1 * directionX))
            SetAvailableMove(Y + 1 * directionY, X + 1 * directionX);
        else if (EnemySquare(Y + 1 * directionY, X + 1 * directionX))
            SetAvailableMove(Y + 1 * directionY, X + 1 * directionX);
    }

    private bool AbleToCastle(int side)
    {
        if (UnderAttack(Y, X + side * 1)) return false;
        if (UnderAttack(Y, X + side * 2)) return false;
        if (!EmptySpot(Y, X + side * 1)) return false;
        if (!EmptySpot(Y, X + side * 2)) return false;
        if (side == -1)
        {
            if (UnderAttack(Y, X + side * 3)) return false;
            if (!EmptySpot(Y, X + side * 3)) return false;
        }
        return true;
    }
}