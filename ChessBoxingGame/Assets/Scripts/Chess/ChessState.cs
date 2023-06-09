using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using UnityEngine; //for debugging
using static Unity.VisualScripting.Member;

//replacing lists with arrays will make it more time and space efficient if needed
//switching ints to sbytes will make it more space efficient (up to 4 times) if needed but less stable (not CLS compliant)
public struct Move {
    public int startRow;
    public int startCol;
    public int endRow;
    public int endCol;
    public bool isEP;

    //if start row is negative, white is castling
    //if start col is negative, black is castling
    //if end row is negative, it is queenside
    //if end col is negative, it is kingside
    
    //if neither start value is negative, but end col is, it is en passant
    //in this case everything else is still accurate, but the end col should be replaced by ep col
    public Move(int startRow, int startCol, int endRow, int endCol, bool isEP=false)
    {
        this.startRow = startRow;
        this.startCol = startCol;
        this.endRow = endRow;
        this.endCol = endCol;
        this.isEP = isEP;
    }

}

public struct CastlingRights
{
    public bool wQS;
    public bool bQS;
    public bool wKS;
    public bool bKS;
    public bool wH;
    public bool bH;

    public CastlingRights(bool wQS, bool bQS, bool wKS, bool bKS, bool wH, bool bH)
    {
        this.wQS = wQS;
        this.bQS = bQS;
        this.wKS = wKS;
        this.bKS = bKS;
        this.wH = wH;
        this.bH = bH;
    }
}

public class ChessState
{

    public ChessState() {
        this.init_new_board();
        this.setMoves(white);
        this.currentColor = white;
        this.halfMovesIn = 0;
        this.blackHasCastled = false;
        this.whiteHasCastled = false;
        this.threeMove = false;
        this.epCol = -1;
        statesToCheck = new Queue<ChessState>();
        statesToCheck.Enqueue(new ChessState(this));
    }

    public ChessState(ChessState current)
    {
        this.board = new int[8][];
        for (int i = 0; i < 8; ++i)
        {
            this.board[i] = new int[8];
            for (int j = 0; j < 8; ++j)
            {
                this.board[i][j] = current.board[i][j];
            }
        }
        this.whiteCanCastleQS = current.whiteCanCastleQS;
        this.blackCanCastleQS = current.blackCanCastleQS;
        this.whiteCanCastleKS = current.whiteCanCastleKS;
        this.blackCanCastleKS = current.blackCanCastleKS;

        this.epCol = current.epCol;
        this.wkRow = current.wkRow;
        this.wkCol = current.wkCol;
        this.bkRow = current.bkRow;
        this.bkCol = current.bkCol;
        this.currentColor = current.currentColor;
        this.realMoves = new List<Move>(current.realMoves);
        this.halfMovesIn = current.halfMovesIn;
        this.whiteHasCastled = current.whiteHasCastled;
        this.blackHasCastled = current.blackHasCastled;
        this.threeMove = current.threeMove;
        this.statesToCheck = new Queue<ChessState>();
        foreach (ChessState s in current.statesToCheck)
        {
            this.statesToCheck.Enqueue(s);
        }
    }

    //we can avoid checking this for every single move only checking positions since the last piece was captured or pawn was moved
    public bool Equals(ChessState c2)
    {
        if (this.currentColor != c2.currentColor)
        {
            return false;
        }

        if (!(this.whiteCanCastleKS == c2.whiteCanCastleKS && this.whiteCanCastleQS == c2.whiteCanCastleQS &&
            this.blackCanCastleKS == c2.blackCanCastleKS && this.blackCanCastleQS == c2.blackCanCastleQS))
        {
            return false;
        }

        for (int i = 0; i < 7; ++i)
        {
            for (int j = 0; j < 7; ++j)
            {
                if (this.board[i][j] != c2.board[i][j])
                {
                    return false;
                }
            }
        }

        if (this.epCol != c2.epCol)
        {
            return false;
        }
        return true;
    }

    public CastlingRights GetCastlingRights()
    {
        return new CastlingRights(whiteCanCastleQS, blackCanCastleQS, whiteCanCastleKS, blackCanCastleKS, whiteHasCastled, blackHasCastled);
    }

    public int getPlayer()
    {
        return currentColor;
    }

    public int getHalfMoves()
    {
        return halfMovesIn;
    }

    //whoever gets this array should really not touch it
    //maybe i should make a copy
    public int[][] getBoard() {
        return this.board;
    }

