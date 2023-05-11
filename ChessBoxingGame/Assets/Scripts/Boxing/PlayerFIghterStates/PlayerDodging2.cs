using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodging2 : State
{
    public PlayerDodging2(Animator _anim, Transform _player, PlayerFighter _fighter) : base(_anim, _player, _fighter)
    {
        name = STATE.P_BLOCKING;
    }

    public override void goKO()
    {
        throw new System.NotImplementedException();
    }
}
