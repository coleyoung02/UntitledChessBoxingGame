using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public abstract class State {

    [Serializable]
    public enum STATE {
        // Enemy states
        E_IDLE              = 0,
        E_BLOCKING          = 1,
        E_HEAVYPUNCHINGFST  = 2,
        E_HEAVYPUNCHINGSND  = 3,
        E_LIGHTPUNCHING     = 4,
        E_STUNNED           = 5,
        E_KO                = 6,
        E_FAKEIDLE          = 7,
        P_IDLE              = 8,
        P_PUNCHING          = 9,
        P_DODGING1          = 10,
        P_DODGING2          = 11,
        P_BLOCKING          = 12,
        P_KO                = 13,
        P_HEAVYPUNCHING     = 14
    };

    public enum EVENT {
        ENTER,
        UPDATE,
        EXIT
    };

    public STATE name;
    protected EVENT stage;
    protected Animator anim;
    protected Transform player;
    protected Fighter fighter;
    protected State nextState;


    public State(Animator _anim, Transform _player, Fighter _fighter) {
        anim = _anim;
        player = _player;
        fighter = _fighter;
        stage = EVENT.ENTER;
    }

    public virtual void enter() { stage = EVENT.UPDATE; }
    public virtual void update() { stage = EVENT.UPDATE; }
    public virtual void exit() { stage = EVENT.EXIT; }

    public State process() {

        if (stage == EVENT.ENTER) enter();
        if (stage == EVENT.UPDATE) update();
        if (stage == EVENT.EXIT) {

            exit();
            return nextState;
        }

        return this;
    }

    public abstract void goKO();

}

// Reference codes from the tutorial
//
// public class Idle : State {
//
//     public Idle(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
//         : base(_npc, _agent, _anim, _player) {
//
//         name = STATE.IDLE;
//     }
//
//     public override void Enter() {
//
//         anim.SetTrigger("isIdle");
//         base.Enter();
//     }
//
//     public override void Update() {
//
//         if (CanSeePlayer()) {
//
//             nextState = new Pursue(npc, agent, anim, player);
//             stage = EVENT.EXIT;
//         } else if (Random.Range(0, 100) < 10) {
//
//             nextState = new Patrol(npc, agent, anim, player);
//             stage = EVENT.EXIT;
//         }
//     }
//
//     public override void Exit() {
//
//         anim.ResetTrigger("isIdle");
//         base.Exit();
//     }
// }
//
// public class Patrol : State {
//
//     int currentIndex = -1;
//
//     public Patrol(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
//         : base(_npc, _agent, _anim, _player) {
//
//         name = STATE.PATROL;
//         agent.speed = 2.0f;
//         agent.isStopped = false;
//
//     }
//
//     public override void Enter() {
//
//         float lastDistance = Mathf.Infinity;
//
//         for (int i = 0; i < GameEnvironment.Singleton.Checkpoints.Count; ++i) {
//
//             GameObject thisWP = GameEnvironment.Singleton.Checkpoints[i];
//             float distance = Vector3.Distance(npc.transform.position, thisWP.transform.position);
//             if (distance < lastDistance) {
//
//                 currentIndex = i - 1;
//                 lastDistance = distance;
//             }
//         }
//
//         anim.SetTrigger("isWalking");
//         base.Enter();
//     }
//
//     public override void Update() {
//
//         if (agent.remainingDistance < 1) {
//
//             if (currentIndex >= GameEnvironment.Singleton.Checkpoints.Count - 1) {
//
//                 currentIndex = 0;
//             } else {
//
//                 currentIndex++;
//             }
//
//             agent.SetDestination(GameEnvironment.Singleton.Checkpoints[currentIndex].transform.position);
//         }
//
//         if (CanSeePlayer()) {
//
//             nextState = new Pursue(npc, agent, anim, player);
//             stage = EVENT.EXIT;
//         } else if (IsPlayerBehind()) {
//
//             nextState = new RunAway(npc, agent, anim, player);
//             stage = EVENT.EXIT;
//         }
//     }
//
//     public override void Exit() {
//
//         anim.ResetTrigger("isWalking");
//         base.Exit();
//     }
// }
//
// public class Pursue : State {
//
//     public Pursue(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
//         : base(_npc, _agent, _anim, _player) {
//
//
//         name = STATE.PURSUE;
//         agent.speed = 5.0f;
//         agent.isStopped = false;
//     }
//
//     public override void Enter() {
//
//         anim.SetTrigger("isRunning");
//         base.Enter();
//     }
//
//     public override void Update() {
//
//         agent.SetDestination(player.position);
//
//         if (agent.hasPath) {
//
//             if (CanAttackPlayer()) {
//
//                 nextState = new Attack(npc, agent, anim, player);
//                 stage = EVENT.EXIT;
//             } else if (!CanSeePlayer()) {
//
//                 nextState = new Patrol(npc, agent, anim, player);
//                 stage = EVENT.EXIT;
//             }
//         }
//     }
//
//     public override void Exit() {
//
//         anim.ResetTrigger("isRunning");
//         base.Exit();
//     }
// }
//
// public class Attack : State {
//
//     float rotationSpeed = 2.0f;
//     AudioSource shoot;
//
//     public Attack(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
//         : base(_npc, _agent, _anim, _player) {
//
//         name = STATE.ATTACK;
//         shoot = _npc.GetComponent<AudioSource>();
//     }
//
//     public override void Enter() {
//
//         anim.SetTrigger("isShooting");
//         agent.isStopped = true;
//         shoot.Play();
//         base.Enter();
//     }
//
//     public override void Update() {
//
//         Vector3 direction = player.position - npc.transform.position;
//         float angle = Vector3.Angle(direction, npc.transform.forward);
//         direction.y = 0.0f;
//
//         npc.transform.rotation =
//             Quaternion.Slerp(npc.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotationSpeed);
//
//         if (!CanAttackPlayer()) {
//
//             nextState = new Idle(npc, agent, anim, player);
//             shoot.Stop();
//             stage = EVENT.EXIT;
//         }
//     }
//
//     public override void Exit() {
//
//         anim.ResetTrigger("isShooting");
//         base.Exit();
//     }
// }
//
// public class RunAway : State {
//
//     GameObject safeLocation;
//
//     public RunAway(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
//         : base(_npc, _agent, _anim, _player) {
//
//         name = STATE.RUNAWAY;
//         safeLocation = GameObject.FindGameObjectWithTag("Safe");
//     }
//
//     public override void Enter() {
//
//         anim.SetTrigger("isRunning");
//         agent.isStopped = false;
//         agent.speed = 6;
//         agent.SetDestination(safeLocation.transform.position);
//         base.Enter();
//     }
//
//     public override void Update() {
//
//         if (agent.remainingDistance < 1.0f) {
//
//             nextState = new Idle(npc, agent, anim, player);
//             stage = EVENT.EXIT;
//         }
//     }
//
//     public override void Exit() {
//
//         anim.ResetTrigger("isRunning");
//         base.Exit();
//     }
// }