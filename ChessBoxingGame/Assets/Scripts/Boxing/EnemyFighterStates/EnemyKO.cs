using UnityEngine;


public class EnemyKO : State
{
    private static readonly int GoKo = Animator.StringToHash("GoKO");

    public EnemyKO(Animator _anim, Transform _player, EnemyFighter _fighter) : base(_anim, _player, _fighter)
    {
        name = STATE.E_KO;
    }

    public override void enter()
    {
        base.enter();
        anim.SetBool(GoKo, true);
    }

    public override void exit()
    {
        base.exit();
        anim.SetBool(GoKo, false);
    }

    public override void goKO()
    {
        return;
    }
}