    public int getColor()
    {
        return this.currentColor;
    }

    //if mate, return losing color, otherwise return -1
    public int isMate()
    {
        if (this.realMoves.Count == 0 && (blackCheck >= 0 || whiteCheck >= 0))
        {
            return this.currentColor;
        }
        else
        {
            return -1;
        }
    }

    public bool isStale()
    {
        if (threeMove)
        {
            return true;
        }
        bool b;
        b = this.realMoves.Count == 0 && !(blackCheck >= 0 || whiteCheck >= 0);
        return b;
    }

    public bool playMove(int row, int col, int newRow, int newCol, int color) {
        //List<Move> possible = squareMoves(row, col, color);
        potentialMoves = squareMoves(row, col, color);
        List<Move> possible = legalMoves(color);
        bool canMove = false;
        bool isEP = false;
        for (int i = 0; i < possible.Count; ++i)
        {
            //Debug.Log("possible move: " + moveStr(possible[i]));
            if (possible[i].startCol == col && possible[i].startRow == row
                && possible[i].endCol == newCol && possible[i].endRow == newRow)
            {
                isEP = possible[i].isEP;
                canMove = true;
                break;
            }
            //Debug.Log(moveStr(possible[i]));
        }
        if (canMove)
        {
            if (board[row][col] == wK)
            {
                doKingMoveUpdates(newRow, newCol, white);
            }
            else if (board[row][col] == bK)
            {
                doKingMoveUpdates(newRow, newCol, black);
            }
            else if (isEP)
            {
                if (color == black)
                {
                    board[4][epCol] = ee;
                }
                else
                {
                    board[3][epCol] = ee;
                }
            }
            else {
                doCornerUpdates(row, col);
                doCornerUpdates(newRow, newCol);
            }
            if (board[row][col] == wp && newRow == row - 2)
            {
                epCol = col;
            }
            else if (board[row][col] == bp && newRow == row + 2)
            {
                epCol = col;
            } 
            else
            {
                epCol = -1;
            }
            board[newRow][newCol] = board[row][col];
            board[row][col] = ee;
            this.onMove(color);
            return true;
        }
        else if (isCastling(row, col, newRow, newCol, color))
        {
            for (int i = 0; i < realMoves.Count; ++i)
            {
                if (moveMatch(realMoves[i], castleType))
                {
                    if (tryCastling(castleType))
                    {
                        epCol = -1;
                        this.onMove(color);
                        return true;
                    }
                }
            }
            return false;
        }
        return false;
    }

    public void playMove(Move m, bool setNext=true)
    {
        if (m.startRow >= 0 & m.startCol >= 0)
        {
            if (board[m.endRow][m.endCol] != ee)
            {
                this.statesToCheck = new Queue<ChessState>();
            }
            else if (board[m.startRow][m.startCol] == wp || board[m.startRow][m.startCol] == bp)
            {
                this.statesToCheck = new Queue<ChessState>();
            }
        } 
        else
        {
            this.statesToCheck = new Queue<ChessState>();
        }
        if (this.currentColor == white)
        {
            playWhiteMove(m, setNext);
        }
        else
        {
            playBlackMove(m, setNext);
        }
    }

    public void playWhiteMove(Move m, bool setNext = true)
    {
        if (m.startRow < 0 && m.startCol >= 0)
        {
            tryCastling(m);
            this.onMove(white);
            return;
        }
        if (m.isEP)
        {
            board[3][epCol] = ee;
        }
        else
        {
            if (m.startRow >= 0 && board[m.startRow][m.startCol] == wK)
            {
                doKingMoveUpdates(m.endRow, m.endCol, white);
            }
            if (board[m.startRow][m.startCol] == wp && m.endRow == m.startRow - 2)
            {
                epCol = m.endCol;
            }
            doCornerUpdates(m.startRow, m.startCol);
            doCornerUpdates(m.endRow, m.endCol);
        }
        if (m.endRow == 0 && board[m.startRow][m.startCol] == wp)
        {
            board[m.endRow][m.endCol] = wQ;
        }
        else
        {
            board[m.endRow][m.endCol] = board[m.startRow][m.startCol];
        }
        board[m.startRow][m.startCol] = ee;
        if (setNext)
        {
            this.onMove(white);
        }
        return;
    }

