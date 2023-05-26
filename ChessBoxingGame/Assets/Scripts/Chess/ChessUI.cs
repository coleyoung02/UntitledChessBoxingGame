using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ChessUI : MonoBehaviour
{
    private ChessState chess;
    private int[][] board;
    private Button[] buttons;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Sprite[] highlighted;
    [SerializeField] private GameObject promotionMenu;
    [SerializeField] private GameObject whitePiecesSelect;
    [SerializeField] private GameObject blackPiecesSelect;
    private int selectedRow;
    private int selectedCol;
    private int promotionCol;
    private GameManagerClass gameManager;
    private float playerChessTime;
    private float enemyChessTime;


    //for testing only, can (and should) be deleted once ai is implemented
    private int color;

    // Start is called before the first frame update
    void Start()
    {
        promotionCol = -1;
        color = 0;
        selectedRow = -1;
        selectedCol = -1;
        gameManager = Resources.FindObjectsOfTypeAll<GameManagerClass>()[0];
        chess = gameManager.getChessState();
        playerChessTime = gameManager.getPlayerChessTime();
        enemyChessTime = gameManager.getEnemyChessTime();
        buttons = this.GetComponentsInChildren<Button>();
        Array.Sort(buttons, new ButtonCompare());
        //i think its fine to hard code 8 unless chess drops an update to the board size
        //this hasnt been updated in a few centuries so it should be fine
        board = chess.getBoard();
        refreshUI();
        playWhite();
    }

    public void OnDestroy()
    {
        gameManager.setChessState(chess); // theoretically no need
        gameManager.setPlayerChessTime(playerChessTime);
        gameManager.setEnemyChessTime(enemyChessTime);
    }

    public void endRound()
    {
        SceneManager.LoadScene("BoxingScene");
    }

    public void squareClicked(int buttonNo) 
    {
        bool moved = false;
        int row = buttonNo / 8;
        int col = buttonNo % 8;
        if (selectedRow < 0)
        {
            int piece = board[row][col];
            if (piece >= 0 && piece % 2 == color)
            {
                selectedRow = row;
                selectedCol = col;
                //refreshUI();
            }
        }
        else
        {
            if (board[row][col] % 2 == color)
            {
                selectedRow = row;
                selectedCol = col;
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
                    Debug.Log("hey");
                    promotionCol = col;
                    whitePiecesSelect.SetActive(true);
                    promotionMenu.SetActive(true);
                }
                else if (row == 7 && board[row][col] == 1)
                {
                    Debug.Log("hey");

                    promotionCol = col;
                    blackPiecesSelect.SetActive(true);
                    promotionMenu.SetActive(true);
                }
                selectedCol = -1;
                selectedRow = -1;
            }
        }
        refreshUI();
        if (moved)
        {
            playWhite();
        }
    }

    public void playWhite()
    {
        //do something better
        StartCoroutine(playMove());
    }

    IEnumerator playMove()
    {
        yield return new WaitForSeconds(1.5f);
        chess.playWhite();
        color = (color + 1) % 2;
        board = chess.getBoard();
        refreshUI();
    }

    // Update is called once per frame
    void Update()
    {
        
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
                    buttons[i * 8 + j].GetComponent<Image>().sprite = highlighted[board[i][j] + 1];
                }
                else
                {
                    buttons[i * 8 + j].GetComponent<Image>().sprite = sprites[board[i][j] + 1];
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