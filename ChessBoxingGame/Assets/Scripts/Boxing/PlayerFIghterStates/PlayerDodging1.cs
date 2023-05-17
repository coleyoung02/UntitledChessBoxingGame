using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodging1 : State
{
    private Coroutine coroutine;
    public PlayerDodging1(Animator _anim, Transform _player, PlayerFighter _fighter) : base(_anim, _player, _fighter)
    {
        name = STATE.P_DODGING1;
        coroutine = null;
    }

    public override void enter()
    {
        fighter.StartCoroutine(startDodging1());
        base.enter();
    }

    public IEnumerator startDodging1()
    {
        yield return new WaitForSeconds(Constants.Player.DODGE_IMMMUNITY_TIME);
        if (fighter.currentState == this)
        {
            nextState = new PlayerDodging2(anim, player, (PlayerFighter)fighter);
            stage = EVENT.EXIT;
        }
        coroutine = null;
    }

    public override void goKO()
    {
        if (coroutine != null)
        {
            fighter.StopCoroutine(coroutine);
            coroutine = null;
        }
        nextState = new PlayerKO(anim, player, (PlayerFighter)fighter);
        stage = EVENT.EXIT;
    }
}
