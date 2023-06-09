using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
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
    [SerializeField] private Sprite[] indicators;
    [SerializeField] private GameObject promotionMenu;
    private int selectedRow;
    private int selectedCol;
    private int promotionCol;
    private GameObject[] overlay;
    private GameManagerClass gameManager;
    [SerializeField] private GameObject backupManager;
    [SerializeField] ChessPlayerClock whiteClock;
    [SerializeField] ChessPlayerClock blackClock;
    [SerializeField] ChessText chessText;
    [SerializeField] AudioClip[] sounds;

    private Move AIMove;
    private AudioSource source;
    private bool moveDone;
    private float extraDelay = .7f;

    //for testing only, can (and should) be deleted once ai is implemented - or leave it so i can test again
    private int color;

    private bool doStuff;

    // Start is called before the first frame update
    void Start()
    {
        doStuff = true;
        source = GetComponent<AudioSource>();
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
        moveDone = false;
        color = chess.getColor();
        setTurn(color);
        if (color == 0)
        {
            threadMove();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.ResetGame();
            SceneManager.LoadScene("TitleScreen");
        }
        if(moveDone && doStuff)
        {
            moveDone = false;
            StartCoroutine(finishMove());
        }
    }

    public void Disable()
    {
        this.doStuff = false;
        this.setTurn(-1);
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
        gameManager.setChessStateAndAI(chess, AI);
        gameManager.setPlayerChessTime(blackClock.getTime());
        gameManager.setEnemyChessTime(whiteClock.getTime());
    }

    public void endRound()
    {
        SceneManager.LoadScene("BoxingScene");
    }

    public void squareClicked(int buttonNo)
    {
        if (doStuff)
        {
            refreshUI();
            board = chess.getBoard();
            color = chess.getColor();
            int row = buttonNo / 8;
            int col = buttonNo % 8;
            if (handleClick(row, col))
            {
                refreshUI();
                playSound();
                setTurn(0);
                if (chess.isStale())
                {
                    Debug.Log("STALEMATE");
                    chessText.onStale(gameManager);
                }
                else if (chess.isMate() < 0)
                {
                    if (chess.inCheck(ChessState.white) > 0)
                    {
                        chessText.onCheck();
                    }
                    playWhite();
                }
                else
                {
                    gameManager.setWinner(GameManagerClass.Winner.PLAYER);
                    chessText.onMate();
                }
            }
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
                else if (board[selectedRow][selectedCol] == ChessState.bp && row == 7)
                {
                    List<Move> moves = chess.getLegalMoves();
                    for (int i = 0; i < moves.Count; ++i)
                    {
                        if (moves[i].startCol == selectedCol && moves[i].startRow == selectedRow &&
                            moves[i].endRow == row && moves[i].endCol == col)
                        {
                            promotionCol = col;
                            promotionMenu.SetActive(true);
                        }
                    }
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
                        promotionMenu.SetActive(true);
                    }
                    else if (row == 7 && board[row][col] == 1)
                    {
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
        threadMove();
    }

    private void setTurn(int color)
    {
        if (color == 0)
        {
            whiteClock.setTicking(true);
            blackClock.setTicking(false);
        }
        else if (color == 1)
        {
            whiteClock.setTicking(false);
            blackClock.setTicking(true);
        }
        else if (color < 0)
        {
            whiteClock.setTicking(false);
            blackClock.setTicking(false);
        }
        if (whiteClock.getTime() < blackClock.getTime())
        {
            AI.setDepth(AI.getBaseDepth());
        }
        else 
        {
            AI.setDepth(AI.getBaseDepth() + 1);
        }
    }

    private void threadMove()
    {
        AIMove = chess.getRandomMove(ChessState.white);
        Thread backgroundThread = new Thread(new ThreadStart(this.setMove));
        backgroundThread.Start();
    }

    IEnumerator finishMove()
    {
        Move m = AIMove;
        if (AIMove.startRow < 0)
        {
            overlay[7 * 8 + 3].GetComponent<Image>().sprite = indicators[4];
        }
        else
        {
            overlay[m.startRow * 8 + m.startCol].GetComponent<Image>().sprite = indicators[4];
        }
        yield return new WaitForSeconds(extraDelay + UnityEngine.Random.Range(0f, .7f));
        chess.playWhiteMove(m);
        if (chess.isStale())
        {
            Debug.Log("STALEMATE");
        }
        else if (chess.isMate() < 0)
        {
            if (chess.inCheck(ChessState.black) > 0)
            {
                chessText.onCheck();
            }
            color = (color + 1) % 2;
            board = chess.getBoard();
            setTurn(1);
        }
        else
        {
            gameManager.setWinner(GameManagerClass.Winner.ENEMY);
            chessText.onMate();
        }
        playSound();
        refreshUI();
    }

    private void setMove()
    {
        AIMove = AI.GetBestMove(chess, whiteClock.getTime(), blackClock.getTime());
        moveDone = true;
    }

    
    public void replace(int piece)
    {
        Debug.Log("inputting");
        //color gets flipped after moving pawn to end so this passes the correct color
        chess.promote(promotionCol, piece + (color) % 2, selectedRow, selectedCol);
        playSound();
        selectedCol = -1;
        selectedRow = -1;
        if (chess.isMate() < 0)
        {
            playWhite();
        }
        else
        {
            Debug.Log("You win! " + chess.isMate());
        }
        setTurn(0);
        refreshUI();
        promotionMenu.SetActive(false);
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
        if (color == ChessState.black) 
        {
            chess.setMoves(ChessState.black);
        }
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

    private void playSound()
    {
        source.clip = sounds[UnityEngine.Random.Range(0, sounds.Length)];
        source.Play();
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