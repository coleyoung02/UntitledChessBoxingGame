using UnityEngine;


public class EnemyKO : State
{
    public EnemyKO(Animator _anim, Transform _player, EnemyFighter _fighter) : base(_anim, _player, _fighter)
    {
        name = STATE.E_KO;
    }

    public override void enter()
    {
        Debug.Log("EnemyKO.enter()");
        base.enter();
    }

    public override void exit()
    {
        Debug.Log("EnemyKO.exit()");
        base.exit();
    }

    public override void goKO()
    {
        return;
    }
}