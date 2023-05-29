using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoxingRound : MonoBehaviour
{
    [SerializeField] private RoundCountdown timer;
    [SerializeField] private PlayerFighter player;
    [SerializeField] private EnemyFighter enemy;
    [SerializeField] private int roundNumber;
    private GameManagerClass gameManager;

    private void Start()
    {
        gameManager = Resources.FindObjectsOfTypeAll<GameManagerClass>()[0];
        Debug.Log("Setting health to " + gameManager.getPlayerHealth());
        player.setHealth(gameManager.getPlayerHealth());
        enemy.setHealth(gameManager.getEnemyHealth());
    }

    public void Win()
    {

    }

    public void Lose()
    {

    }

    public void endRound()
    {
        gameManager.setPlayerHealth(player.getHealth());
        gameManager.setEnemyHealth(enemy.getHealth());
        SceneManager.LoadScene("ChessScene");
    }

    private void Update()
    {
        
    }
}
