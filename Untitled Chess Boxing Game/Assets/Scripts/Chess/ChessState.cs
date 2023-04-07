using System.Collections;
using System.Collections.Generic;

public struct Move {
    public int startRow;
    public int startCol;
    public int endRow;
    public int endCol;

    //if start row is negative, white is castling
    //if start col is negative, black is castling
    //if end row is negative, it is queenside
    //if end col is negative, it is kingside
    
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
        this.init_new_board();
    }

    //whoever gets this array should really not touch it
    //maybe i should make a copy
    public int[][] getBoard() {
        return this.board;
    }

    public bool playMove(int row, int col, int newRow, int newCol) {
        List<Move> possible = squareMoves(row, col, black);
        bool canMove = false;
        for (int i = 0; i < possible.Count; i++)
        {
            if (possible[i].startCol == col && possible[i].startRow == row
                && possible[i].endCol == newCol && possible[i].endRow == newRow)
            {
                canMove = true;
            }
        }
        if (canMove)
        {
            board[newRow][newCol] = board[row][col];
            board[row][col] = ee;
            return true;
        }
        return false;
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
    private const int white = 0;
    private const int black = 1;
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
                                new int[] {-1,2}, new int[] {-2,1}, new int[] {1,-2}, new int[] {2,-1}};
    private static readonly int[][] bishopDirs = {new int[] {1,1}, new int[] {1,-1}, new int[] {-1,1}, new int[] {-1,-1}};
    private static readonly int[][] rookDirs = {new int[] {0,1}, new int[] {0,-1}, new int[] {1,0}, new int[] {-1,0}};
    // board[row][col]
    private bool whiteCanCastleQS;
    private bool blackCanCastleQS;
    private bool whiteCanCastleKS;
    private bool blackCanCastleKS;

    private int[][] board;
    //may be more efficient as a linked list
    private List<Move> potentialMoves;
    private List<Move> realMoves;
    //relevant for en passant worry about this later
    //row is not relevant as it should always be 5 for white moving and 2 for black moving
    private int epCol;
    

    private void init_new_board() {
        board = new int[8][];
        for (int i = 0; i < 8; i++) {
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
        board[0][3] = bQ;
        board[7][3] = wQ;
        //kings
        board[0][4] = bK;
        board[7][4] = wK;
    }

    private void onMove(int color) {
        potentialMoves = possibleMoves(color);
        realMoves = legalMoves(color);
        color = (color + 1) % 2;
    }

    private List<Move> legalMoves(int color) {
        return null;
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
        if (color == 0) {
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
        moves.AddRange(pawnCaptures(row, col, color));
        return moves;
    }

    //en passant not yet handled
    private List<Move> pawnCaptures(int row, int col, int color) {
        //if it's on the edge, dont go out of bounds
        List<Move> moves = new List<Move>();
        int delta;
        if (color == 0) {
            delta = -1;
        }
        else {
            delta = 1;
        }
        if (col == 0) {
            if (board[row+delta][col+1] >= 0 && board[row+delta][col+1] % 2 != color) {
                moves.Add(new Move(row, col, row+delta, col+1));
            }
        }
        else if (col == 7) {
            if (board[row+delta][col-1] >= 0 && board[row+delta][col-1] % 2 != color) {
                moves.Add(new Move(row, col, row+delta, col+1));
            }
        }
        else {
            if (board[row+delta][col-1] >= 0 && board[row+delta][col-1] % 2 != color) {
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
            if (whiteCanCastleKS) {
                moves.Add(new Move(-1, 0, 0, -1));
            }
            if (whiteCanCastleQS) {
                moves.Add(new Move(-1, 0, -1, 0));
            }
        }
        //black
        if (color == 1) {
            if (blackCanCastleKS) {
                moves.Add(new Move(0, -1, 0, -1));
            }
            if (blackCanCastleQS) {
                moves.Add(new Move(0, -1, -1, 0));
            }
        }
        return moves;
    }

    private bool isLegal(Move m) {
        return false;
    }
}
