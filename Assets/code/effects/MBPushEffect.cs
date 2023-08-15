using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MBEffect))]
public class MBPushEffect : MonoBehaviour {
    
    [SerializeField]
    private float impulse = 1f;

    [SerializeField]
    private float duration = 1f;

    private MBEntity target;
    private MBCoroutineMaster crtnMaster;
    private Vector3 direction;

    private IEnumerator Push() {
        float startTime = Time.time;
        yield return this.crtnMaster.Push("Push Effect Move", 
            this.target.MoveWhile(this.direction * this.impulse, 
                () => !this.target.IsTouchingWall(this.direction)
                && Time.time - startTime < this.duration
            )
        );
        GameObject.Destroy(this.gameObject);
    }

    public void ApplyTo(MBEntity entity, Vector3 direction) {
        if (entity.GetComponentInChildren<MBPushEffect>()) return;
        GameObject go = GameObject.Instantiate(
            this.gameObject, entity.transform
        );
        MBPushEffect instance = go.GetComponent<MBPushEffect>();
        instance.crtnMaster = entity.GetComponent<MBCoroutineMaster>();
        instance.direction = direction;
        instance.target = entity;
        instance.crtnMaster.Push("Push Effect Logic", instance.Push());
    }
}
