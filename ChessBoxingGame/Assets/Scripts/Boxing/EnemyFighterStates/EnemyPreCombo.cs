using System;
using System.Collections;
using UnityEngine;

public class EnemyPreCombo : State
{
    private Coroutine coroutine;
    private static readonly int GoPreCombo = Animator.StringToHash("GoPreCombo");

    public EnemyPreCombo(Animator _anim, Transform _player, EnemyFighter _fighter) : base(_anim, _player, _fighter)
    {
        name = STATE.E_PRECOMBO;
        coroutine = null;
    }

    public override void enter()
    {
        fighter.StartCoroutine(startPreCombo());
        base.enter();
        anim.SetBool(GoPreCombo, true);
    }

    public override void update()
    {
        base.update();
    }

    public override void exit()
    {
        base.exit();
        anim.SetBool(GoPreCombo, false);
    }

    public IEnumerator startPreCombo()
    {
        yield return new WaitForSeconds(Constants.Enemy.COMBO_TELE_TIME);
        if (fighter.currentState == this)
        {
            nextState = new EnemyHeavyPunchingFst(anim, player, (EnemyFighter)fighter, 3);
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
        nextState = new EnemyKO(anim, player, (EnemyFighter)fighter);
        stage = EVENT.EXIT;
    }
    
}