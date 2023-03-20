using System.Collections;
using System.Collections.Generic;

public struct Move {
    public int startRow;
    public int startCol;
    public int endRow;
    public int endCol;

    public Move(int startRow, int startCol, int endRow, int endCol)
    {
        this.startRow = startRow;
        this.startCol = startCol;
        this.endRow = endRow;
        this.endCol = endCol;
    }
}

public class ChessState
{
    public ChessState() {
        this.board = new int[8][];
        this.init_new_board();
    }

    public void playMove() {

    }

    public List<Move> getLegalMoves() {
        return null;
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
    private const int ee = -1;
    private const int wp = 0;
    private const int bp = 1;
    private const int wk = 2;
    private const int bk = 3;
    private const int wb = 4;
    private const int bb = 5;
    private const int wr = 6;
    private const int br = 7;
    private const int wQ = 8;
    private const int bQ = 9;
    private const int wK = 10;
    private const int bK = 11;
    private static readonly int[][] knightPossiblities = {new int[] {1, 2}, new int[] {2, 1}, new int[] {-1,-2}, new int[] {-2,-1}, 
                                new int[] {-1,2}, new int[] {-2,1}, new int[] {1, -2}, new int[] {2, -1}};
    private static readonly int[][] bishopDirs = {new int[] {1,1}, new int[] {1,-1}, new int[] {-1,1}, new int[] {-1,-1}};
    // board[row][col]
    private bool whiteCanCastleQS;
    private bool blackCanCastleQS;
    private bool whiteCanCastleKS;
    private bool blackCanCastleKS;

    private int[][] board;
    //may be more efficient as a linked list
    private List<Move> legalMoves;
    //relevant for en passant worry about this later
    //row is not relevant as it should always be 5 for white moving and 2 for black moving
    private int epCol;
    

    private void init_new_board() {
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
        board[0][3] = bQ;
        board[7][3] = wQ;
        //kings
        board[0][4] = bK;
        board[7][4] = wK;
    }

    private bool moveIsLegal() {
        return false;
    }

    // 3d array :(
    // array of moves where each move is an array of the form
    // [[starting square] [ending square1, ending square2, ...]]
    // not checking if it is legal, just if the piece would be allowed to move there
    // with no respect to checks
    private List<Move> possibleMoves(int color) {
        return null;
    }

    // for moves: color coded as 0 for white, 1 for black
    // pass in starting row, starting col, and color
    // should be the job of whoever called it to handle getting back null


    private List<Move> pawnMoves(int row, int col, int color) {
        List<Move> moves = new List<Move>();
        if (color == 0) {
            if (board[row+1][col] < 0) {
                moves.Add(new Move(row, col, row+1, col));
                //checks on pawns first move only
                if (row == 1) {
                    if (board[row+2][col] < 0) {
                        moves.Add(new Move(row, col, row+2, col));
                    }
                }
            }
        }
        else {
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
        moves.AddRange(pawnCaptures(row, col, color));
        return moves;
    }

    //en passant not yet handled
    private List<Move> pawnCaptures(int row, int col, int color) {
        //if it's on the edge, dont go out of bounds
        List<Move> moves = new List<Move>();
        int delta;
        if (color == 0) {
            delta = 1;
        }
        else {
            delta = -1;
        }
        if (col == 0) {
            if (board[row+delta][col+1] >= 0 && board[row+delta][col+1] % 2 != color) {
                moves.Add(new Move(row, col, row+delta, col+1));
            }
        }
        else if (col == 7) {
            if (board[row+delta][col-1] >= 0 && board[row+delta][col+1] % 2 != color) {
                moves.Add(new Move(row, col, row+delta, col+1));
            }
        }
        else {
            if (board[row+delta][col-1] >= 0 && board[row+delta][col+1] % 2 != color) {
                moves.Add(new Move(row, col, row+delta, col+1));
            }
            if (board[row+delta][col+1] >= 0 && board[row+delta][col+1] % 2 != color) {
                moves.Add(new Move(row, col, row+delta, col+1));
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
        
        int steps;
        int newRow;
        int newCol;
        //for each direction we can go, keep adding moves until 
        //1. we hit one of our pieces,  don't add that square
        //2. we hit an enemy piece,     do add that square
        foreach (int[] dir in bishopDirs) {
            steps = 1;
            newRow = row + dir[0];
            newCol = col + dir[1];
            while (newRow < 8 && newRow >= 0 && newCol < 8 && newCol >= 0) {

            }
        }
        return null;
    }

}
