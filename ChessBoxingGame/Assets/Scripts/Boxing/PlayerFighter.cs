using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFighter : Fighter
{
    private Animator anim;
    public string stateName; // Debug only
    [SerializeField] Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    private static int stateNumOffset = 7;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        healthMax = Constants.Player.HEALTH_MAX;
        currentHealth = healthMax;
        blockingReduction = Constants.Player.BLOCKING_REDUC;
        currentState = new PlayerIdle(anim, transform, this);
        lightPunch = new Attack(Constants.Player.LIGHT_PUNCH_DAMAGE, Constants.Player.LIGHT_PUNCH_TELE_TIME, true);
        heavyPunch = new Attack(Constants.Player.HEAVY_PUNCH_DAMAGE,
            Constants.Enemy.HEAVY_PUNCH_FST_TELE_TIME + Constants.Enemy.HEAVY_PUNCH_SND_TELE_TIME, false);
    }

    private void Update()
    {
        stateName = currentState.name.ToString();
        spriteRenderer.sprite = sprites[(int)currentState.name - stateNumOffset];
        if (Input.GetKeyDown("f"))
        {
            Punch();
        }
        else if (Input.GetKeyDown("j"))
        {
            Dodge();
        }
        else if (Input.GetKey("space"))
        {
            Block();
        }
        else
        {
            EndBlock();
        }
        currentState = currentState.process();
    }

    void Punch()
    {
        Debug.Log("entering punch");
        if (currentState.name == State.STATE.P_IDLE)
        {
            Debug.Log("punch");
            ((PlayerIdle)currentState).goPunching();
        }
        else if (currentState.name == State.STATE.P_BLOCKING)
        {
            ((PlayerBlocking)currentState).goPunching();
        }
    }

    void Block()
    {
        if (currentState.name == State.STATE.P_IDLE)
        {
            ((PlayerIdle)currentState).goBlocking();
        }
    }

    void EndBlock()
    {
        if (currentState.name == State.STATE.P_BLOCKING)
        {
            ((PlayerBlocking)currentState).goIdle();
        }
    }

    void Dodge()
    {
        if (currentState.name == State.STATE.P_IDLE)
        {
            ((PlayerIdle)currentState).goDodging1();
        }
        else if (currentState.name == State.STATE.P_BLOCKING)
        {
            ((PlayerBlocking)currentState).goDodging1();
        }
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

    

}
