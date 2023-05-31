using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class ChessAI
{
    private static int pieceScore(int piece)
    {
        switch (piece)
        {
            case ChessState.ee: return 0;
            case ChessState.wp: return 1;
            case ChessState.bp: return -1;
            case ChessState.wk: return 3;
            case ChessState.bk: return -3;
            case ChessState.wb: return 3;
            case ChessState.bb: return -3;
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
        int bestScore = -10000;
        int newScore;
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

    private int getMaterialScore(ChessState state)
    {
        int[][] board = state.getBoard();
        int cumScore = 0;
        for (int i = 0; i < 8; ++i)
        {
            for (int j = 0; j < 8; ++j)
            {
                cumScore += pieceScore(board[i][j]);
            }
        }
        return cumScore;
    }
}
