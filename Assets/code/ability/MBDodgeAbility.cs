using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MBAbility))]
public class MBDodgeAbility : MonoBehaviour {

    [SerializeField]
    private float dodgeDistance = 2f;

    [SerializeField]
    private float dodgeSpeed = 5f;
    
    private MBAbility ability;
    /* message */ void Awake() {
        this.ability = this.GetComponent<MBAbility>();
        this.ability.InvokeOnUse(this.Use);
    }

    public IEnumerator Use(MBEntity master, Vector3 target) { 
        Debug.Log("Dodge Use");
        Vector3 direction = (target - master.transform.position).normalized;
        Vector3 dodgeTarget = 
            master.transform.position + dodgeDistance * direction;
        Collider2D collider = master.GetComponent<Collider2D>();
        float time = this.dodgeDistance / this.dodgeSpeed;
        float startTime = Time.time;
        while (Time.time - startTime < time) {
            yield return new WaitForFixedUpdate();
            master.FixedDeltaMoveTo(dodgeTarget, this.dodgeSpeed);
        }
        yield return null;
    }

}
