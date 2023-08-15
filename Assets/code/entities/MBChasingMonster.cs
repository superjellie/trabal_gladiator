using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MBEntity)),
 RequireComponent(typeof(MBCoroutineMaster))]
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

    private MBEntity entity;
    private MBCoroutineMaster crtnMaster;

    /* message */ void Awake() {
        this.entity = this.GetComponent<MBEntity>();
        this.crtnMaster = this.GetComponent<MBCoroutineMaster>();
        this.crtnMaster.Push("Logic", this.Process());
    }

    private IEnumerator Process() {
        while (true) {
            GameObject target = entity.GetClosestVisibleEnemyInRange(
                this.viewRange
            );

            if (target == null) {
                yield return this.crtnMaster.Push("Wander", entity.Wander(
                    this.transform.position, this.wanderRange, this.wanderSpeed
                ));
                continue;
            }

            yield return this.crtnMaster.Push("ChaseTarget", entity.ChaseTarget(
                target, this.attackRange, this.chasingSpeed
            )); 
                
            if (JMisc.Distance(this.gameObject, target) < this.attackRange) {
                // Debug.Log("MBChasingMonster: <color=yellow>Attacking</color> " 
                //     + target.name
                // );
                yield return new WaitForSeconds(1f);
            }
        } 
    }

}
