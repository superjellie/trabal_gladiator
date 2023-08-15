using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MBAbility))]
public class MBDodgeAbility : MonoBehaviour {

    [SerializeField]
    private float distance = 2f;

    [SerializeField]
    private float speed = 5f;

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
        }
    }

}
