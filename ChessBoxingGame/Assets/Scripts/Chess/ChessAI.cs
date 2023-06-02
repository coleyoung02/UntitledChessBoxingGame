﻿using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class ChessAI
{

    private static int mateScore = 32000;
    private static int baseSearch = 3;
    private int searchDepth = 2;
    private List<Move> bestMoves = new List<Move>();
    

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
                    if (col == 0 || col == 7)
                    {
                        return 2.8f;
                    }
                    if (col == 1 || col == 6)
                    {
                        return 2.9f;
                    }
                    return 3f;
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
                    if (col == row || col == 7 - row)
                    {
                        return 3.2f;
                    }
                    return 3;
                };
            case ChessState.bb:
                {
                    if (col == row || col == 7 - row)
                    {
                        return -3.2f;
                    }
                    return -3;
                };
            case ChessState.wr: return 5;
            case ChessState.br: return -5;
            case ChessState.wQ: return 9;
            case ChessState.bQ: return -9;
            case ChessState.wK: return 0;
            case ChessState.bK: return 0;
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
            searchDepth = 3;
        }
        else if (piecesOnBoard() < 4)
        {
            searchDepth = 4;
        }
        else
        {
            searchDepth = baseSearch;
        }
        bestMoves = new List<Move>();
        //MinMax(chess, searchDepth);
        negaMaxAB(chess, searchDepth, -(chess.getColor() * 2 - 1), -mateScore, mateScore);
        System.Random rnd = new System.Random();
        int j = rnd.Next(0, bestMoves.Count);
        Debug.Log("Making move " + j + " of " + bestMoves.Count);
        return bestMoves[j];
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

    private float negaMaxAB(ChessState state, int depth, int multiplier, float a, float b, bool evalFinal=false)
    {
        List<Move> possible = state.getLegalMoves();
        float bestMaxScore;
        float maxScore;
        ChessState result;
        int piece = -1;
        if (depth == 0)
        {
            return multiplier * getMaterialScore(state);
        }
        bestMaxScore = -mateScore;
        for (int i = 0; i < possible.Count; ++i)
        { 
            if (possible[i].startRow >= 0 && possible[i].startCol >=0)
            {
                piece = state.getBoard()[possible[i].startRow][possible[i].startCol];
            }
            result = new ChessState(state);
            result = new ChessState(state);
            if (depth == 1 && !evalFinal)
            {
                result.playMove(possible[i], false);
            }
            else
            {
                result.playMove(possible[i]);
            }
            //Debug.Log("depth = " + depth);
            maxScore = -negaMaxAB(result, depth - 1, -multiplier, -b, -a);
            if (maxScore > bestMaxScore)
            {
                bestMaxScore = maxScore;
                if (depth == searchDepth)
                {
                    bestMoves = new List<Move> { possible[i] };
                }
            }
            else if (maxScore == bestMaxScore)
            {
                if (depth == searchDepth)
                {
                    bestMoves.Add(possible[i]);
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
        if (chess.getColor() == ChessState.white) {
            cumScore -= .5f * state.inCheck(ChessState.white);
        }
        else if (chess.getColor() == ChessState.black)
        {
            cumScore += .5f * state.inCheck(ChessState.black);
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