    public void playBlackMove(Move m, bool setNext = true)
    {
        if (m.startCol < 0 && m.startRow >=0)
        {
            tryCastling(m);
            this.onMove(black);
            return;
        }
        if (m.isEP)
        {
            board[5][epCol] = ee;
        }
        else
        {
            if (m.startRow >= 0 && board[m.startRow][m.startCol] == bK)
            {
                doKingMoveUpdates(m.endRow, m.endCol, black);
            }
            if (board[m.startRow][m.startCol] == bp && m.endRow == m.startRow + 2)
            {
                epCol = m.endCol;
            }
            doCornerUpdates(m.startRow, m.startCol);
            doCornerUpdates(m.endRow, m.endCol);
        }
        if (m.endRow == 7 && board[m.startRow][m.startCol] == bp)
        {
            board[m.endRow][m.endCol] = bQ;
        }
        else
        {
            board[m.endRow][m.endCol] = board[m.startRow][m.startCol];
        }
        board[m.startRow][m.startCol] = ee;
        if (setNext)
        {
            this.onMove(black);
        }
        return;
    }

    public bool playWhiteRandom()
    {
        Move m = getRandomMove(white);
        if (m.startRow == -1 && m.startCol == -1 && m.endRow == -1 && m.endCol == -1)
        {
            return false;
        }
        playWhiteMove(m);
        return true;
    }

    public Move getRandomMove(int color)
    {
        setMoves(white);
        List<Move> possible = legalMoves(color);
        if (possible.Count == 0)
        {
            return new Move(-1, -1, -1, -1);
        }
        System.Random rnd = new System.Random();
        return possible[rnd.Next(0, possible.Count)];
    }

    public void promote(int col, int piece, int prevRow, int prevCol)
    {
        int row = -1;
        if (piece % 2 == white)
        {
            row = 0;
        }
        else
        {
            row = 7;
        }
        board[row][col] = piece;
        board[prevRow][prevCol] = ee;
        this.onMove(black);
    }

    public int inCheck(int color)
    {
        if (speedyCheck(color))
        {
            return 1;
        }
        return 0;
        if (color == white)
        {
            return squareAttacked(wkRow, wkCol, white);
        }
        else
        {
            return squareAttacked(bkRow, bkCol, black);
        }
    }

    //ugly but fast
    public bool speedyCheck(int color)
    {
        int newRow;
        int newCol;
        int row;
        int col;
        int delta = color * 2 - 1;
        bool checkPawns;

        if (Math.Abs(wkRow - bkRow) <= 1 && Math.Abs(wkCol - bkCol) <= 1)
        {
            return true;
        }
        if (color == white)
        {
            row = wkRow;
            col = wkCol;
            checkPawns = row + delta > 0;
        }
        else
        {
            row = bkRow;
            col = bkCol;
            checkPawns = row + delta < 7;
        }

        foreach (int[] dir in rookDirs)
        {
            newRow = row + dir[0];
            newCol = col + dir[1];
            while (newRow < 8 && newRow >= 0 && newCol < 8 && newCol >= 0)
            {
                if (board[newRow][newCol] < 0)
                {
                    newRow = newRow + dir[0];
                    newCol = newCol + dir[1];
                }
                else if (board[newRow][newCol] == br - color || board[newRow][newCol] == bQ - color)
                {
                    return true;
                }
                else
                {
                    break;
                }
            }
        }
        foreach (int[] dir in bishopDirs)
        {
            newRow = row + dir[0];
            newCol = col + dir[1];
            while (newRow < 8 && newRow >= 0 && newCol < 8 && newCol >= 0)
            {
                if (board[newRow][newCol] < 0)
                {
                    newRow = newRow + dir[0];
                    newCol = newCol + dir[1];
                }
                else if (board[newRow][newCol] == bb - color || board[newRow][newCol] == bQ - color)
                {
                    return true;
                }
                else
                {
                    break;
                }
            }
        }
        foreach (int[] dir in knightPossiblities)
        {
            newRow = row + dir[0];
            newCol = col + dir[1];
            //check if its on the board, and then if its not occupied by the same color piece
            if (newRow < 8 && newRow >= 0 && newCol < 8 && newCol >= 0 &&
                (board[newRow][newCol] == bk - color))
            {
                return true;
            }
        }
        if (checkPawns && ((col - 1 >= 0 && board[row + delta][col - 1] == bp - color) ||
            (col + 1 <= 7 && board[row + delta][col + 1] == bp - color)))
        {
            return true;
        }
        return false;
    }


    public List<Move> getLegalMoves()
    {
        return realMoves;
    }


