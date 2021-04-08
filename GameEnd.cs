using UnityEngine;
using UnityEngine.UI;

public class GameEnd : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Text winnerOut, winnerIn;
    public void CheckMate(string winner)
    {
        PanelShow();
        ShowText("Winner is " + winner.ToUpper());
    }
    public void StaleMate(string stalemateBy)
    {
        PanelShow();
        ShowText("Draw... Stalemate by " + stalemateBy.ToUpper());
    }

    private void ShowText(string text)
    {
        winnerOut.text = text;
        winnerIn.text = text;
    }
    private void PanelShow() => panel.SetActive(true);
}
