public class Pawn : Piece
{
    public Pawn(Board board, PieceColor color) : base(board, color)
    {
        points = 1;
    }
    protected override bool AttackAvailable(int y, int x) => AttackAvailable(y, x, color);
    private bool AttackAvailable(int y, int x, PieceColor color)
    {
        int yD;
        if (color == PieceColor.Black)
            yD = -1;
        else
            yD = 1;
        //UnityEngine.Debug.Log(InPassing);
        if (InPassing)
        {
            if (((InPassingPiece.y + yD == y && InPassingPiece.x == x) && (InPassingPiece.x == X + 1 || InPassingPiece.x == X - 1)) && InPassingPiece.y == Y)
            {
                if(!tryOut)
                    inPassed.SetTrue();
                return true;
            }
            if (!EnemySquare(y, x)) return false;
        }
        //UnityEngine.Debug.Log("before diagonal att");
        if (y == Y + yD * 1 && (X + 1 == x || X - 1 == x))
        {
            if (!tryOut)
            {
                if (ReadyToUpgrade(y))
                    board.SetChangingPiece(y, x);
            }
            return true;
        }
        return false;
    }
    protected override bool LegalMove(int y, int x) => LegalMove(y, x, color);
    private bool LegalMove(int y, int x, PieceColor color)
    {

        int yD;
        if (color == PieceColor.Black)
            yD = -1;
        else
            yD = 1;

        if (Y == 2 || Y == 7)
        {
            if (y == Y + 2 * yD && x == X)
            {
                if (!EmptySpot(Y + 1 * yD, x)) return false;
                else
                {
                    if (!tryOut)
                        SetInPassing(Y + 2 * yD, X);
                    return true;
                }
            }
        }
        if (y == Y + 1 * yD && x == X)
        {
            if (!tryOut)
            {
                if (ReadyToUpgrade(y))
                    board.SetChangingPiece(y, x);
            }
            return true;
        }
        return false;
    }
    protected override void ShowAvailableMoves()
    {
        int yD;
        if (color == PieceColor.Black)
            yD = -1;
        else
            yD = 1;

        if (Y == 2 || Y == 7)
        {
            if (EmptySpot(Y + 2 * yD, X) && EmptySpot(Y + 1 * yD, X) && !KingUnderAttackIfMove(Y + 2 * yD, X))
                SetAvailableMove(Y + 2 * yD, X);
        }
        if (EmptySpot(Y + 1 * yD, X) && !KingUnderAttackIfMove(Y + 1 * yD, X))
            SetAvailableMove(Y + 1 * yD, X);

        if (EnemySquare(Y + 1 * yD, X - 1) && !KingUnderAttackIfMove(Y + 1 * yD, X - 1))
            SetAvailableMove(Y + 1 * yD, X - 1);
        if (EnemySquare(Y + 1 * yD, X + 1) && !KingUnderAttackIfMove(Y + 1 * yD, X + 1))
            SetAvailableMove(Y + 1 * yD, X + 1);
        if (InPassing)
        {
            if(InPassingPiece.y == Y && (InPassingPiece.x == X + 1 || InPassingPiece.x == X - 1) && !KingUnderAttackIfMove(InPassingPiece.y + 1 * yD, InPassingPiece.x, true))
                SetAvailableMove(InPassingPiece.y + 1 * yD, InPassingPiece.x);
        }
    }
    public override bool NoMoves()
    {
        int yD;
        if (color == PieceColor.Black)
            yD = -1;
        else
            yD = 1;

        if (Y == 2 || Y == 7)
        {
            if (EmptySpot(Y + 2 * yD, X) && EmptySpot(Y + 1 * yD, X) && !KingUnderAttackIfMove(Y + 2 * yD, X))
                return false;
        }
        if (EmptySpot(Y + 1 * yD, X) && !KingUnderAttackIfMove(Y + 1 * yD, X))
            return false;

        if (EnemySquare(Y + 1 * yD, X - 1) && !KingUnderAttackIfMove(Y + 1 * yD, X - 1))
            return false;
        if (EnemySquare(Y + 1 * yD, X + 1) && !KingUnderAttackIfMove(Y + 1 * yD, X + 1))
            return false;
        if (InPassing)
        {
            if (InPassingPiece.y == Y && (InPassingPiece.x == X + 1 || InPassingPiece.x == X - 1) && !KingUnderAttackIfMove(InPassingPiece.y + 1 * yD, InPassingPiece.x, true))
                return false;
        }
        return true;
    }
    private bool ReadyToUpgrade(int desiredY) => desiredY == 8 || desiredY == 1;
}