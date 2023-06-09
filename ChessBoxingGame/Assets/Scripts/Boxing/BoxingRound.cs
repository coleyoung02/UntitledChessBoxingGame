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
    [SerializeField] GameObject backupManager;
    private GameManagerClass gameManager;

    private void Start()
    {
        gameManager = Resources.FindObjectsOfTypeAll<GameManagerClass>()[0];
        if (gameManager.getChessState() == null)
        {
            gameManager = Instantiate(backupManager, new Vector2(0, 0), Quaternion.identity).GetComponent<GameManagerClass>();
        }
        Debug.Log("Setting player health to " + gameManager.getPlayerHealth());
        player.setHealth(gameManager.getPlayerHealth());
        Debug.Log("Setting enemy health to " + gameManager.getEnemyHealth());
        enemy.setHealth(gameManager.getEnemyHealth());
        enemy.enabled = true;
        player.enabled = true;
    }

    public void Disable()
    {
        enemy.enabled = false;
        player.enabled = false;
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
        gameManager.incrementRound();
        if (gameManager.getRound() <= Constants.MAX_ROUNDS)
        {
            SceneManager.LoadScene("ChessScene");
        }
        else
        {
            if (enemy.getHealth() < player.getHealth()) 
            {
                gameManager.setWinner(GameManagerClass.Winner.PLAYER);
            }
            else if (enemy.getHealth() > player.getHealth())
            {
                gameManager.setWinner(GameManagerClass.Winner.ENEMY);
            }
            else
            {
                gameManager.setWinner(GameManagerClass.Winner.DRAW);
            }
            SceneManager.LoadScene("EndScreen");

        }
    }

    private void Update()
    {
        
    }
}
