using System;
using System.Collections;
using UnityEngine;

public class EnemyFighter : Fighter
{
    private Animator anim;
    public string stateName; // Debug only
    [SerializeField] Sprite[] sprites; // Delete after testing
    private SpriteRenderer spriteRenderer; // Delete after testing
    [SerializeField] private AudioClip blockedNoise;
    [SerializeField] private AudioClip heavyDamageNoise;
    [SerializeField] private AudioClip mediumDamageNoise;
    private GameManagerClass gameManager;
    private float time;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        healthMax = Constants.Enemy.HEALTH_MAX;
        gameManager = Resources.FindObjectsOfTypeAll<GameManagerClass>()[0];
        currentHealth = gameManager.getEnemyHealth();
        blockingReduction = Constants.Enemy.BLOCKING_REDUC;
        currentState = new EnemyBlocking(anim, transform, this);
        lightPunch = new Attack(Constants.Enemy.LIGHT_PUNCH_DAMAGE, Constants.Enemy.LIGHT_PUNCH_TELE_TIME, true);
        heavyPunch = new Attack(Constants.Enemy.HEAVY_PUNCH_DAMAGE,
            Constants.Enemy.HEAVY_PUNCH_FST_TELE_TIME + Constants.Enemy.HEAVY_PUNCH_SND_TELE_TIME, false);
        time = 3f;
    }


    void Update()
    {
        stateName = currentState.name.ToString(); // Debug only
        // random possbility
        spriteRenderer.sprite = sprites[(int)currentState.name]; //Delete after testing
        // Comment it out to use buttons instead
        if (time <= 0)
        {
            time = UnityEngine.Random.Range(1.0f, 2.0f) + UnityEngine.Random.Range(1.0f, 2.0f) + UnityEngine.Random.Range(1.0f, 2.0f);
            float rand = UnityEngine.Random.Range(0f, 1.0f);
            if (rand < Constants.Enemy.POSS_LIGHT_PUNCH)
            {
                hitLightPunch();
            }
            else if (rand < Constants.Enemy.POSS_LIGHT_PUNCH + Constants.Enemy.POSS_HEAVY_PUNCH)
            {
                hitHeavyPunch();
            }
            else if (rand < Constants.Enemy.POSS_LIGHT_PUNCH + Constants.Enemy.POSS_HEAVY_PUNCH + Constants.Enemy.POSS_IDLE)
            {
                goIdle();
            }
            else if (rand < Constants.Enemy.POSS_LIGHT_PUNCH + Constants.Enemy.POSS_HEAVY_PUNCH + Constants.Enemy.POSS_IDLE + Constants.Enemy.POSS_COMBO)
            {
                doCombo();
            }
        }
        else if (currentState.name == State.STATE.E_BLOCKING || currentState.name == State.STATE.E_IDLE)
        {
            time -= Time.deltaTime;
        }
        currentState = currentState.process();
    }


    public override bool takeAttack(Attack attack)
    {
        Debug.Log("enemy got punched " + currentState.name);
        float damage = attack.damage;
        bool unblocked = true;
        if (currentState.name == State.STATE.E_BLOCKING)
        {
            damage *= (1-blockingReduction);
            unblocked = false;
        }
        if (currentState.name == State.STATE.E_LIGHTPUNCHING)
        {
            damage *= (1 - blockingReduction);
            unblocked = false;
        }


        if (currentHealth - damage <= 0)
        {
            setHealth(0);
            currentState.goKO();
        }
        else
        {
            setHealth(currentHealth - damage);
            if (currentState.name == State.STATE.E_IDLE)
            {
                ((EnemyIdle)currentState).goStunned();
            }
            else if (currentState.name == State.STATE.E_HEAVYPUNCHINGFST)
            {
                ((EnemyHeavyPunchingFst)currentState).goStunned();
            }
            else if (currentState.name == State.STATE.E_FAKEIDLE)
            {
                ((EnemyFakeIdle)currentState).goStunned();
            }
        }
        if (unblocked)
        {
            if (damage > Constants.Player.LIGHT_PUNCH_DAMAGE + .1)
            {
                playAudioClip(heavyDamageNoise);
            }
            else
            {
                playAudioClip(mediumDamageNoise);
            }
        }
        else
        {
            playAudioClip(blockedNoise);
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

    public void doCombo()
    {
        if (currentState.name == State.STATE.E_IDLE)
        {
            ((EnemyIdle)currentState).goPreCombo();
        }
        else if (currentState.name == State.STATE.E_BLOCKING)
        {
            ((EnemyBlocking)currentState).goPreCombo();
        }
    }

    public void goIdle()
    {
        if (currentState.name == State.STATE.E_BLOCKING)
        {
            ((EnemyBlocking)currentState).goIdle();
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

    private void playAudioClip(AudioClip clip)
    {
        this.audioSource.clip = clip;
        audioSource.Play();
    }
}