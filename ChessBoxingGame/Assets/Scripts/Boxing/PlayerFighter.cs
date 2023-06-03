using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFighter : Fighter
{
    private Animator anim;
    public string stateName; // Debug only
    private static int stateNumOffset = 8;


    private bool canPunch;
    private int numPunches;
    private bool waiting;
    private Coroutine lastPunch;
    [SerializeField] Image glove; 

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip damageNoise;
    [SerializeField] private AudioClip hurtNoise;
    [SerializeField] private AudioClip KONoise;
    private GameManagerClass gameManager;

    void Start()
    {
        lastPunch = null;
        canPunch = true;
        numPunches = 0;
        waiting = false;
        anim = GetComponent<Animator>();
        healthMax = Constants.Player.HEALTH_MAX;
        gameManager = Resources.FindObjectsOfTypeAll<GameManagerClass>()[0];
        currentHealth = gameManager.getPlayerHealth();
        blockingReduction = Constants.Player.BLOCKING_REDUC;
        currentState = new PlayerIdle(anim, transform, this);
        lightPunch = new Attack(Constants.Player.LIGHT_PUNCH_DAMAGE, Constants.Player.LIGHT_PUNCH_TELE_TIME, true);
        heavyPunch = new Attack(Constants.Player.HEAVY_PUNCH_DAMAGE,
            Constants.Enemy.HEAVY_PUNCH_FST_TELE_TIME + Constants.Enemy.HEAVY_PUNCH_SND_TELE_TIME, false);
    }

    private void Update()
    {
        glove.gameObject.SetActive(canPunch); 
        stateName = currentState.name.ToString();
        if (currentState.name == State.STATE.P_KO)
        {
            gameManager.setWinner(GameManagerClass.Winner.ENEMY);
            bText.onKO();
        }
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
        if (currentState.name == State.STATE.P_IDLE || currentState.name == State.STATE.P_DODGING2)
        {
            //Debug.Log("punch " + numPunches);
            if (canPunch)
            {
                playAudioClip(damageNoise);
                if (currentState.name == State.STATE.P_IDLE)
                {
                    ((PlayerIdle)currentState).goPunching();
                }
                else
                {
                    ((PlayerDodging2)currentState).goPunching();
                }
            }
            else if (!waiting)
            {
                waiting = true;
                StartCoroutine(waitBeforePunching());
            }
        }
        else if (currentState.name == State.STATE.P_BLOCKING)
        {
            if (canPunch)
            {
                playAudioClip(damageNoise);
                ((PlayerBlocking)currentState).goPunching();
            }
            else if (!waiting)
            {
                waiting = true;
                StartCoroutine(waitBeforePunching());
            }
        }
    }

    public IEnumerator waitBeforePunching()
    {
        yield return new WaitForSeconds(3f); //this should not be hard coded
        numPunches = 0;
        canPunch = true;
        waiting = false;
    }

    public IEnumerator incompleteCombo()
    {
        yield return new WaitForSeconds(1.5f); //this should not be hard coded
        numPunches = 0;
        canPunch = true;
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
        if (opponent.takeAttack(attack))
        {
            numPunches++;
            if (numPunches >= 3)
            {
                if (lastPunch != null)
                {
                    StopCoroutine(lastPunch);
                }
                canPunch = false;
                waiting = true;
                StartCoroutine(waitBeforePunching());
            }
            else if (lastPunch != null)
            {
                StopCoroutine(lastPunch);
                lastPunch = StartCoroutine(incompleteCombo());
            }
            else
            {
                lastPunch = StartCoroutine(incompleteCombo());
            }
            return true;
        }
        canPunch = false;
        if (!waiting)
        {
            StartCoroutine(waitBeforePunching());
        }
        return false;
    }

    public override bool takeAttack(Attack attack)
    {
        gameManager = Resources.FindObjectsOfTypeAll<GameManagerClass>()[0];
        Debug.Log(gameManager.getPlayerHealth());
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
            playAudioClip(KONoise);
            setHealth(0);
            currentState.goKO();
        }
        else
        {
            playAudioClip(hurtNoise);
            setHealth(currentHealth - damage);
        }
        return unblocked;
    }
    
    protected override void onKO()
    {
        this.round.Lose();
    }

    private void playAudioClip(AudioClip clip)
    {
        this.audioSource.clip = clip;
        audioSource.Play();
    }
        

}
