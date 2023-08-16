using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MBEffect))]
public class MBPushEffect : MonoBehaviour {
    
    [SerializeField]
    private float impulse = 1f;

    [SerializeField]
    private float duration = 1f;

    private MBEffect effect;
    private Vector3 direction;

    [SerializeField]
    private SDamage hitWallDamage;

    private MBHealth targetHealth;

    /* message */ void Awake() {
        this.effect = this.GetComponent<MBEffect>();
    }

    /* message */ void Start() {
        this.targetHealth = this.effect.target.GetComponent<MBHealth>();
        this.effect.target.InterruptLogicRoutine(
            this.Push(), this.gameObject, "Push"
        );
    }

    private IEnumerator Push() {
        float startTime = Time.time;
        yield return this.effect.target.MoveInDirection(
            this.direction, this.impulse, this.duration
        );

        if (this.effect.target.IsTouchingWall(this.direction))
            this.targetHealth?.ApplyDamage(this.hitWallDamage);
        GameObject.Destroy(this.gameObject);
    }

    public void Init(Vector3 direction) {
        this.direction = direction;
    }
}