    // piece format:
    // even  : white
    // odd   : black
    // -1    : empty
    // 0/1   : pawn
    // 2/3   : knight
    // 4/5   : bishop
    // 6/7   : rook
    // 8/9   : queen
    // 10/11 : king
    public const int white = 0;
    public const int black = 1;
    public const int ee = -1;
    public const int wp = 0;
    public const int bp = 1;
    public const int wk = 2;
    public const int bk = 3;
    public const int wb = 4;
    public const int bb = 5;
    public const int wr = 6;
    public const int br = 7;
    public const int wQ = 8;
    public const int bQ = 9;
    public const int wK = 10;
    public const int bK = 11;
    //this shouldn't ever be on the board outside of the 
    //square attacker count function being called with true for the default arg
    private const int placeHolderPiece = 12;
    private static readonly int[][] knightPossiblities = {new int[] {1, 2}, new int[] {2, 1}, new int[] {-1,-2}, new int[] {-2,-1}, 
                                new int[] {-1,2}, new int[] {-2,1}, new int[] {1,-2}, new int[] {2,-1}};
    private static readonly int[][] bishopDirs = {new int[] {1,1}, new int[] {1,-1}, new int[] {-1,1}, new int[] {-1,-1}};
    private static readonly int[][] rookDirs = {new int[] {0,1}, new int[] {0,-1}, new int[] {1,0}, new int[] {-1,0}};
    // board[row][col]
    private bool whiteCanCastleQS;
    private bool blackCanCastleQS;
    private bool whiteCanCastleKS;
    private bool blackCanCastleKS;
    private bool whiteHasCastled;
    private bool blackHasCastled;

    private int[][] board;
    //may be more efficient as a linked list
    private List<Move> potentialMoves;
    private List<Move> realMoves;
    //relevant for en passant worry about this later
    //row is not relevant as it should always be 5 for white moving and 2 for black moving
    private int epCol;

    private int wkRow;
    private int wkCol;
    private int bkRow;
    private int bkCol;

    private int currentColor;

    private Move castleType;
    private int halfMovesIn;
    private int whiteCheck;
    private int blackCheck;

    private bool threeMove;
    private Queue<ChessState> statesToCheck;


    private void init_new_board() {
        board = new int[8][];
        for (int i = 0; i < 8; ++i) {
            board[i] = new int[8];
        }
        //empty squares
        for (int i = 2; i < 6; ++i) {
            for (int j = 0; j < 8; ++j) {
                board[i][j] = -1;
            }
        }
        //pawns
        for (int i = 0; i < 8; ++i) {
            board[1][i] = 1;
            board[6][i] = 0;
        }
        place_pieces();
    }

    private void place_pieces() {
        //rooks
        board[0][0] = br;
        board[0][7] = br;
        board[7][0] = wr;
        board[7][7] = wr;
        //knights
        board[0][1] = bk;
        board[0][6] = bk;
        board[7][1] = wk;
        board[7][6] = wk;
        //bishops
        board[0][2] = bb;
        board[0][5] = bb;
        board[7][2] = wb;
        board[7][5] = wb;
        //queens
        board[0][4] = bQ;
        board[7][4] = wQ;
        //kings
        board[0][3] = bK;
        bkRow = 0;
        bkCol = 3;
        board[7][3] = wK;
        wkRow = 7;
        wkCol = 3;
        whiteCanCastleKS = true;
        whiteCanCastleQS = true;
        blackCanCastleKS = true;
        blackCanCastleQS = true;
    }

    public void setMoves(int color)
    {
        potentialMoves = possibleMoves(color);
        realMoves = legalMoves(color);
    }

    private void onMove(int color) {
        this.currentColor = (color + 1) % 2;
        potentialMoves = possibleMoves(this.currentColor);
        realMoves = legalMoves(this.currentColor);
        this.halfMovesIn++;
        if (this.currentColor == black)
        {
            blackCheck = inCheck(black);
        }
        if (this.currentColor == white)
        {
            whiteCheck = inCheck(white);
        }
        if (statesToCheck.Count >= 9)
        {
            statesToCheck.Dequeue();
        }
        if (statesToCheck.Count >= 4)
        {
            int counter = 0;
            ChessState checkAgain = null;
            foreach (ChessState s in statesToCheck)
            {
                if (this.Equals(s))
                {
                    ++counter;
                    checkAgain = s;
                }
            }
            if (checkAgain != null)
            {
                foreach (ChessState s in checkAgain.statesToCheck)
                {
                    if (this.Equals(s))
                    {
                        ++counter;
                        checkAgain = s;
                    }
                }
            }
            if (counter >= 2)
            {
                threeMove = true;
            }
        }
        statesToCheck.Enqueue(new ChessState(this));
    }

