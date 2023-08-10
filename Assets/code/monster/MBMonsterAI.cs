using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MBMonsterAI : MonoBehaviour {

    [SerializeField]
    private uint rngState = 0xAABB;

    [SerializeField]
    private LayerMask viewBlockingMask;

    [SerializeField]
    private LayerMask moveBlockingMask;

    [SerializeField]
    private LayerMask playersMask;

    private Rigidbody2D body;

    /* message */ void Awake() {
        this.body = this.GetComponent<Rigidbody2D>();
    }

    public bool IsVisible(Vector3 targetPosition) {
        return Physics2D.Raycast(
            this.transform.position,
            targetPosition - this.transform.position,
            Vector3.Distance(targetPosition, this.transform.position),
            this.viewBlockingMask
        ).collider == null;
    }

    public bool CanMoveTo(Vector3 targetPosition) {
        return Physics2D.Raycast(
            this.transform.position,
            targetPosition - this.transform.position,
            Vector3.Distance(targetPosition, this.transform.position) + 1.41f,
            this.moveBlockingMask
        ).collider == null;
    }

    public GameObject[] GetEnemiesInRange(float range) {
        Vector2 pos = JMisc.ToVector2(this.transform.position);
        Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(
            pos, range, this.playersMask
        );
        return enemyColliders.Select(collider => collider.gameObject).ToArray();
    }

    public GameObject[] GetVisibleEnemiesInRange(float range) { 
        return this.GetEnemiesInRange(range).Where(
            enemy => this.IsVisible(enemy.transform.position)
        ).ToArray();
    }

    public GameObject GetClosestVisibleEnemyInRange(float range) {
        return this.GetVisibleEnemiesInRange(range).OrderBy(
            go => JMisc.Distance(go, this.gameObject)
        ).FirstOrDefault();
    }

    private void DeltaMoveTo(Vector3 target, float speed) {
        Vector3 offset = target - this.transform.position;
        Vector3 moveby = JMisc.MinAbsComp(
            offset.normalized * speed * Time.fixedDeltaTime, offset
        );
        this.body.MovePosition(this.transform.position + moveby);
    }

    public IEnumerator ChaseTarget(
        GameObject target, float acceptRadius, float speed 
    ) { 
        while (this.IsVisible(target.transform.position)
            && JMisc.Distance(this.gameObject, target) > acceptRadius
        ) {
            yield return new WaitForFixedUpdate();
            this.DeltaMoveTo(target.transform.position, speed);
        }
    }

    private bool    isWandering = false;
    private Vector3 wanderTarget;
    public IEnumerator Wander(
        Vector3 pivot, float range, float speed
    ) {
        int iter = 100;
        Vector3 target;

        do {
            target = pivot + JMisc.ToVector3(
                range * JRNG.Xorshift32fInUnitCircle(ref this.rngState)
            );
        } while (--iter > 0 && !this.CanMoveTo(target));

        if (iter <= 0) yield return null;

        this.wanderTarget = target;
        this.isWandering = true;

        while (JMisc.Distance(this.gameObject, target) > .01f) {
            yield return new WaitForFixedUpdate();
            this.DeltaMoveTo(target, speed);
        }

        this.isWandering = false;
    }

#if UNITY_EDITOR 

    [SerializeField]
    private bool drawGizmo = true;

    /* message */ void OnDrawGizmos() {
        if (!this.drawGizmo) return;
        if (this.isWandering) {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(this.transform.position, this.wanderTarget);
            Gizmos.DrawSphere(this.wanderTarget, .1f);
        }   
    }
#endif
}
