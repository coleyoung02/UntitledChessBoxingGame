using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerClass : MonoBehaviour
{
    public static GameManagerClass Instance { get; private set; }
    private float playerHealth;
    private float enemyHealth;
    private ChessState chessState;
    private float playerChessTime;
    private float enemyChessTime;

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



    [SerializeField] private AudioSource music;

    protected void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            initFields();
        }
        //Instance.music.volume = .1f;
        //Instance.music.Play();
    }

    private void initFields()
    {
        playerHealth = Constants.Player.HEALTH_MAX;
        enemyHealth = Constants.Enemy.HEALTH_MAX;
        chessState = new ChessState();
        playerChessTime = Constants.chessTime;
        enemyChessTime = Constants.chessTime;
    }
}