    private List<Move> legalMoves(int color) {
        for (int i = potentialMoves.Count - 1; i >=0; --i)
        {
            Move m = potentialMoves[i];
            //on castling
            //not in check
            //not moving through check
            //not moving into check
            //do not have to worry about movement of the rook putting us into check as the
            //only way this would happen is if a rook/queen was off the board on the back rank 
            //ex. at -1 or 9
            if (m.startRow < 0 || m.startCol < 0)
            {
                if(!isCastlingSafe(m))
                {
                    potentialMoves.RemoveAt(i);
                }
            }
            //using negative indicies here may kill everything
            else if (board[m.startRow][m.startCol] - color == wK)
            {
                if (squareAttacked(m.endRow, m.endCol, color, true, m.startRow, m.startCol) > 0)
                {
                    potentialMoves.RemoveAt(i);
                }
            }
            else if (!isSafe(m, color))
            {
                int k = board[m.startRow][m.startCol] - color;
                potentialMoves.RemoveAt(i);
            }
            else
            {
                int k = board[m.startRow][m.startCol] - color;
            }
        }
        realMoves = potentialMoves;
        return realMoves;
    }

    private List<Move> possibleMoves(int color) {
        List<Move> moves = new List<Move>();
        for (int i = 0; i < 8; ++i) {
            for (int j = 0; j < 8; ++j) {
                moves.AddRange(squareMoves(i, j, color));
            }
        }
        return moves;
    }

    // for moves: color coded as 0 for white, 1 for black
    // pass in starting row, starting col, and color
    // should be the job of whoever called it to handle getting back null

    private List<Move> squareMoves(int row, int col, int color) {
        int square = board[row][col];
        if (square < 0 || square % 2 != color) {
            return new List<Move>();
        }
        if (square == bp || square == wp) {
            return pawnMoves(row, col, color);
        }
        else if (square == bk || square == wk) {
            return knightMoves(row, col, color);
        }
        else if (square == br || square == wr) {
            return rookMoves(row, col, color);
        }
        else if (square == bb || square == wb) {
            return bishopMoves(row, col, color);
        }
        else if (square == bQ || square == wQ) {
            return queenMoves(row, col, color);
        }
        else if (square == bK || square == wK) {
            return kingMoves(row, col, color);
        }
        return new List<Move>();
    }

    private List<Move> pawnMoves(int row, int col, int color) {
        List<Move> moves = new List<Move>();
        if (color == white) {
            //buggy on promotion
            if (board[row-1][col] < 0) {
                moves.Add(new Move(row, col, row-1, col));
                //checks on pawns first move only
                if (row == 6) {
                    if (board[row-2][col] < 0) {
                        moves.Add(new Move(row, col, row-2, col));
                    }
                }
            }
        }
        else {
            //buggy on promotion
            if (board[row+1][col] < 0) { //error was here
                moves.Add(new Move(row, col, row+1, col));
                //checks on pawns first move only
                if (row == 1) {
                    if (board[row+2][col] < 0) {
                        moves.Add(new Move(row, col, row+2, col));
                    }
                }
            }
        }
        moves.AddRange(pawnCaptures(row, col, color));
        return moves;
    }

    //en passant not yet handled
    private List<Move> pawnCaptures(int row, int col, int color) {
        //if it's on the edge, dont go out of bounds
        List<Move> moves = new List<Move>();
        int delta;
        if (color == white) {
            delta = -1;
        }
        else {
            delta = 1;
        }
        if (col == 0) {
            if (board[row+delta][col+1] >= 0 && board[row+delta][col+1] % 2 != color) 
            {
                moves.Add(new Move(row, col, row+delta, col+1));
            }
            if (color == white && row == 3 && epCol == 1)
            {
                moves.Add(new Move(row, col, row + delta, 1, true));
            }
            if (color == black && row == 4 && epCol == 1)
            {
                moves.Add(new Move(row, col, row + delta, 1, true));
            }
        }
        else if (col == 7) {
            if (board[row+delta][col-1] >= 0 && board[row+delta][col-1] % 2 != color) 
            {
                moves.Add(new Move(row, col, row+delta, col-1));
            }
            if (color == white && row == 3 && epCol == 6)
            {
                moves.Add(new Move(row, col, row + delta, 6, true));
            }
            if (color == black && row == 4 && epCol == 6)
            {
                moves.Add(new Move(row, col, row + delta, 6, true));
            }
        }
        else {
            if (board[row+delta][col-1] >= 0 && board[row+delta][col-1] % 2 != color) 
            {
                moves.Add(new Move(row, col, row+delta, col-1));
            }
            if (board[row+delta][col+1] >= 0 && board[row+delta][col+1] % 2 != color) 
            {
                moves.Add(new Move(row, col, row+delta, col+1));
            }
            if (color == white && row == 3 && epCol == col + 1)
            {
                moves.Add(new Move(row, col, row + delta, col + 1, true));
            }
            if (color == white && row == 3 && epCol == col - 1)
            {
                moves.Add(new Move(row, col, row + delta, col - 1, true));
            }
            if (color == black && row == 4 && epCol == col + 1)
            {
                moves.Add(new Move(row, col, row + delta, col + 1, true));
            }
            if (color == black && row == 4 && epCol == col - 1)
            {
                moves.Add(new Move(row, col, row + delta, col - 1, true));
            }
        }
        return moves;
    }

