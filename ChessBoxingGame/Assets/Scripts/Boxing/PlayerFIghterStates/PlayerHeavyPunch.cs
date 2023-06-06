using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeavyPunch: State
{
    private Coroutine coroutine;
    public PlayerHeavyPunch(Animator _anim, Transform _player, PlayerFighter _fighter) : base(_anim, _player, _fighter)
    {
        name = STATE.P_HEAVYPUNCHING;
        coroutine = null;
    }

    public override void enter()
    {
        coroutine = fighter.StartCoroutine(startPunching());
        base.enter();
    }

    public IEnumerator startPunching()
    {
        yield return new WaitForSeconds(Constants.Player.LIGHT_PUNCH_TELE_TIME);
        if (fighter.currentState == this)
        {
            fighter.doAttack(fighter.HeavyPunch);
            nextState = new PlayerIdle(anim, player, (PlayerFighter)fighter);
            anim.SetTrigger("GoIdle");
            stage = EVENT.EXIT;
        }
        coroutine = null;
    }

    public void goIdle()
    {
        if (coroutine != null)
        {
            fighter.StopCoroutine(coroutine);
            coroutine = null;
        }
        anim.SetTrigger("GoIdle");
        nextState = new PlayerIdle(anim, player, (PlayerFighter)fighter);
        stage = EVENT.EXIT;
    }

    public override void goKO()
    {
        if (coroutine != null)
        {
            fighter.StopCoroutine(coroutine);
            coroutine = null;
        }
        anim.SetTrigger("KO");
        nextState = new PlayerKO(anim, player, (PlayerFighter)fighter);
        stage = EVENT.EXIT;
    }
}
