using System;
using System.Collections;
using UnityEngine;

public struct Attack
{
    public float damage;
    public float telegraphTime;
    public bool isBlockable;

    public Attack(float damage, float telegraphTime, bool isBlockable)
    {
        this.damage = damage;
        this.telegraphTime = telegraphTime;
        this.isBlockable = isBlockable;
    }
}

public abstract class Fighter : MonoBehaviour
{
    [SerializeField] protected float healthMax;
    [SerializeField] protected float currentHealth;
    protected Attack heavyPunch;
    protected Attack lightPunch;
    [SerializeField] protected Fighter opponent;
    public bool isBlocking;
    public bool isDodging;
    [SerializeField] protected float blockingReduction; // if blockingReduction is 0.1, the fighter takes only 90% damage when blocking.
    public State currentState;


    #region getters and setters

    public float HealthMax => healthMax;

    public float CurrentHealth => currentHealth;

    public Attack HeavyPunch => heavyPunch;

    public Attack LightPunch => lightPunch;

    public Fighter Opponent => opponent;

    public float BlockingReduction => blockingReduction;

    #endregion

    // depreciated and should be overwritten
    public virtual bool doAttack(Attack attack)
    {
        //TODO: How to determine whether attack is successful?
        //Currently, as long as the fighter is not blocking, the attack is successful.
        if (!isBlocking)
        {
            StartCoroutine(doAttackAfterTelegraph(attack));
            return true;
        }
        else
        {
            return false;
        }
    }

    // depreciated
    private IEnumerator doAttackAfterTelegraph(Attack attack)
    {
        yield return new WaitForSeconds(attack.telegraphTime);
        opponent.takeAttack(attack);
    }

    public abstract void takeAttack(Attack attack);

    public void blockUp()
    {
        isBlocking = true;
    }

    public void blockDown()
    {
        isBlocking = false;
    }
}