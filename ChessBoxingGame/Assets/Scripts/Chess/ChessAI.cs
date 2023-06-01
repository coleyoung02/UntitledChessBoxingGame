using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class ChessAI
{

    private static int mateScore = 32000;
    private static float pieceScore(int piece, int row, int col)
    {
        switch (piece)
        {
            case ChessState.ee: return 0;
            case ChessState.wp: return 1;
            case ChessState.bp: return -1;
            case ChessState.wk:
                {
                    if (col == 0 || col == 7)
                    {
                        return 2.8f;
                    }
                    return 3;
                };
            case ChessState.bk:
                {
                    if (col == 0 || col == 7)
                    {
                        return -2.8f;
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

    public Move GetBest()
    {
        List<Move> possible = chess.getLegalMoves();
        List<Move> highest = new List<Move>();
        ChessState result;
        float bestScore = -10000;
        float newScore;
        for (int k = 0; k < possible.Count; ++k)
        {
            result = new ChessState(chess);
            result.playWhiteMove(possible[k]);
            newScore = getMaterialScore(result);
            if (newScore > bestScore)
            {
                bestScore = newScore;
                highest = new List<Move>();
            }
            if (newScore == bestScore)
            {
                highest.Add(possible[k]);
            }
        }
        System.Random rnd = new System.Random();
        int j = rnd.Next(0, highest.Count);
        return highest[j];
    }

    private float getMaterialScore(ChessState state)
    {
        int[][] board = state.getBoard();
        float cumScore = 0;
        if (state.isMate() == 0)
        {
            return mateScore;
        }
        else if (state.isMate() == 1)
        {
            return -mateScore;
        }
        else if (state.isStale())
        {
            return 0;   
        }
        for (int i = 0; i < 8; ++i)
        {
            for (int j = 0; j < 8; ++j)
            {
                cumScore += pieceScore(board[i][j], i, j);
            }
        }
        return cumScore;
    }
}
