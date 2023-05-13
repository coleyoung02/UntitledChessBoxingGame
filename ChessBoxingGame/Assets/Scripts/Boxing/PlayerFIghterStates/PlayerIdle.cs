using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdle : State
{
    public PlayerIdle(Animator _anim, Transform _player, PlayerFighter _fighter) : base(_anim, _player, _fighter)
    {
        name = STATE.P_IDLE;
    }

    public void goDodging1()
    {
        nextState = new PlayerDodging1(anim, player, (PlayerFighter)fighter);
        stage = EVENT.EXIT;
    }

    public void goPunching()
    {
        nextState = new PlayerPunching(anim, player, (PlayerFighter)fighter);
        stage = EVENT.EXIT;
    }

    public void goBlocking()
    {
        nextState = new PlayerBlocking(anim, player, (PlayerFighter)fighter);
        stage = EVENT.EXIT;
    }

    public override void goKO()
    {
        nextState = new PlayerKO(anim, player, (PlayerFighter)fighter);
        stage = EVENT.EXIT;
    }
}
