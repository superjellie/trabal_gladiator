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
    }

    /* message */ void Start() {
        this.ability.InvokeOnUse(this.Use);
        this.ability.runDuration = this.distance / this.speed;
    }

    private IEnumerator Use(MBEntity master, Vector3 target) { 
        Vector3 masterPos = master.transform.position;
        Vector3 dodgeDirection = (target - masterPos).normalized;
        Vector3 dodgeTarget = masterPos + this.distance * dodgeDirection;
        while (this.ability.GetRunTime() < this.ability.runDuration) {
            yield return new WaitForFixedUpdate();
            master.FixedDeltaMoveTo(dodgeTarget, this.speed);
            masterPos = master.transform.position;
            master.InvokeOnCollisionWithEnemy(entity => {
                Vector3 entityDirection = 
                    (entity.transform.position - masterPos).normalized;
                MBEffect.ApplyFromPrefab(pushEffectPrefab, entity)
                    ?.Init((dodgeDirection + entityDirection).normalized);
            });
        }
    }

}
