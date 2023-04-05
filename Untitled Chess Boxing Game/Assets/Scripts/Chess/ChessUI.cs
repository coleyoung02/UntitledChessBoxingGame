using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessUI : MonoBehaviour
{
    private ChessState chess;
    private int[][] board;
    private Button[] buttons;
    [SerializeField] private Sprite[] sprites;

    // Start is called before the first frame update
    void Start()
    {
        chess = new ChessState();
        board = chess.getBoard();
        buttons = this.GetComponentsInChildren<Button>();
        Array.Sort(buttons, new ButtonCompare());
        //i think its fine to hard code 8 unless chess drops an update to the board size
        //this hasnt been updated in a few centuries so it should be fine
        for (int i = 0; i < 8; ++i) {
            for (int j = 0; j < 8; ++j) {
                buttons[i*8 + j].GetComponent<Image>().sprite = sprites[board[i][j] + 1];
            }
        }
    }

    public void pieceClicked(String name) {
        Debug.Log("print");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}


class ButtonCompare : IComparer
{
    public int Compare(object x, object y)
    {
        return (new CaseInsensitiveComparer()).Compare(((Button)x).name, ((Button)y).name);
    }
}