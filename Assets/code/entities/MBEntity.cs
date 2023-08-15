using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D)),
 RequireComponent(typeof(MBCoroutineMaster))]
public class MBEntity : MonoBehaviour {

    [SerializeField]
    private uint rngState = 0xAABB;

    [SerializeField]
    private LayerMask viewBlockingMask;

    [SerializeField]
    private LayerMask moveBlockingMask;

    [SerializeField]
    private LayerMask enemyMask;

    private Rigidbody2D body;
    private MBCoroutineMaster coroutineMaster;

    /* message */ void Awake() {
        this.body = this.GetComponent<Rigidbody2D>();
        this.coroutineMaster = this.GetComponent<MBCoroutineMaster>();
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

    public bool IsTouchingWall(Vector3 direction) {
        return this.body.IsTouchingLayers(this.moveBlockingMask);
    }

    public GameObject[] GetEnemiesInRange(float range) {
        Vector2 pos = JMisc.ToVector2(this.transform.position);
        Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(
            pos, range, this.enemyMask
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

    public void FixedDeltaMoveTo(Vector3 target, float speed) {
        Vector3 offset = target - this.transform.position;
        Vector3 moveby = JMisc.MinAbsComp(
            offset.normalized * speed * Time.fixedDeltaTime, offset
        );
        this.body.MovePosition(this.transform.position + moveby);
    }

    public IEnumerator MoveTo(
        Vector3 to, float speed, System.Func<bool> whileTrue = null
    ) {
        whileTrue = whileTrue ?? (() => true);
        while (JMisc.Distance(this.gameObject, to) > .0001f
            && whileTrue()) {
            this.FixedDeltaMoveTo(to, speed);
            yield return new WaitForFixedUpdate();
        }
    } 

    public IEnumerator MoveWhile(
        Vector3 speed, System.Func<bool> whileTrue
    ) {
        whileTrue = whileTrue ?? (() => true);
        while (whileTrue()) {
            this.body.MovePosition(
                this.transform.position + speed * Time.fixedDeltaTime
            );
            yield return new WaitForFixedUpdate();
        }
    } 

    private bool      isChasingTarget = false;
    private Transform chaseTarget;
    public IEnumerator ChaseTarget(
        GameObject target, float acceptRadius, float speed
    ) { 
        this.isChasingTarget = true;
        this.chaseTarget = target.transform;
        while (this.IsVisible(target.transform.position)
            && JMisc.Distance(this.gameObject, target) > acceptRadius
        ) {
            this.FixedDeltaMoveTo(target.transform.position, speed);
            yield return new WaitForFixedUpdate();
        }
        this.isChasingTarget = false;
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
            this.FixedDeltaMoveTo(target, speed);
        }

        this.isWandering = false;
    }

    public void InvokeOnCollisionWithEnemy(UnityAction<MBEntity> handler) {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(this.enemyMask);
        Collider2D[] results = new Collider2D[10];
        int count = this.body.OverlapCollider(filter, results);
        for (int i = 0; i < count; ++i) 
            if (results[i] != null && results[i].attachedRigidbody != null)
                handler.Invoke(
                    results[i].attachedRigidbody.GetComponent<MBEntity>()
                );
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
        if (this.isChasingTarget) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(this.transform.position, this.chaseTarget.position);
            Gizmos.DrawSphere(this.chaseTarget.position, .1f);
        }   
    }
#endif
}
