using UnityEngine;

public class PieceInput : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Board board;

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
            ClickHandler();
    }

    private void ClickHandler()
    {
        if (board.AwaitingPawnUpgrade)
        {
            FocusUpgradePiece();
            return;
        }
        PieceFocus();
    }

    private void FocusUpgradePiece()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject.name == "PieceContainer(Clone)")
        {
            board.UpgradePawn(hit.collider.GetComponent<PieceContainer>().Piece);
        }
    }

    private void PieceFocus()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject.name == "Square(Clone)")
        {
            BoardSquare boardSquare = hit.collider.GetComponent<BoardSquare>();
            if (boardSquare.AllyPiece())
                boardSquare.FocusPiece();
            else if (boardSquare.Focused)
                boardSquare.HandlePieceMovement();
        }
    }
}