    private List<Move> knightMoves(int row, int col, int color) {
        List<Move> moves = new List<Move>();
        int newRow;
        int newCol;
        foreach (int[] dir in knightPossiblities) {
            newRow = row + dir[0];
            newCol = col + dir[1];
            //check if its on the board, and then if its not occupied by the same color piece
            if (newRow < 8 && newRow >= 0 && newCol < 8 && newCol >= 0 && 
                (board[newRow][newCol] < 0 || board[newRow][newCol] % 2 != color)) {
                moves.Add(new Move(row, col, newRow, newCol));
            }
        }
        return moves;
    }

    private List<Move> bishopMoves(int row, int col, int color) {
        List<Move> moves = new List<Move>();
        int newRow;
        int newCol;
        //for each direction we can go, keep adding moves until 
        //1. we hit one of our pieces,  don't add that square
        //2. we hit an enemy piece,     do add that square
        foreach (int[] dir in bishopDirs) {
            newRow = row + dir[0];
            newCol = col + dir[1];
            while (newRow < 8 && newRow >= 0 && newCol < 8 && newCol >= 0) {
                if (board[newRow][newCol] < 0) {
                    moves.Add(new Move(row, col, newRow, newCol));
                    newRow = newRow + dir[0];
                    newCol = newCol + dir[1];
                }
                else if (board[newRow][newCol] % 2 != color) {
                    moves.Add(new Move(row, col, newRow, newCol));
                    break;
                }
                else {
                    break;
                }
            }
        }
        return moves;
    }

    //castling is technically a king move so not needed here
    private List<Move> rookMoves(int row, int col, int color) {
        List<Move> moves = new List<Move>();
        int newRow;
        int newCol;
        //for each direction we can go, keep adding moves until 
        //1. we hit one of our pieces,  don't add that square
        //2. we hit an enemy piece,     do add that square

        foreach (int[] dir in rookDirs) {
            newRow = row + dir[0];
            newCol = col + dir[1];
            while (newRow < 8 && newRow >= 0 && newCol < 8 && newCol >= 0) {
                if (board[newRow][newCol] < 0) {
                    moves.Add(new Move(row, col, newRow, newCol));
                    newRow = newRow + dir[0];
                    newCol = newCol + dir[1];
                }
                else if (board[newRow][newCol] % 2 != color) {
                    moves.Add(new Move(row, col, newRow, newCol));
                    break;
                }
                else {
                    break;
                }
            }
        }
        return moves;
    }

    private List<Move> queenMoves(int row, int col, int color) {
        List<Move> moves = new List<Move>();
        moves.AddRange(bishopMoves(row, col, color));
        moves.AddRange(rookMoves(row, col, color));
        return moves;
    }

