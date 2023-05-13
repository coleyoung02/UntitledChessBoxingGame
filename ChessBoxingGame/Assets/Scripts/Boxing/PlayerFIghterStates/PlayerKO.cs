using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKO : State
{
    public PlayerKO(Animator _anim, Transform _player, PlayerFighter _fighter) : base(_anim, _player, _fighter)
    {
        name = STATE.P_KO;
    }

    public override void goKO()
    {
        return;
    }
}
