using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

public class ChessAI
{

    private static int mateScore = 32000;
    private static int baseSearch = 3;
    private const int baseBaseSearch = 4;
    private int searchDepth = 2;
    private List<Move> bestMoves = new List<Move>();
    private int count = 0;

    private const float pawnScore = 1f;
    private const float pawnDistanceMultiplier = .05f;
    private const float pawnChainScore = .1f;
    private const float doubledPawnPenalty = -.08f;
    private const float pawnAttackBonus = 1.5f;


    private const float bishopScore = 3.4f;
    private const float diagonalnessMultiplier = .1f;


    private const float knightScore = 2.9f;
    private const float sidePenalty = .1f;

    private const float rookScore = 5f;
    private const float linkedBonus = .3f;


    private const float queenScore = 12f;

    private const float kingScore = 0f;
    private const float edgenessMuliplier = .2f;

    private static System.Random rng = new System.Random();

    public static void Shuffle<Move>(IList<Move> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Move value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public int getBaseDepth()
    {
        return baseBaseSearch;
    }

    public void setDepth(int depth)
    {
        baseSearch = depth;
    }

    private float scorePawn(int[][] board, int row, int col, int color)
    {
        float distanceScore;
        int delta = -(color * 2 - 1);
        bool supported = false;
        if (color == ChessState.white)
        {
            distanceScore = (6 - row) * pawnDistanceMultiplier;
        }
        else
        {
            distanceScore = (row - 2) * pawnDistanceMultiplier;
        }
        for (int i = 0; i < 8; ++i)
        {
            if (i != row && board[i][col] == ChessState.wp)
            {
                distanceScore -= doubledPawnPenalty;
            }
        }
        if (col - 1 >= 0 && board[row + delta][col - 1] == ChessState.wp + color)
        {
            distanceScore += pawnChainScore;
            supported = true;
        }
        if (col + 1 <= 7 && board[row + delta][col + 1] == ChessState.wp + color)
        {
            distanceScore += pawnChainScore;
            supported = true;
        }
        if (supported && col - 1 >= 0 && board[row - delta][col - 1] >= ChessState.wk && board[row - delta][col - 1] % 2 != color)
        {
            distanceScore += pawnAttackBonus;
        }
        if (supported && col + 1 <= 7 && board[row - delta][col + 1] >= ChessState.wk && board[row - delta][col + 1] % 2 != color)
        {
            distanceScore += pawnAttackBonus;
        }
        return pawnScore + distanceScore;
    }

    private float scoreKnight(int row, int col)
    {
        float increment = 0;
        if (row > 1 && row < 6)
        {
            increment = sidePenalty * 2;
        }
        if (col == 0 || col == 7)
        {
            return knightScore - (sidePenalty * 2) + increment;
        }
        if (col == 1 || col == 6)
        {
            return knightScore - sidePenalty + increment;
        }
        return knightScore + increment;
    }

    private float scoreBishop(int row, int col)
    {
        {
            int centerness = Math.Min(Math.Abs(col - row), Math.Abs(col - (7 - row)));
            if (centerness < 3)
            {
                return bishopScore + diagonalnessMultiplier * (3 - centerness);
            }
            return bishopScore;
        };
    }

    private float scoreRook(int[][] board, int row, int col, int color)
    {
        for (int i = col + 1; i < 8; ++i)
        {
            if (board[row][i] == ChessState.wr + color)
            {
                return rookScore + linkedBonus;
            }
            else if (board[row][i] != ChessState.ee)
            {
                break;
            }
        }
        for (int i = col - 1; i >= 0; --i)
        {
            if (board[row][i] == ChessState.wr + color)
            {
                return rookScore + linkedBonus;
            }
            else if (chess.getBoard()[row][i] != ChessState.ee)
            {
                break;
            }
        }
        for (int i = row + 1; i < 8; ++i)
        {
            if (board[i][col] == ChessState.wr + color)
            {
                return rookScore + linkedBonus;
            }
            else if (board[row][i] != ChessState.ee)
            {
                break;
            }
        }
        for (int i = row - 1; i >= 0; --i)
        {
            if (board[i][col] == ChessState.wr + color)
            {
                return rookScore + linkedBonus;
            }
            else if (board[i][col] != ChessState.ee)
            {
                break;
            }
        }
        return rookScore;
    }

    private float scoreKing(ChessState state, int row, int col)
    {
        int edgeness = 0;
        if (state.getHalfMoves() > 25 && piecesOnBoard(state) < 6)
        {
            if (row == 7 || row == 0)
            {
                edgeness += 3;
            }
            else if (row == 6 || row == 1)
            {
                edgeness += 2;
            }
            else if (row == 5 || row == 2)
            {
                edgeness += 1;
            }
            if (col == 7 || col == 0)
            {
                edgeness += 3;
            }
            else if (col == 6 || col == 1)
            {
                edgeness += 2;
            }
            else if (col == 5 || col == 2)
            {
                edgeness += 1;
            }
            return -edgeness * edgenessMuliplier;
        }
        else if (row == 7 && col == 1)
        {
            return .3f;
        }
        else if (row == 0 && col == 1)
        {
            return .3f;
        }
        else if (row == 7 && col == 1)
        {
            return .3f;
        }
        else if (row == 0 && col == 5)
        {
            return .3f;
        }
        return 0f;
    }

    private float pieceScore(ChessState state, int row, int col)
    {
        int[][] board = state.getBoard();
        int piece = board[row][col];
        switch (piece)
        {
            case ChessState.ee: return 0;
            case ChessState.wp: return scorePawn(board, row, col, ChessState.white);
            case ChessState.bp: return -scorePawn(board, row, col, ChessState.black);
            case ChessState.wk: return scoreKnight(row, col);
            case ChessState.bk: return -scoreKnight(row, col);
            case ChessState.wb: return scoreBishop(row, col);
            case ChessState.bb: return -scoreBishop(row, col);
            case ChessState.wr: return scoreRook(board, row, col, ChessState.white);
            case ChessState.br: return -scoreRook(board, row, col, ChessState.black);
            case ChessState.wQ: return queenScore;
            case ChessState.bQ: return -queenScore;
            case ChessState.wK: return scoreKing(state, row, col);
            case ChessState.bK: return -scoreKing(state, row, col);
        }
        return 0;
    }
    

    public ChessState chess;


    public ChessAI(ChessState chess)
    {
        this.chess = chess;
    }

    public Move GetBestMove(ChessState chess, float whiteTime, float blackTime)
    {
        Move openable = doOpening();
        if (!(openable.startCol == -2))
        {
            return openable;
        }
        if (piecesOnBoard(chess) < 7)
        {
            searchDepth = baseSearch + 1;
        }
        else if (piecesOnBoard(chess) < 4)
        {
            searchDepth = baseSearch + 2;
        }
        else
        {
            searchDepth = baseSearch;
        }
        if (chess.getLegalMoves().Count < 4)
        {
            searchDepth += 1;
        }
        if (chess.getHalfMoves() < 6)
        {
            searchDepth = Math.Min(searchDepth, 2);
        }
        else if (chess.getHalfMoves() < 25)
        {
            searchDepth = Math.Min(searchDepth, 3);
        }
        else if (chess.getHalfMoves() < 50)
        {
            searchDepth = Math.Min(searchDepth, 4);
        }
        if (whiteTime < 15)
        {
            searchDepth = Math.Min(searchDepth, 3);
        }
        else if (whiteTime < 8)
        {
            searchDepth = Math.Min(searchDepth, 2);
        }
        if (whiteTime - blackTime > 30)
        {
            searchDepth = Math.Min(searchDepth, 2);
        }
        else if (whiteTime - blackTime > 15)
        {
            searchDepth = Math.Min(searchDepth, 3);
        }
        bestMoves = new List<Move>();
        //MinMax(chess, searchDepth);
        UnityEngine.Debug.Log(" Depth of " + searchDepth);
        negaMaxAB(chess, searchDepth, -(chess.getColor() * 2 - 1), -mateScore, mateScore);
        System.Random rnd = new System.Random();
        int j = rnd.Next(0, bestMoves.Count);
        UnityEngine.Debug.Log(count);
        count = 0;
        openable = bestMoves[j];
        return openable;
    }
    

    private float negaMaxAB(ChessState state, int depth, int multiplier, float a, float b, bool q=false)
    {
        count++;
        List<Move> possible = state.getLegalMoves();
        float bestMaxScore;
        float maxScore;
        ChessState result;
        int piece = -1;
        int adjustment = 0;
        if (depth == 0)
        {
            return multiplier * getMaterialScore(state);
        }
        else if (getMaterialScore(state) > 100)
        {
            return multiplier * getMaterialScore(state);
        }
        else if (state.isStale())
        {
            return 0;
        }
        bestMaxScore = -mateScore;
        List<Move> first = new List<Move>();
        List<Move> second = new List<Move>();
        List<Move> third = new List<Move>();
        for (int i = 0; i < possible.Count; ++i)
        {
            if (possible[i].endRow >= 0 && possible[i].endCol >= 0 && state.getBoard()[possible[i].endRow][possible[i].endCol] >= 2)
            {
                first.Add(possible[i]);
            }
            else if (possible[i].endRow >= 0 && possible[i].endCol >= 0 && state.getBoard()[possible[i].endRow][possible[i].endCol] >= 0)
            {
                second.Add(possible[i]);
            }
            else
            {
                third.Add(possible[i]);
            }
        }
        if (!q)
        {
            first.AddRange(second);
            first.AddRange(third);
        }
        if (q && first.Count <=0)
        {
            return multiplier * getMaterialScore(state);
        }
        for (int i = 0; i < first.Count; ++i)
        { 
            if (first[i].startRow >= 0 && first[i].startCol >=0)
            {
                piece = state.getBoard()[first[i].startRow][first[i].startCol];
                if (state.getHalfMoves() < 4)
                {
                    if (piece == ChessState.wQ || piece == ChessState.bQ)
                    {
                        adjustment = -2;
                    }
                    if (piece == ChessState.wK || piece == ChessState.bK)
                    {
                        adjustment = -3;
                    }
                }
                if (state.getHalfMoves() < 8)
                {
                    if (piece == ChessState.wQ || piece == ChessState.bQ)
                    {
                        adjustment = -1;
                    } 
                }
            }
            else
            {
                adjustment = 2;
            }
            result = new ChessState(state);
            result.playMove(first[i]);
            if (searchDepth < 4 && depth == 1)
            {
                maxScore = -negaMaxAB(result, 1, -multiplier, -b, -a, true);
            }
            else
            {
                maxScore = -negaMaxAB(result, depth - 1, -multiplier, -b, -a);
            }
            maxScore += adjustment;
            if (maxScore > bestMaxScore)
            {
                bestMaxScore = maxScore;
                if (depth == searchDepth)
                {
                    bestMoves = new List<Move> { first[i] };
                }
            }
            else if (maxScore == bestMaxScore)
            {
                if (depth == searchDepth)
                {
                    bestMoves.Add(first[i]);
                }
            }
            if (bestMaxScore > a)
            {
                a = bestMaxScore;
            }
            
            if (a > b)
            {
                break;
            }
            
        }
        return bestMaxScore;
    }


    private float getMaterialScore(ChessState state)
    {
        int isMate = state.isMate();
        if (isMate == ChessState.white)
        {
            return -ChessAI.mateScore;
        } 
        else if (isMate == ChessState.black)
        {
            return ChessAI.mateScore;
        }
        else if (state.isStale())
        {
            return -2;
        }
        int[][] board = state.getBoard();
        float cumScore = 0;
        for (int i = 0; i < 8; ++i)
        {
            for (int j = 0; j < 8; ++j)
            {
                cumScore += pieceScore(state, i, j);
            }
        }
        CastlingRights r = state.GetCastlingRights();
        if (!r.wH)
        {
            if (!r.wQS)
            {
                cumScore -= .1f;
            }
            if (!r.wKS)
            {
                cumScore -= .2f;
            }
        }
        else
        {
            cumScore += .4f;
        }
        if (!r.bH)
        {
            if (!r.bQS)
            {
                cumScore += .1f;
            }
            if (!r.bKS)
            {
                cumScore += .2f;
            }
        }
        else
        {
            cumScore -= .4f;
        }
        return cumScore;
    }


    private Move doOpening()
    {
        System.Random rnd = new System.Random();
        if (chess.getHalfMoves() == 0)
        {
            if (rnd.Next(0, 2) == 0)
                return new Move(6, 3, 4, 3);
            else
                return new Move(6, 4, 4, 4);
        }
        if (chess.getHalfMoves() == 2)
        {
            if (chess.getBoard()[4][3] == ChessState.wp)
            {
                if (chess.getBoard()[3][5] == ChessState.bp)
                {
                    return new Move(7, 1, 5, 2);
                }
            }
            else if (chess.getBoard()[4][4] == ChessState.wp)
            {
                if (chess.getBoard()[3][4] == ChessState.bp || chess.getBoard()[2][2] == ChessState.bk)
                {
                    return new Move(6, 5, 4, 5);
                }
            }
        }
        if (chess.getHalfMoves() == 4)
        {
            if (chess.getBoard()[4][4] == ChessState.wp)
            {
                if (chess.getBoard()[4][5] == ChessState.bp && chess.getBoard()[1][4] == ChessState.ee)
                {
                    return new Move(6, 3, 4, 3);
                }
            }
            else if (chess.getBoard()[4][3] == ChessState.wp)
            {
                if (chess.getBoard()[3][5] == ChessState.bp && chess.getBoard()[2][4] == ChessState.bp)
                {
                    return new Move(6, 4, 4, 4);
                }
            }
        }
        //return -2 on no move found
        return new Move(-2, -2, -2, -2);
    }
    
    private float piecesOnBoard(ChessState state)
    {
        float pieces = 0;
        int[][] board = state.getBoard(); 
        for (int i = 0; i < 8; ++i)
        {
            for (int j = 0; j < 8; ++j)
            {
                if (board[i][j] == ChessState.wp || board[i][j] == ChessState.bp)
                {
                    pieces += .24f;
                }
                else if (board[i][j] == ChessState.wk || board[i][j] == ChessState.bk)
                {
                    pieces += .67f;
                }
                else if (board[i][j] == ChessState.wQ || board[i][j] == ChessState.bQ)
                {
                    pieces += 1.12f;
                }
                else if (board[i][j] != ChessState.ee)
                {
                    pieces += 1f;
                }
            }
        }
        return pieces;
    }
}
