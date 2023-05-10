using System;
using System.Collections;
using UnityEngine;

public class EnemyFighter : Fighter
{
    private Animator anim;
    public string stateName; // Debug only

    void Start()
    {
        anim = GetComponent<Animator>();
        healthMax = Constants.Enemy.HEALTH_MAX;
        currentHealth = healthMax;
        blockingReduction = Constants.Enemy.BLOCKING_REDUC;
        currentState = new EnemyIdle(anim, transform, this);
        lightPunch = new Attack(Constants.Enemy.LIGHT_PUNCH_DAMAGE, Constants.Enemy.LIGHT_PUNCH_TELE_TIME, true);
        heavyPunch = new Attack(Constants.Enemy.HEAVY_PUNCH_DAMAGE,
            Constants.Enemy.HEAVY_PUNCH_FST_TELE_TIME + Constants.Enemy.HEAVY_PUNCH_SND_TELE_TIME, false);
    }


    void Update()
    {
        stateName = currentState.name.ToString(); // Debug only
        // random possbility
        
        // Comment it out to use buttons instead
        float rand = UnityEngine.Random.Range(0.0f, 1.0f);
        if (rand < Constants.Enemy.POSS_BLOCKING)
        {
            hitBlocking();
        }else if (rand < Constants.Enemy.POSS_BLOCKING + Constants.Enemy.POSS_LIGHT_PUNCH)
        {
            hitLightPunch();
        }else if (rand < Constants.Enemy.POSS_BLOCKING + Constants.Enemy.POSS_LIGHT_PUNCH + Constants.Enemy.POSS_HEAVY_PUNCH)
        {
            hitHeavyPunch();
        }

        currentState = currentState.process();
    }


    public override void takeAttack(Attack attack)
    {
        float damage = attack.damage;
        if (currentState.name == State.STATE.E_BLOCKING)
        {
            damage *= (1-blockingReduction);
        }
        Debug.Log("EnemyFighter took attack of " + damage);
        
        if (currentHealth - damage <= 0)
        {
            currentHealth = 0;
            currentState.goKO();
        }
        else
        {
            currentHealth -= damage;
            if (currentState.name == State.STATE.E_IDLE)
            {
                ((EnemyIdle)currentState).goStunned();
            }else if (currentState.name == State.STATE.E_HEAVYPUNCHINGFST)
            {
                ((EnemyHeavyPunchingFst)currentState).goStunned();
            }
        }
    }

    public override bool doAttack(Attack attack)
    {
        Debug.Log("EnemyFighter made attack of " + attack.damage);
        return true;
    }

    public void hitBlocking()
    {
        if (currentState.name == State.STATE.E_IDLE)
        {
            ((EnemyIdle)currentState).goBlocking();
        }
    }

    public void hitLightPunch()
    {
        if (currentState.name == State.STATE.E_IDLE)
        {
            ((EnemyIdle)currentState).goLightPunching();
        }
        else if (currentState.name == State.STATE.E_BLOCKING)
        {
            ((EnemyBlocking)currentState).goLightPunching();
        }
    }

    public void hitHeavyPunch()
    {
        if (currentState.name == State.STATE.E_IDLE)
        {
            ((EnemyIdle)currentState).goHeavyPunching();
        }
        else if (currentState.name == State.STATE.E_BLOCKING)
        {
            ((EnemyBlocking)currentState).goHeavyPunching();
        }
    }

    // Debug only
    public void getPunched()
    {
        takeAttack(lightPunch); 
    }
}