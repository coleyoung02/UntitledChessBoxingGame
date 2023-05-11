using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFighter : Fighter
{

    private void Start()
    {
        heavyPunch = new Attack(5f, .2f, false);
        lightPunch = new Attack(5f, .2f, false);
    }

    void Punch()
    {
        doAttack(lightPunch);
    }
    void Block()
    {
        Debug.Log("block");
    }
    void Dodge()
    {
        Debug.Log("dodge");
    }

    public override bool doAttack(Attack attack)
    {
        return base.doAttack(attack);
    }

    public override void takeAttack(Attack attack)
    {
        return;
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
