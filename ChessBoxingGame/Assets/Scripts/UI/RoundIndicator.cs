using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundIndicator : MonoBehaviour
{
    [Header("ClockUI References")]
    [SerializeField] private Transform pos1;
    [SerializeField] private Transform pos2;

    // Start is called before the first frame update
    private void Start()
    {
        GameManagerClass gameManager = Resources.FindObjectsOfTypeAll<GameManagerClass>()[0];
        setRounds(gameManager.getRound(), Constants.MAX_ROUNDS);    
    }

    private void Update()
    {
        GameManagerClass gameManager = Resources.FindObjectsOfTypeAll<GameManagerClass>()[0];
        if (gameManager.getRound() != 0) 
        {
            setRounds(gameManager.getRound(), Constants.MAX_ROUNDS);
            this.enabled = false;
        }
        
    }

    private void setRounds(int current, int max)
    {
        if (pos1.GetChild(current).gameObject.activeSelf == false)
        {
            //If not, call changeValue
            changeValue(current, pos1);
        }

        if (pos1.GetChild(max).gameObject.activeSelf == false)
        {
            //If not, call changeValue
            changeValue(max, pos2);
        }
    }

    void changeValue(int val, Transform pos)
    {
        //Disable everything
        foreach (Transform child in pos)
        {
            child.gameObject.SetActive(false);
        }

        //Turn on desired value
        pos.GetChild(val).gameObject.SetActive(true);
    }
}
