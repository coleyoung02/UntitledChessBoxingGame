using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ChessAI
{

    private static int mateScore = 32000;
    private static int baseSearch = 3;
    private static int baseBaseSearch = 3;
    private int searchDepth = 2;
    private List<Move> bestMoves = new List<Move>();
    private int count = 0;


    public int getBaseDepth()
    {
        return baseBaseSearch;
    }

    public void setDepth(int depth)
    {
        baseSearch = depth;
    }
    private float pieceScore(ChessState state, int row, int col)
    {
        int[][] board = state.getBoard();
        int piece = board[row][col];
        switch (piece)
        {
            case ChessState.ee: return 0;
            case ChessState.wp: {
                    for (int i = 0; i < 8; ++i)
                    {
                        if (i != row && board[i][col] == ChessState.wp)
                        {
                            return .7f;
                        }
                    }
                    if (col - 1 >=0 && board[row + 1][col - 1] == ChessState.wp)
                    {
                        return 1.1f;
                    }
                    if (col + 1 <= 7 && board[row + 1][col + 1] == ChessState.wp)
                    {
                        return 1.1f;
                    }
                    return 1;
                };
            case ChessState.bp:
                {
                    for (int i = 0; i < 8; ++i)
                    {
                        if (i != row && board[i][col] == ChessState.bp)
                        {
                            return -.7f;
                        }
                    }
                    if (col - 1 >= 0 && board[row - 1][col - 1] == ChessState.wp)
                    {
                        return 1.1f;
                    }
                    if (col + 1 <= 7 && board[row - 1][col + 1] == ChessState.wp)
                    {
                        return 1.1f;
                    }
                    return -1;
                };
            case ChessState.wk:
                {
                    float increment = 0;
                    if (row > 1 && row < 6)
                    {
                        increment = .3f;
                    }
                    if (col == 0 || col == 7)
                    {
                        return 2.8f + increment;
                    }
                    if (col == 1 || col == 6)
                    {
                        return 2.9f + increment;
                    }
                    return 3f + increment;
                };
            case ChessState.bk:
                {
                    if (col == 0 || col == 7)
                    {
                        return -2.8f;
                    }
                    if (col == 1 || col == 6)
                    {
                        return -2.9f;
                    }
                    return -3;
                };
            case ChessState.wb:
                {
                    int centerness = Math.Min(Math.Abs(col - row), Math.Abs(col - (7 - row)));
                    if (centerness < 3)
                    {
                        UnityEngine.Debug.Log("great position " + (3.3f + .1f * (3 - centerness)));
                        return 3.3f + .1f * (2 - centerness);
                    }
                    return 3.3f;
                };
            case ChessState.bb:
                {
                    int centerness = Math.Min(Math.Abs(col - row), Math.Abs(col - (7 - row)));
                    if (centerness < 3)
                    {
                        return -3.3f - .1f * (2 - centerness);
                    }
                    return -3.3f;
                };
            case ChessState.wr:
                {
                    for (int i = col + 1; i < 8; ++i)
                    {
                        if (chess.getBoard()[row][i] == ChessState.wr)
                        {
                            return 5.3f;
                        }
                        else if (chess.getBoard()[row][i] != ChessState.ee)
                        {
                            break;
                        }
                    }
                    for (int i = col - 1; i >= 0; --i)
                    {
                        if (chess.getBoard()[row][i] == ChessState.wr)
                        {
                            return 5.3f;
                        }
                        else if (chess.getBoard()[row][i] != ChessState.ee)
                        {
                            break;
                        }
                    }
                    for (int i = row + 1; i < 8; ++i)
                    {
                        if (chess.getBoard()[row][i] == ChessState.wr)
                        {
                            return 5.3f;
                        }
                        else if (chess.getBoard()[row][i] != ChessState.ee)
                        {
                            break;
                        }
                    }
                    for (int i = row - 1; i >= 0; --i)
                    {
                        if (chess.getBoard()[i][col] == ChessState.wr)
                        {
                            return 5.3f;
                        }
                        else if (chess.getBoard()[i][col] != ChessState.ee)
                        {
                            break;
                        }
                    }
                    return 5f;
                };
            case ChessState.br:
                {
                    for (int i = col + 1; i < 8; ++i)
                    {
                        if (chess.getBoard()[row][i] == ChessState.br)
                        {
                            return -5.3f;
                        }
                        else if (chess.getBoard()[row][i] != ChessState.ee)
                        {
                            break;
                        }
                    }
                    for (int i = col - 1; i >= 0; --i)
                    {
                        if (chess.getBoard()[row][i] == ChessState.br)
                        {
                            return -5.3f;
                        }
                        else if (chess.getBoard()[row][i] != ChessState.ee)
                        {
                            break;
                        }
                    }
                    for (int i = row + 1; i < 8; ++i)
                    {
                        if (chess.getBoard()[row][i] == ChessState.br)
                        {
                            return -5.3f;
                        }
                        else if (chess.getBoard()[row][i] != ChessState.ee)
                        {
                            break;
                        }
                    }
                    for (int i = row - 1; i >= 0; --i)
                    {
                        if (chess.getBoard()[i][col] == ChessState.br)
                        {
                            return -5.3f;
                        }
                        else if (chess.getBoard()[i][col] != ChessState.ee)
                        {
                            break;
                        }
                    }
                    return -5f;
                };
            case ChessState.wQ: return 9;
            case ChessState.bQ: return -9;
            case ChessState.wK:
                {
                    if (row == 7 && col == 1)
                    {
                        return .3f;
                    }
                    if (row == 7 && col == 5)
                    {
                        return .3f;
                    }
                    return 0f;
                };
            case ChessState.bK:
                {
                    if (row == 0 && col == 1)
                    {
                        return -.3f;
                    }
                    if (row == 0 && col == 5)
                    {
                        return -.3f;
                    }
                    return 0f;
                };
        }
        return 0;
    }
    

    public ChessState chess;


    public ChessAI(ChessState chess)
    {
        this.chess = chess;
    }

    public Move GetBestMove(ChessState chess)
    {
        Move openable = doOpening();
        if (!(openable.startCol == -2))
        {
            return openable;
        }
        if (piecesOnBoard() < 7)
        {
            searchDepth = baseSearch + 1;
        }
        else if (piecesOnBoard() < 4)
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
        bestMoves = new List<Move>();
        //MinMax(chess, searchDepth);
        UnityEngine.Debug.Log(" Depth of " + searchDepth);
        negaMaxAB(chess, searchDepth, -(chess.getColor() * 2 - 1), -mateScore, mateScore);
        System.Random rnd = new System.Random();
        int j = rnd.Next(0, bestMoves.Count);
        UnityEngine.Debug.Log(count);
        count = 0;
        openable = bestMoves[j];
        /* would be for 3 move repitition
        if (openable.startRow > 0 && openable.startCol > 0)
        {
            if (chess.getBoard()[openable.startRow][openable.startCol] == ChessState.wp || chess.getBoard()[openable.startRow][openable.startCol] == ChessState.bp)
            {
                statesToCheck = new Stack<ChessState>();
            }
            else if (chess.getBoard()[openable.endRow][openable.endCol] != ChessState.ee || openable.isEP)
            {
                statesToCheck = new Stack<ChessState>();
            }
        }
        else
        {
            statesToCheck = new Stack<ChessState>();
        }
        statesToCheck.Push((new ChessState(chess)).playWhiteMove(openable));
        */
        return openable;
    }
    
    /*
    private float MinMax(ChessState chess, int depth)
    {
        List<Move> possible = chess.getLegalMoves();
        float bestMaxScore;
        float bestMinScore;
        float maxScore;
        float minScore;
        ChessState result;
        if (depth == 0)
        {
            return getMaterialScore(chess);
        }
        if (chess.getColor() == ChessState.white)
        {
            bestMaxScore = -mateScore;
            for (int i = 0; i < possible.Count; ++i)
            {
                result = new ChessState(chess);
                result.playMove(possible[i]);
                maxScore = MinMax(result, depth - 1);
                if (maxScore == bestMaxScore)
                {
                    if (depth == searchDepth)
                    {
                        bestMoves.Add(possible[i]);
                    }
                }
                else if (maxScore > bestMaxScore)
                {
                    Debug.Log("Max = " + maxScore);
                    bestMaxScore = maxScore;
                    if (depth == searchDepth)
                    {
                        bestMoves = new List<Move> { possible[i] };
                    }
                }
            }
            return bestMaxScore;
        }
        else
        {
            bestMinScore = mateScore;
            for (int i = 0; i < possible.Count; ++i)
            {
                result = new ChessState(chess);
                result.playMove(possible[i]);
                minScore = MinMax(result, depth - 1);
                if (minScore == bestMinScore)
                {
                    if (depth == searchDepth)
                    {
                        bestMoves.Add(possible[i]);
                    }
                }
                else if (minScore < bestMinScore)
                {
                    bestMinScore = minScore;
                    if (depth == searchDepth)
                    {
                        bestMoves = new List<Move> { possible[i] };
                    }
                }
            }
            return bestMinScore;
        }
    }
    */
    /*
    private float negaMax(ChessState state, int depth, int multiplier)
    {
        List<Move> possible = chess.getLegalMoves();
        float bestMaxScore;
        float maxScore;
        ChessState result;

        if (depth == 0)
        {
            return multiplier * getMaterialScore(state);
        }
        bestMaxScore = -mateScore;
        for (int i = 0; i < possible.Count; ++i)
        {
            result = new ChessState(state);
            result.playMove(possible[i]);
            maxScore = -negaMax(result, depth - 1, -multiplier);
            if (maxScore > bestMaxScore)
            {
                bestMaxScore = maxScore;
                if (depth == searchDepth)
                {
                    Debug.Log("new max score " + bestMaxScore + " from depth " + depth + " mv " + ChessState.moveStr(possible[i]));

                    bestMoves = new List<Move> { possible[i] };
                }
            }
            if (maxScore == bestMaxScore)
            {
                if (depth == searchDepth)
                {
                    bestMoves.Add(possible[i]);
                }
            }
        }
        return bestMaxScore;
    }
    */

    private float negaMaxAB(ChessState state, int depth, int multiplier, float a, float b, bool evalFinal=true)
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
        for (int i = 0; i < possible.Count; ++i)
        {
            if (possible[i].endRow >= 0 && possible[i].endCol >= 0 && state.getBoard()[possible[i].endRow][possible[i].endCol] >= 0)
            {
                first.Add(possible[i]);
            }
            else
            {
                second.Add(possible[i]);
            }
        }
        first.AddRange(second);
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
            if (depth == 1 && !evalFinal)
            {
                result.playMove(first[i], false);
            }
            else
            {
                result.playMove(first[i]);
            }
            maxScore = -negaMaxAB(result, depth - 1, -multiplier, -b, -a);
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
            return 0;
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
            cumScore += .1f;
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
            cumScore -= .1f;
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
    
    private float piecesOnBoard()
    {
        float pieces = 0;
        int[][] board = chess.getBoard(); 
        for (int i = 0; i < 8; ++i)
        {
            for (int j = 0; j < 8; ++j)
            {
                if (board[i][j] == ChessState.wp || board[i][j] == ChessState.bp)
                {
                    pieces += .49f;
                }
                else if (board[i][j] == ChessState.wQ || board[i][j] == ChessState.bQ)
                {
                    pieces += 1.52f;
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