    //possibly a terrible idea
    private List<Move> kingMoves(int row, int col, int color) {
        List<Move> moves = new List<Move>();
        int newRow;
        int newCol;
        foreach (int[] dir in bishopDirs) {
            newRow = row + dir[0];
            newCol = col + dir[1];
            if (newRow < 8 && newRow >= 0 && newCol < 8 && newCol >= 0) {
                if (board[newRow][newCol] < 0 || board[newRow][newCol] % 2 != color) {
                    moves.Add(new Move(row, col, newRow, newCol));
                }
            }
        }
        foreach (int[] dir in rookDirs) {
            newRow = row + dir[0];
            newCol = col + dir[1];
            if (newRow < 8 && newRow >= 0 && newCol < 8 && newCol >= 0) {
                if (board[newRow][newCol] < 0 || board[newRow][newCol] % 2 != color) {
                    moves.Add(new Move(row, col, newRow, newCol));
                }
            }
        }
        //if start row is negative, white is castling
        //if start col is negative, black is castling
        //if end row is negative, it is queenside
        //if end col is negative, it is kingside
        //white
        if (color == 0) {
            if (whiteCanCastleKS && board[7][1] < 0 && board[7][2] < 0 && board[7][0] == wr) {
                moves.Add(new Move(-1, 0, 0, -1));
            }
            if (whiteCanCastleQS && board[7][4] < 0 && board[7][5] < 0 && board[7][6] < 0 && board[7][7] == wr) {
                moves.Add(new Move(-1, 0, -1, 0));
            }
        }
        //black
        if (color == 1) {
            if (blackCanCastleKS && board[0][1] < 0 && board[0][2] < 0 && board[0][0] == br) {
                moves.Add(new Move(0, -1, 0, -1));
            }
            if (blackCanCastleQS && board[0][4] < 0 && board[0][5] < 0 && board[0][6] < 0 && board[0][7] == br) {
                moves.Add(new Move(0, -1, -1, 0));
            }
        }
        return moves;
    }

    //mostly to parse moves that would come in from ChessUI
    private bool isCastling(int row, int col, int newRow, int newCol, int color)
    {
        if (color == white)
        {
            if (row == 7 && col == 3 && newRow == 7 && newCol == 1 && wkRow == 7 && wkCol == 3)
            {
                castleType = new Move(-1, 0, 0, -1);
                return true;
            }
            if (row == 7 && col == 3 && newRow == 7 && newCol == 5 && wkRow == 7 && wkCol == 3)
            {
                castleType = new Move(-1, 0, -1, 0);
                return true;
            }
        }
        else
        {
            if (row == 0 && col == 3 && newRow == 0 && newCol == 1 && bkRow == 0 && bkCol == 3)
            {
                castleType = new Move(0, -1, 0, -1);
                return true;
            }
            if (row == 0 && col == 3 && newRow == 0 && newCol == 5 && bkRow == 0 && bkCol == 3)
            {
                castleType = new Move(0, -1, -1, 0);
                return true;
            }
        }
        return false;
    }

    //make castling illegal if king is moved
    private bool doKingMoveUpdates(int newRow, int newCol, int color)
    {
        if (color == white)
        {
            wkRow = newRow;
            wkCol = newCol;
            whiteCanCastleKS = false;
            whiteCanCastleQS = false;
        }
        else
        {
            bkRow = newRow;
            bkCol = newCol;
            blackCanCastleKS = false;
            blackCanCastleQS = false;
        }
        return true;
    }

    //if a corener piece is moved, set a possible castling to be invalid
    private void doCornerUpdates(int row, int col)
    {
        if (row == 7)
        {
            if (col == 0)
            {
                whiteCanCastleKS = false;
            }
            else if (col == 7)
            {
                whiteCanCastleQS = false;
            }
        }
        else if (row == 0)
        {
            if (col == 0)
            {
                blackCanCastleKS = false;
            }
            else if (col == 7)
            {
                blackCanCastleQS = false;
            }
        }
    }

    private bool tryCastling(Move m)
    {
        if (m.startRow == -1 && m.startCol != -1 && isCastlingSafe(m))
        {
            if (m.endCol == -1 && whiteCanCastleKS)
            {
                board[7][3] = -1;
                board[7][0] = -1;
                board[7][1] = wK;
                board[7][2] = wr;
                wkCol = 1;
                whiteCanCastleKS = false;
                whiteCanCastleQS = false;
                whiteHasCastled = true;
                return true;
            }
            if (m.endRow == -1 && whiteCanCastleQS)
            {
                board[7][3] = -1;
                board[7][7] = -1;
                board[7][5] = wK;
                board[7][4] = wr;
                wkCol = 5;
                whiteCanCastleKS = false;
                whiteCanCastleQS = false;
                whiteHasCastled = true;
                return true;
            }
        }
        if (m.startCol == -1 && m.startRow != -1 && isCastlingSafe(m))
        {
            if (m.endCol == -1 && blackCanCastleKS)
            {
                board[0][3] = -1;
                board[0][0] = -1;
                board[0][1] = bK;
                board[0][2] = br;
                bkCol = 1;
                blackCanCastleKS = false;
                blackCanCastleQS = false;
                blackHasCastled = true;
                return true;
            }
            if (m.endRow == -1 && blackCanCastleQS)
            {
                board[0][3] = -1;
                board[0][7] = -1;
                board[0][5] = bK;
                board[0][4] = br;
                bkCol = 5;
                blackCanCastleKS = false;
                blackCanCastleQS = false;
                blackHasCastled = true;
                return true;
            }
        }
        return false;
    }

