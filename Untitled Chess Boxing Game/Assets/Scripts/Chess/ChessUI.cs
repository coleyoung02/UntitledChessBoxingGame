using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct intHolder
{
    public int value;

    public intHolder(int i)
    {
        value = i;
    }
}
public class ChessUI : MonoBehaviour
{
    private ChessState chess;
    private int[][] board;
    private Button[] buttons;
    [SerializeField] private Sprite[] sprites;
    private int selectedRow;
    private int selectedCol;

    // Start is called before the first frame update
    void Start()
    {
        selectedRow = -1;
        selectedCol = -1;
        chess = new ChessState();
        board = chess.getBoard();
        buttons = this.GetComponentsInChildren<Button>();
        Array.Sort(buttons, new ButtonCompare());
        //i think its fine to hard code 8 unless chess drops an update to the board size
        //this hasnt been updated in a few centuries so it should be fine
        refreshUI();
    }

    public void squareClicked(int buttonNo) 
    {
        int row = buttonNo / 8;
        int col = buttonNo % 8;
        if (selectedRow < 0)
        {
            int piece = board[row][col];
            Debug.Log(board[row][col]);
            if (piece >= 0)
            {
                selectedRow = row;
                selectedCol = col;
            }
        }
        else
        {
            if (chess.playMove(selectedRow, selectedCol, row, col))
            {
                refreshUI();
            }
            selectedCol = -1;
            selectedRow = -1;
            Debug.Log(selectedRow + " " + selectedCol);
            Debug.Log(row + " " + col);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void refreshUI()
    {
        for (int i = 0; i < 8; ++i)
        {
            for (int j = 0; j < 8; ++j)
            {
                buttons[i * 8 + j].GetComponent<Image>().sprite = sprites[board[i][j] + 1];
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