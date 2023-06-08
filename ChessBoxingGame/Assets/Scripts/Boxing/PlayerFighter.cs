using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerFighter : Fighter
{
    private Animator anim;
    public string stateName; // Debug only
    private static int stateNumOffset = 8;


    private bool canPunch;
    private int numPunches;
    private bool waiting;
    private bool canDodge;
    private Coroutine lastPunch;
    private bool punchNext;
    private bool nextHeavy;
    [SerializeField] Image glove; 
    [SerializeField] Image dodge; 

    [SerializeField] private AudioClip hurtNoise;
    [SerializeField] private AudioClip flames;
    private GameManagerClass gameManager;
    private bool gameWon;

    void Start()
    {
        gameWon = false;
        lastPunch = null;
        punchNext = false;
        nextHeavy = false;
        canPunch = true;
        numPunches = 0;
        waiting = false;
        canDodge = true;
        anim = GetComponent<Animator>();
        healthMax = Constants.Player.HEALTH_MAX;
        gameManager = Resources.FindObjectsOfTypeAll<GameManagerClass>()[0];
        currentHealth = gameManager.getPlayerHealth();
        blockingReduction = Constants.Player.BLOCKING_REDUC;
        currentState = new PlayerIdle(anim, transform, this);
        lightPunch = new Attack(Constants.Player.LIGHT_PUNCH_DAMAGE, Constants.Player.LIGHT_PUNCH_TELE_TIME, true);
        heavyPunch = new Attack(Constants.Player.HEAVY_PUNCH_DAMAGE, Constants.Player.HEAVY_PUNCH_TELE_TIME, true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.ResetGame();
            SceneManager.LoadScene("TitleScreen");
        }
        glove.gameObject.SetActive(canPunch);
        dodge.gameObject.SetActive(canDodge);
        stateName = currentState.name.ToString();
        if (currentState.name == State.STATE.P_KO && !gameWon)
        {
            gameWon = true;
            Debug.Log("should only happen once");
            gameManager.setWinner(GameManagerClass.Winner.ENEMY);
            bText.onKO();
        }
        else if (this.opponent.currentState.name == State.STATE.E_KO && !gameWon)
        {
            gameWon = true;
            Debug.Log("should only happen once");
            gameManager.setWinner(GameManagerClass.Winner.PLAYER);
            bText.onKO();
        }
        else if (currentState.name == State.STATE.P_IDLE && punchNext)
        {
            punchNext = false;
            Punch(nextHeavy);
        }
        else
        {
            if (Input.GetKeyDown("c"))
            {
                Punch(false);
            }
            else if (Input.GetKeyDown("x"))
            {
                Punch(true);
            }
            else if (Input.GetKeyDown("z"))
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
        }
        currentState = currentState.process();
    }

    void Punch(bool isHeavy)
    {
        if (currentState.name == State.STATE.P_IDLE || currentState.name == State.STATE.P_DODGING1 || currentState.name == State.STATE.P_DODGING2 || currentState.name == State.STATE.P_BLOCKING)
        {
            if (isHeavy)
            {
                //Debug.Log("punch " + numPunches);
                if (canPunch)
                {
                    playAudioClip(flames);
                    if (currentState.name == State.STATE.P_IDLE)
                    {
                        ((PlayerIdle)currentState).goHeavyPunching();
                    }
                    else if (currentState.name == State.STATE.P_BLOCKING)
                    {
                        ((PlayerBlocking)currentState).goHeavyPunching();
                    }
                    else if (currentState.name == State.STATE.P_DODGING1)
                    {
                        ((PlayerDodging1)currentState).goHeavyPunching();
                    }
                    else
                    {
                        ((PlayerDodging2)currentState).goHeavyPunching();
                    }
                }
                else if (!waiting)
                {
                    waiting = true;
                    StartCoroutine(waitBeforePunching());
                }
            }
            else
            {
                //Debug.Log("punch " + numPunches);
                if (canPunch)
                {
                    if (currentState.name == State.STATE.P_IDLE)
                    {
                        ((PlayerIdle)currentState).goPunching();
                    }
                    else if (currentState.name == State.STATE.P_BLOCKING)
                    {
                        ((PlayerBlocking)currentState).goPunching();
                    }
                    else if (currentState.name == State.STATE.P_DODGING1)
                    {
                        ((PlayerDodging1)currentState).goPunching();
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
           
        }
        else if (currentState.name == State.STATE.P_PUNCHING || currentState.name == State.STATE.P_HEAVYPUNCHING)
        {
            if (numPunches < Constants.Player.COMBO_MAX)
            {
                punchNext = true;
                nextHeavy = isHeavy;
            }
        }
    }

    public IEnumerator waitBeforePunching()
    {
        yield return new WaitForSeconds(Constants.Player.PUNCH_WAIT); //this should not be hard coded
        numPunches = 0;
        canPunch = true;
        waiting = false;
    }

    public IEnumerator incompleteCombo()
    {
        yield return new WaitForSeconds(Constants.Player.PUNCH_WAIT_SHORT); //this should not be hard coded
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
        if (canDodge)
        {
            if (currentState.name == State.STATE.P_IDLE)
            {
                ((PlayerIdle)currentState).goDodging1();
                canDodge = false;
                StartCoroutine(resetDodge());
            }
            else if (currentState.name == State.STATE.P_BLOCKING)
            {
                ((PlayerBlocking)currentState).goDodging1();
                canDodge = false;
                StartCoroutine(resetDodge());
            }
        }
    }

    IEnumerator resetDodge()
    {
        yield return new WaitForSeconds(Constants.Player.DODGE_DELAY);
        canDodge = true;
    }

    public override bool doAttack(Attack attack)
    {
        if (opponent.takeAttack(attack))
        {
            numPunches++;
            if (numPunches >= Constants.Player.COMBO_MAX)
            {
                if (lastPunch != null)
                {
                    StopCoroutine(lastPunch);
                }
                canPunch = false;
                waiting = true;
                StartCoroutine(waitBeforePunching());
            }
            else
            {
                shortDelay();
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

    private void shortDelay()
    {
        if (lastPunch != null)
        {
            StopCoroutine(lastPunch);
            lastPunch = StartCoroutine(incompleteCombo());
        }
        else
        {
            lastPunch = StartCoroutine(incompleteCombo());
        }
    }

    public override bool takeAttack(Attack attack)
    {
        Debug.Log("got punched " + currentState.name);
        if (currentState.name == State.STATE.P_DODGING1)
        {
            return false;
        }
        if (currentState.name == State.STATE.P_PUNCHING)
        {
            ((PlayerPunching)currentState).goIdle();
            canPunch = false;
            if (!waiting)
            {
                StartCoroutine(waitBeforePunching());
            }
        }
        else if (currentState.name == State.STATE.P_HEAVYPUNCHING)
        {
            ((PlayerHeavyPunch)currentState).goIdle();
            canPunch = false;
            if (!waiting)
            {
                StartCoroutine(waitBeforePunching());
            }
        }
        else if (currentState.name == State.STATE.P_IDLE)
        {
            canPunch = false;
            shortDelay();
        }
        else if (currentState.name == State.STATE.P_DODGING2)
        {
            canPunch = false;
            shortDelay();
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
