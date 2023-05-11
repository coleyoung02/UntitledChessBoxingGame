using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFighter : Fighter
{
    void Punch()
    {
        Debug.Log("punch");
    }
    void Block()
    {
        Debug.Log("block");
    }
    void Dodge()
    {
        Debug.Log("dodge");
    }
    void Update()
    {
        if(Input.GetKeyDown("f"))
        {
            Punch();
        }
        if(Input.GetKeyDown("space"))
        {
            Block();
        }
        if(Input.GetKeyDown("j"))
        {
            Dodge();
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown("f"))
        {
            Punch();
        }
        if (Input.GetKeyDown("space"))
        {
            Block();
        }
        if (Input.GetKeyDown("j"))
        {
            Dodge();
        }


    }

}
