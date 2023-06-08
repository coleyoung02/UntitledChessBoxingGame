using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerClass : MonoBehaviour
{
    public enum Winner
    {
        PLAYER,
        ENEMY,
        DRAW
    }

    public static GameManagerClass Instance { get; private set; }
    [SerializeField] private float playerHealth;
    [SerializeField] private float enemyHealth;
    [SerializeField] private ChessState chessState;
    [SerializeField] private float playerChessTime;
    [SerializeField] private float enemyChessTime;
    private ChessAI chessAI;
    private Winner winner;
    private int round;

    public int getRound()
    {
        return round;
    }

    public void incrementRound()
    {
        round++;
    }

    public void setRound(int r)
    {
        round = r;
    }

    public void setPlayerHealth(float health)
    {
        playerHealth = health;
    }

    public void setEnemyHealth(float health)
    {
        enemyHealth = health;
    }

    public void setPlayerChessTime(float health)
    {
        playerChessTime = health;
    }

    public void setEnemyChessTime(float health)
    {
        enemyChessTime = health;
    }

    public float getPlayerHealth()
    {
        return playerHealth;
    }

    public float getEnemyHealth()
    {
        return enemyHealth;
    }

    public float getPlayerChessTime()
    {
        return playerChessTime;
    }

    public float getEnemyChessTime()
    {
        return enemyChessTime;
    }
    
    public ChessState getChessState()
    {
        return chessState;
    }

    public ChessAI GetChessAI()
    {
        return chessAI;
    }
    
    public void setChessStateAndAI(ChessState state, ChessAI chessAI)
    {
        chessState = state;
        this.chessAI = chessAI;
    }

    public void setWinner(Winner winner)
    {
        this.winner = winner;
    }

    public Winner getWinner()
    {
        return winner;
    }



    [SerializeField] private AudioSource music;
    [SerializeField] private AudioClip chessClip;
    [SerializeField] private AudioClip boxingClip;

    protected void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Destroy Game Manager");
            Debug.Log(this.chessState);
            Debug.Log(Instance.chessState);
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            initFields();
        }
        Instance.music.clip = chessClip;
        Instance.music.Play();
    }

    public void ResetGame()
    {
        Debug.Log("resetting here 1");
        initFields();
    }

    private void initFields()
    {
        Debug.Log("resetting here 2");
        playerHealth = Constants.Player.HEALTH_MAX;
        enemyHealth = Constants.Enemy.HEALTH_MAX;
        chessState = new ChessState();
        Debug.Log(chessState);
        playerChessTime = Constants.chessTime;
        enemyChessTime = Constants.chessTime;
        chessAI = new ChessAI(chessState);
        round = 1;
    }
}
