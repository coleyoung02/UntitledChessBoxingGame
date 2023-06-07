using UnityEngine;


public class EnemyIdle : State
{
    private static readonly int GoIdle = Animator.StringToHash("GoIdle");

    public EnemyIdle(Animator _anim, Transform _player, EnemyFighter _fighter) : base(_anim, _player, _fighter)
    {
        name = STATE.E_IDLE;
    }

    public override void enter()
    {
        //Debug.Log("EnemyIdle.enter()");
        base.enter();
        anim.SetBool(GoIdle, true);
    }

    public override void exit()
    {
        //Debug.Log("EnemyIdle.exit()");
        base.exit();
        anim.SetBool(GoIdle, false);
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