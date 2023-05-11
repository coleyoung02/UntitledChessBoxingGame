using System;
using System.Collections;
using UnityEngine;

public class EnemyLightPunching : State
{
    public EnemyLightPunching(Animator _anim, Transform _player, EnemyFighter _fighter) : base(_anim, _player, _fighter)
    {
        name = STATE.E_LIGHTPUNCHING;
    }

    public override void enter()
    {
        Debug.Log("EnemyLightPunching.enter()");
        fighter.StartCoroutine(startLightPunching());
        base.enter();
    }

    public override void update()
    {
        base.update();
    }

    public override void exit()
    {
        Debug.Log("EnemyLightPunching.exit()");
        base.exit();
    }

    public IEnumerator startLightPunching()
    {
        yield return new WaitForSeconds(Constants.Enemy.LIGHT_PUNCH_TELE_TIME);
        if (fighter.currentState == this)
        {
            fighter.doAttack(fighter.LightPunch);
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