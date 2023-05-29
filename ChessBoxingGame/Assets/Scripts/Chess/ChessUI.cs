using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ChessUI : MonoBehaviour
{
    private ChessState chess;
    private ChessAI AI;
    private int[][] board;
    private Button[] buttons;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Sprite[] highlighted;
    [SerializeField] private Sprite[] indicators;
    [SerializeField] private GameObject promotionMenu;
    [SerializeField] private GameObject whitePiecesSelect;
    [SerializeField] private GameObject blackPiecesSelect;
    private int selectedRow;
    private int selectedCol;
    private int promotionCol;
    private GameObject[] overlay;
    private GameManagerClass gameManager;
    [SerializeField] private GameObject backupManager;
    [SerializeField] ChessPlayerClock whiteClock;
    [SerializeField] ChessPlayerClock blackClock;


    //for testing only, can (and should) be deleted once ai is implemented
    private int color;

    // Start is called before the first frame update
    void Start()
    {
        promotionCol = -1;
        selectedRow = -1;
        selectedCol = -1;
        manageGame();
        buttons = this.GetComponentsInChildren<Button>();
        overlay = GameObject.FindGameObjectsWithTag("Overlay");
        Array.Sort(buttons, new ButtonCompare());
        Array.Sort(overlay, new OverlayCompare());
        board = chess.getBoard();
        AI = new ChessAI(chess);
        refreshUI();
        setTurn(color);
        if (color == 0)
        {
            playWhite();
        }
    }

    private void manageGame()
    {
        gameManager = Resources.FindObjectsOfTypeAll<GameManagerClass>()[0];
        if (gameManager.getChessState() == null)
        {
            gameManager = Instantiate(backupManager, new Vector2(0, 0), Quaternion.identity).GetComponent<GameManagerClass>();
        }
        chess = gameManager.getChessState();
        color = chess.getColor();
        blackClock.setTime(gameManager.getPlayerChessTime());
        whiteClock.setTime(gameManager.getEnemyChessTime());
    }



    public void OnDestroy()
    {
        gameManager.setChessState(chess); // theoretically no need
        gameManager.setPlayerChessTime(blackClock.getTime());
        gameManager.setEnemyChessTime(whiteClock.getTime());
    }

    public void endRound()
    {
        SceneManager.LoadScene("BoxingScene");
    }

    public void squareClicked(int buttonNo)
    {
        board = chess.getBoard();
        int row = buttonNo / 8;
        int col = buttonNo % 8;
        if (handleClick(row, col))
        {
            setTurn(0);
            playWhite();
        }
    }

    private bool handleClick(int row, int col)
    {
        if (color == 1)
        {
            bool moved = false;
            if (selectedRow < 0)
            {
                int piece = board[row][col];
                if (piece >= 0 && piece % 2 == color)
                {
                    setMoveSquares(row, col);
                }
            }
            else
            {
                if (board[row][col] % 2 == color)
                {
                    setMoveSquares(row, col);
                }
                else
                {
                    if (chess.playMove(selectedRow, selectedCol, row, col, color))
                    {
                        color = (color + 1) % 2;
                        board = chess.getBoard();
                        moved = true;
                    }
                    if (row == 0 && board[row][col] == 0)
                    {
                        promotionCol = col;
                        whitePiecesSelect.SetActive(true);
                        promotionMenu.SetActive(true);
                    }
                    else if (row == 7 && board[row][col] == 1)
                    {
                        promotionCol = col;
                        blackPiecesSelect.SetActive(true);
                        promotionMenu.SetActive(true);
                    }
                    selectedCol = -1;
                    selectedRow = -1;
                }
            }
            refreshUI();
            return moved;
        }
        return false;
    }

    public void playWhite()
    {
        //do something better
        StartCoroutine(playMove());
    }

    private void setTurn(int color)
    {
        if (color == 0)
        {
            whiteClock.setTicking(true);
            blackClock.setTicking(false);
        }
        else
        {
            whiteClock.setTicking(false);
            blackClock.setTicking(true);
        }
    }

    IEnumerator playMove()
    {
        yield return new WaitForSeconds(1.2f);
        Move m = AI.GetBest();
        if (m.startRow < 0)
        {
            overlay[7 * 8 + 3].GetComponent<Image>().sprite = indicators[4];
        }
        else
        {
            overlay[m.startRow * 8 + m.startCol].GetComponent<Image>().sprite = indicators[4];
        }
        yield return new WaitForSeconds(.3f);
        chess.playWhiteMove(m);
        color = (color + 1) % 2;
        board = chess.getBoard();
        refreshUI();
        setTurn(1);
    }

    public void replace(int piece)
    {
        //color gets flipped after moving pawn to end so this passes the correct color
        chess.promote(promotionCol, piece + (color + 1) % 2);
        refreshUI();
        promotionMenu.SetActive(false);
        whitePiecesSelect.SetActive(false);
        whitePiecesSelect.SetActive(false);
    }

    private void refreshUI()
    {
        board = chess.getBoard();
        for (int i = 0; i < 8; ++i)
        {
            for (int j = 0; j < 8; ++j)
            {
                if (selectedRow >= 0 && selectedRow == i && selectedCol == j)
                {
                    overlay[i * 8 + j].GetComponent<Image>().sprite = indicators[1];
                }
                else
                {
                    overlay[i * 8 + j].GetComponent<Image>().sprite = indicators[0];
                    
                }
                buttons[i * 8 + j].GetComponent<Image>().sprite = sprites[board[i][j] + 1];
            }
        }
        highlightLegal();
    }

    private void setMoveSquares(int row, int col)
    {
        selectedRow = row;
        selectedCol = col;
    }

    private void highlightLegal()
    {
        List<Move> legal = chess.getLegalMoves();
        for (int i = 0; i < legal.Count; ++i)
        {
            if (legal[i].startRow == selectedRow && legal[i].startCol == selectedCol)
            {
                if (chess.getBoard()[legal[i].endRow][legal[i].endCol] % 2 != 0)
                {
                    overlay[legal[i].endRow * 8 + legal[i].endCol].GetComponent<Image>().sprite = indicators[2];
                }
                else
                {
                    overlay[legal[i].endRow * 8 + legal[i].endCol].GetComponent<Image>().sprite = indicators[3];
                }
            }
            else if (legal[i].startRow < 0 && selectedRow == 7 && selectedCol == 3)
            {
                if (legal[i].endCol < 0)
                {   
                    overlay[7 * 8 + 1].GetComponent<Image>().sprite = indicators[2];
                }
                else if (legal[i].endRow < 0)
                {
                    overlay[7 * 8 + 5].GetComponent<Image>().sprite = indicators[2];
                }
            }
            else if (legal[i].startCol < 0 && selectedRow == 0 && selectedCol == 3)
            {
                if (legal[i].endCol < 0)
                {
                    overlay[0 * 8 + 1].GetComponent<Image>().sprite = indicators[2];
                }
                else if (legal[i].endRow < 0)
                {
                    overlay[0 * 8 + 5].GetComponent<Image>().sprite = indicators[2];
                }
            }
        }
    }
}



class ButtonCompare : IComparer
{
    public int Compare(object x, object y)
    {
        return (new CaseInsensitiveComparer()).Compare(((Button)x).name, ((Button)y).name);
    }
}

class OverlayCompare : IComparer
{
    public int Compare(object x, object y)
    {
        return (new CaseInsensitiveComparer()).Compare(((GameObject)x).name, ((GameObject)y).name);
    }
}