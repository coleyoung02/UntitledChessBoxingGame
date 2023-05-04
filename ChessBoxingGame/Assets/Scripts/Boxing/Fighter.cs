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
    [SerializeField] private float healthMax;
    [SerializeField] private float currentHealth;
    private Attack heavyPunch;
    private Attack lightPunch;
    [SerializeField] private Fighter opponent;
    [SerializeField] private bool isBlocking;
    [SerializeField] private float blockingReduction; // if blockingReduction is 0.1, the fighter takes only 90% damage when blocking.

    #region getters and setters

    public float HealthMax => healthMax;

    public float CurrentHealth => currentHealth;

    public Attack HeavyPunch => heavyPunch;

    public Attack LightPunch => lightPunch;

    public Fighter Opponent => opponent;

    public bool IsBlocking => isBlocking;

    public float BlockingReduction => blockingReduction;

    #endregion

    public bool doAttack(Attack attack)
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

    private IEnumerator doAttackAfterTelegraph(Attack attack)
    {
        yield return new WaitForSeconds(attack.telegraphTime);
        opponent.takeAttack(attack);
    }

    public void takeAttack(Attack attack)
    {
        // No dodging considered. Player should override this method.
        if (isBlocking && attack.isBlockable)
        {
            currentHealth -= attack.damage * (1 - blockingReduction);
        }
        else
        {
            currentHealth -= attack.damage;
        }
    }

    public void blockUp()
    {
        isBlocking = true;
    }

    public void blockDown()
    {
        isBlocking = false;
    }
}