using System;
using System.Collections;
using UnityEngine;

public class EnemyFighter : Fighter
{
    private Animator anim;
    public string stateName; // Debug only
    [SerializeField] Sprite[] sprites; // Delete after testing
    private SpriteRenderer spriteRenderer; // Delete after testing

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        spriteRenderer.sprite = sprites[(int)currentState.name]; //Delete after testing
        // Comment it out to use buttons instead
        float rand = UnityEngine.Random.Range(0.0f, 1.0f); 
        if (rand < Constants.Enemy.POSS_LIGHT_PUNCH)
        {
            hitLightPunch();
        }
        else if (rand < Constants.Enemy.POSS_LIGHT_PUNCH + Constants.Enemy.POSS_HEAVY_PUNCH)
        {
            hitHeavyPunch();
        }

        currentState = currentState.process();
    }


    public override bool takeAttack(Attack attack)
    {
        float damage = attack.damage;
        bool unblocked = true;
        if (currentState.name == State.STATE.E_BLOCKING)
        {
            damage *= (1-blockingReduction);
            unblocked = false;
        }
        if (currentState.name == State.STATE.E_LIGHTPUNCHING)
        {
            return false;
        }


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
            }
            else if (currentState.name == State.STATE.E_HEAVYPUNCHINGFST)
            {
                ((EnemyHeavyPunchingFst)currentState).goStunned();
            }
        }
        return unblocked;
    }

    public override bool doAttack(Attack attack)
    {
        return opponent.takeAttack(attack);
        //return true;
    }
    

    public void hitLightPunch()
    {
        if (currentState.name == State.STATE.E_IDLE)
        {
            ((EnemyIdle)currentState).goLightPunching();
        }
    }

    public void hitHeavyPunch()
    {
        if (currentState.name == State.STATE.E_IDLE)
        {
            ((EnemyIdle)currentState).goHeavyPunching();
        }
    }

    // Debug only
    public void getPunched()
    {
        takeAttack(lightPunch); 
    }

    protected override void onKO()
    {
        this.round.Win();
    }
}