    private bool isCastlingSafe(Move m)
    {
        if (m.startRow < 0)
        {
            if (m.endRow < 0)
            {
                return squareAttacked(7, 3, white) + squareAttacked(7, 4, white, true, 7, 3) + squareAttacked(7, 5, white, true, 7, 3) <= 0;
            }
            else if (m.endCol < 0)
            {
                return squareAttacked(7, 1, white, true, 7, 3) + squareAttacked(7, 2, white, true, 7, 3) + squareAttacked(7, 3, white) <= 0;
            }
        }
        else if (m.startCol < 0)
        {
            if (m.endRow < 0)
            {
                return squareAttacked(0, 3, black) + squareAttacked(0, 4, black, true, 0, 3) + squareAttacked(0, 5, black, true, 0, 3) <= 0;
            }
            else if (m.endCol < 0)
            {
                return squareAttacked(0, 1, black, true, 0, 3) + squareAttacked(0, 2, black, true, 0, 3) + squareAttacked(0, 3, black) <= 0;
            }
        }
        //should never get here as this function should only be called
        //with an attempted castle
        return false;
    }

    //determine if square can be attacked by opposite color of what is passed in
    //return num of attackers
    private int squareAttacked(int row, int col, int color, bool testForNextMoveSafety=false, int oldRow=-1, int oldCol=-1)
    {
        int revertPiece1 = -1;
        int revertPiece2 = -1;
        if (testForNextMoveSafety)
        {
            revertPiece1 = board[row][col];
            revertPiece2 = board[oldRow][oldCol];
            board[row][col] = placeHolderPiece + color;
            board[oldRow][oldCol] = ee;
        }
        int count = 0;
        for (int i = 0; i < 8; ++i)
        {
            for (int j = 0; j < 8; ++j)
            {
                if (board[i][j] >= 0 && board[i][j] % 2 != color)
                {
                    List<Move> attackerMoves = squareMoves(i, j, (color + 1) % 2);

                    for (int k = 0; k < attackerMoves.Count; ++k)
                    {
                        if (attackerMoves[k].endRow == row && attackerMoves[k].endCol == col)
                        {
                            ++count;
                        }
                    }
                }
            }
        }
        if (testForNextMoveSafety)
        {
            board[row][col] = revertPiece1;
            board[oldRow][oldCol] = revertPiece2;
        }
        return count;
    }

    //return a copy of the current board array
    private int[][] getBoardCopy()
    {
        int[][] boardCopy = new int[8][];
        for (int i = 0; i < 8; ++i)
        {
            boardCopy[i] = new int[8];
            for (int j = 0; j < 8; ++j)
            {
                boardCopy[i][j] = board[i][j];
            }
        }
        return boardCopy;
    }

    //should NOT be used for king moves (that means dont use it for castling either)
    private bool isSafe(Move m, int color)
    {
        int[][] boardCopy = getBoardCopy();
        board[m.endRow][m.endCol] = boardCopy[m.startRow][m.startCol];
        board[m.startRow][m.startCol] = ee;
        bool validity;
        if (m.isEP)
        {
            board[m.startRow][epCol] = ee;
        }
        if (color == white)
        {
            validity = !speedyCheck(color);
        }
        else
        {
            validity = !speedyCheck(color);

        }

        board = boardCopy;
        return validity;
    }

    private static bool moveMatch(Move m1, Move m2)
    {
        return m1.startRow == m2.startRow && m1.endRow == m2.endRow && 
            m1.startCol == m2.startCol && m1.endCol == m2.endCol && m1.isEP == m2.isEP;
    }

    //only here for debugging
    public static string moveStr(Move m)
    {
        return "" + m.startRow + " " + m.startCol + "->" + m.endRow + " " + m.endCol;
    }

    public string boardStr()
    {
        string s = "\n";
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i][j] == ee)
                {
                    s += "_";
                }
                else 
                    s += (board[i][j]%2).ToString();
            }
            s+= "\n";
        }
        return s;
    }

 }
