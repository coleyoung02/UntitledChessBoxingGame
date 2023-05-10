// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.AI;
//
// public class AI : MonoBehaviour {
//
//     NavMeshAgent agent;
//     Animator anim;
//     State currentState;
//
//     public Transform player;
//
//     void Start() {
//
//         agent = GetComponent<NavMeshAgent>();
//         anim = GetComponent<Animator>();
//         currentState = new Idle(gameObject, agent, anim, player);
//     }
//
//
//     void Update() {
//
//         currentState = currentState.Process();
//     }
// }
