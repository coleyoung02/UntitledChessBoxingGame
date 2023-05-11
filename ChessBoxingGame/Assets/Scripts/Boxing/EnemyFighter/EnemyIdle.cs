using UnityEngine;


public class EnemyIdle : State
{
    public EnemyIdle(Animator _anim, Transform _player, EnemyFighter _fighter) : base(_anim, _player, _fighter)
    {
        name = STATE.E_IDLE;
    }

    public override void enter()
    {
        //Debug.Log("EnemyIdle.enter()");
        base.enter();
    }

    public override void exit()
    {
        //Debug.Log("EnemyIdle.exit()");
        base.exit();
    }

    public void goBlocking()
    {
        nextState = new EnemyBlocking(anim, player, (EnemyFighter)fighter);
        stage = EVENT.EXIT;
    }

    public void goLightPunching()
    {
        nextState = new EnemyLightPunching(anim, player, (EnemyFighter)fighter);
        stage = EVENT.EXIT;
    }

    public void goHeavyPunching()
    {
        nextState = new EnemyHeavyPunchingFst(anim, player, (EnemyFighter)fighter);
        stage = EVENT.EXIT;
    }

    public void goStunned()
    {
        nextState = new EnemyStunned(anim, player, (EnemyFighter)fighter);
        stage = EVENT.EXIT;
    }

    public override void goKO()
    {
        nextState = new EnemyKO(anim, player, (EnemyFighter)fighter);
        stage = EVENT.EXIT;
    }
}