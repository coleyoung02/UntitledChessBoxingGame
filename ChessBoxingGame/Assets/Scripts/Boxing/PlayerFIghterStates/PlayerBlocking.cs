using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlocking : State
{
    public PlayerBlocking(Animator _anim, Transform _player, PlayerFighter _fighter) : base(_anim, _player, _fighter)
    {
        name = STATE.P_BLOCKING;
    }

    public void goDodging1()
    {
        nextState = new PlayerDodging1(anim, player, (PlayerFighter)fighter);
        stage = EVENT.EXIT;
    }

    public void goIdle()
    {
        nextState = new PlayerIdle(anim, player, (PlayerFighter)fighter);
        stage = EVENT.EXIT;
    }

    public override void goKO()
    {
        nextState = new PlayerKO(anim, player, (PlayerFighter)fighter);
        stage = EVENT.EXIT;
    }


}
