using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MBAbility))]
public class MBDashAbility : MonoBehaviour {

    [SerializeField]
    private float distance = 2f;

    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private MBPushEffect pushEffectPrefab;

    private MBAbility ability;
    /* message */ void Awake() {
        this.ability = this.GetComponent<MBAbility>();
        this.ability.InvokeOnUse(this.Use);
        this.ability.runDuration = this.distance / this.speed;
    }

    private IEnumerator Use(MBEntity master, Vector3 target) { 
        Vector3 masterPos = master.transform.position;
        Vector3 dodgeTarget = 
            masterPos + this.distance * (target - masterPos).normalized;
        while (this.ability.GetRunTime() < this.ability.runDuration) {
            yield return new WaitForFixedUpdate();
            master.FixedDeltaMoveTo(dodgeTarget, this.speed);
            masterPos = master.transform.position;
            master.InvokeOnCollisionWithEnemy(entity => 
                pushEffectPrefab.ApplyTo(entity, 
                    ((entity.transform.position - masterPos).normalized
                    + (dodgeTarget - masterPos).normalized).normalized
                )
            );
        }
    }

}
