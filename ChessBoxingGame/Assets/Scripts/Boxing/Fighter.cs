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
    protected float healthMax;
    protected float currentHealth;
    protected Attack heavyPunch;
    protected Attack lightPunch;
    [SerializeField] protected Fighter opponent;
    protected float blockingReduction; // if blockingReduction is 0.1, the fighter takes only 90% damage when blocking.
    public State currentState;


    #region getters and setters

    public float HealthMax => healthMax;

    public float CurrentHealth => currentHealth;

    public Attack HeavyPunch => heavyPunch;

    public Attack LightPunch => lightPunch;

    public Fighter Opponent => opponent;

    public float BlockingReduction => blockingReduction;

    #endregion

    public abstract bool doAttack(Attack attack);

    public string HealthFormatted()
    {
        return currentHealth.ToString("0.0");
    }

    public abstract bool takeAttack(Attack attack);

}