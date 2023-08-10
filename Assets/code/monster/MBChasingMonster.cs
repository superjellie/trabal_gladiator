using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MBMonsterAI))]
public class MBChasingMonster : MonoBehaviour {
    
    [SerializeField]
    private float wanderSpeed = .1f;

    [SerializeField]
    private float chasingSpeed = .2f;

    [SerializeField]
    private float viewRange = 5f;

    [SerializeField]
    private float attackRange = .5f;

    [SerializeField]
    private float wanderRange = 2f;

    // [SerializeField]
    // private MBAbility attackAbility;

    private MBMonsterAI monsterAI;

    /* message */ void Awake() {
        this.monsterAI = this.GetComponent<MBMonsterAI>();
    }

    /* message */ IEnumerator Start() {
        while (true) {
            GameObject target = monsterAI.GetClosestVisibleEnemyInRange(
                this.viewRange
            );

            if (target == null) {
                yield return this.StartCoroutine(monsterAI.Wander(
                    this.transform.position, this.wanderRange, this.wanderSpeed
                ));
                continue;
            }

            yield return this.StartCoroutine(monsterAI.ChaseTarget(
                target, this.attackRange, this.chasingSpeed
            )); 
                
            if (JMisc.Distance(this.gameObject, target) < this.attackRange) {
                Debug.Log("MBChasingMonster: <color=yellow>Attacking</color> " 
                    + target.name
                );
                yield return new WaitForSeconds(1f);
            }
        } 
    }

}
