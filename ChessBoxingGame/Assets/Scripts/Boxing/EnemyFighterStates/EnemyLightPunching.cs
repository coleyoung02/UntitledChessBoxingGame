using System;
using System.Collections;
using UnityEngine;

public class EnemyLightPunching : State
{
    private Coroutine coroutine;
    private static readonly int GoLightPunching = Animator.StringToHash("GoLightPunching");

    public EnemyLightPunching(Animator _anim, Transform _player, EnemyFighter _fighter) : base(_anim, _player, _fighter)
    {
        name = STATE.E_LIGHTPUNCHING;
        coroutine = null;
    }

    public override void enter()
    {
        coroutine = fighter.StartCoroutine(startLightPunching());
        base.enter();
        anim.SetBool(GoLightPunching, true);
    }

    public override void update()
    {
        base.update();
    }

    public override void exit()
    {
        base.exit();
        anim.SetBool(GoLightPunching, false);
    }

    public IEnumerator startLightPunching()
    {
        yield return new WaitForSeconds(Constants.Enemy.LIGHT_PUNCH_TELE_TIME);
        if (fighter.currentState == this)
        {
            fighter.doAttack(fighter.LightPunch);
            nextState = new EnemyFakeIdle(anim, player, (EnemyFighter)fighter);
            stage = EVENT.EXIT;
        }
    }

    public override void goKO()
    {
        if (coroutine != null)
        {
            fighter.StopCoroutine(coroutine);
            coroutine = null;
        }
        nextState = new EnemyKO(anim, player, (EnemyFighter)fighter);
        stage = EVENT.EXIT;
    }
}