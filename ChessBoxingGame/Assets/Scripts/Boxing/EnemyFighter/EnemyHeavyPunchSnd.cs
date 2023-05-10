using System;
using System.Collections;
using UnityEngine;

public class EnemyHeavyPunchingSnd : State
{
    public EnemyHeavyPunchingSnd(Animator _anim, Transform _player, EnemyFighter _fighter) : base(_anim, _player,
        _fighter)
    {
        name = STATE.E_HEAVYPUNCHINGSND;
    }

    public override void enter()
    {
        Debug.Log("EnemyHeavyPunchingSnd.enter()");
        fighter.StartCoroutine(startHeavyPunchingSnd());
        base.enter();
    }

    public override void update()
    {
        base.update();
    }

    public override void exit()
    {
        Debug.Log("EnemyHeavyPunchingSnd.exit()");
        base.exit();
    }

    public IEnumerator startHeavyPunchingSnd()
    {
        yield return new WaitForSeconds(Constants.Enemy.HEAVY_PUNCH_SND_TELE_TIME);
        if (fighter.currentState == this)
        {
            fighter.doAttack(fighter.HeavyPunch);
            nextState = new EnemyIdle(anim, player, (EnemyFighter)fighter);
            stage = EVENT.EXIT;
        }
    }

    public override void goKO()
    {
        nextState = new EnemyKO(anim, player, (EnemyFighter)fighter);
        stage = EVENT.EXIT;
    }
}