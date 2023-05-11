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
        return opponent.takeAttack(attack);
    }

    public override bool takeAttack(Attack attack)
    {
        if (currentState.name == State.STATE.P_DODGING1)
        {
            return false;
        }

        float damage = attack.damage;
        bool unblocked = true;
        if (currentState.name == State.STATE.P_BLOCKING)
        {
            damage *= (1 - blockingReduction);
            unblocked = false;
        }
        if (currentHealth - damage <= 0)
        {
            currentHealth = 0;
            currentState.goKO();
        }
        else
        {
            currentHealth -= damage;
        }
        return unblocked;


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
