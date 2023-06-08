using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    private GameManagerClass gameManager;
    [SerializeField] private GameObject win;
    [SerializeField] private GameObject lose;
    [SerializeField] private GameObject tie;

    void Start()
    {
        gameManager = Resources.FindObjectsOfTypeAll<GameManagerClass>()[0];
        win.SetActive(false);
        lose.SetActive(false);
        tie.SetActive(false);
        if (gameManager.getWinner() == GameManagerClass.Winner.PLAYER)
        {
            win.SetActive(true);
        }
        else if (gameManager.getWinner() == GameManagerClass.Winner.ENEMY)
        {
            lose.SetActive(true);
        }
        else if (gameManager.getWinner() == GameManagerClass.Winner.DRAW)
        {
            tie.SetActive(true);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void NewGame()
    {
        gameManager.ResetGame();
        SceneManager.LoadScene("ChessScene");
    }

    public void MainMenu()
    {
        gameManager.ResetGame();
        SceneManager.LoadScene("TitleScreen");
    }
}
