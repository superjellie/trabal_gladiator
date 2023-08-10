using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    /* message */ void Awake() {

    }

    /* message */ IEnumerator Start() {
        while (true) {
            GameObject target = MBMonsterSystem.GetClosestEnemyInView(
                this.gameObject, this.viewRange
            );

            if (target == null) {
                yield return this.StartCoroutine(MBMonsterSystem.Wander(
                    this.gameObject, this.transform.position, 
                    this.wanderRange, this.wanderSpeed
                ));
                continue;
            }

            yield return this.StartCoroutine(MBMonsterSystem.ChaseTarget(
                this.gameObject, target, this.attackRange, this.chasingSpeed
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
