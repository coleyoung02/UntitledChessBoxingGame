using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodging2 : State
{
    private Coroutine coroutine;
    public PlayerDodging2(Animator _anim, Transform _player, PlayerFighter _fighter) : base(_anim, _player, _fighter)
    {
        name = STATE.P_DODGING2;
        coroutine = null;
    }

    public override void enter()
    {
        fighter.StartCoroutine(startDodging2());
        base.enter();
    }

    public IEnumerator startDodging2()
    {
        yield return new WaitForSeconds(Constants.Player.DODGE_NO_IMMMUNITY_TIME);
        if (fighter.currentState == this)
        {
            nextState = new PlayerIdle(anim, player, (PlayerFighter)fighter);
            anim.SetTrigger("GoIdle");
            stage = EVENT.EXIT;
        }
        coroutine = null;
    }

    public void goPunching()
    {
        if (coroutine != null)
        {
            fighter.StopCoroutine(coroutine);
            coroutine = null;
        }
        nextState = new PlayerPunching(anim, player, (PlayerFighter)fighter);
        anim.SetTrigger("Punch");
        stage = EVENT.EXIT;
    }

    public void goHeavyPunching()
    {
        if (coroutine != null)
        {
            fighter.StopCoroutine(coroutine);
            coroutine = null;
        }
        nextState = new PlayerHeavyPunch(anim, player, (PlayerFighter)fighter);
        anim.SetTrigger("HeavyPunch");
        stage = EVENT.EXIT;
    }

    public override void goKO()
    {
        if (coroutine != null)
        {
            fighter.StopCoroutine(coroutine);
            coroutine = null;
        }
        nextState = new PlayerKO(anim, player, (PlayerFighter)fighter);
        anim.SetTrigger("KO");
        stage = EVENT.EXIT;
    }
}
