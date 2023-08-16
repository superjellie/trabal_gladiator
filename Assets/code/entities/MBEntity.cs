using System.Collections;
using System.Collections.Generic;
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
        GameObject[] enemies = new GameObject[enemyColliders.Length];
        for (int i = 0; i < enemies.Length; ++i) 
            enemies[i] = enemyColliders[i].gameObject;
        return enemies;
    }

    public GameObject[] GetVisibleEnemiesInRange(float range) { 
        List<GameObject> visibleEnemies = new List<GameObject>();
        GameObject[] enemies = this.GetEnemiesInRange(range);
        for (int i = 0; i < enemies.Length; ++i) {
            if (this.IsVisible(enemies[i].transform.position))
                visibleEnemies.Add(enemies[i]);
        }
        return visibleEnemies.ToArray();
    }

    public GameObject GetClosestVisibleEnemyInRange(float range) {
        GameObject[] enemies = this.GetVisibleEnemiesInRange(range);

        float minDistance = 100000f;
        GameObject closestEnemy = null;

        foreach (GameObject enemy in enemies) {
            float distanceToEnemy = JMisc.Distance(enemy, this.gameObject);
            if (distanceToEnemy < minDistance) {
                minDistance = distanceToEnemy;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    public void FixedDeltaMoveTo(Vector3 target, float speed) {
        Vector3 offset = target - this.transform.position;
        Vector3 moveby = JMisc.MinAbsComp(
            offset.normalized * speed * Time.fixedDeltaTime, offset
        );
        this.body.MovePosition(this.transform.position + moveby);
    }

    private IEnumerator FixedUpdateCoroutine(
        System.Action action, System.Func<bool> whileTrue
    ) {
        while (whileTrue()) {
            action();
            yield return new WaitForFixedUpdate();
        }
    } 

    public JRoutine PushLogicRoutine(IEnumerator it, string name = "noname") {
        JRoutine rtn = this.coroutineMaster.Push(
            it, this.gameObject, this.name + ":" + name
        );
        return rtn;
    }

    public JRoutine InterruptLogicRoutine(
        IEnumerator it, GameObject master, string name = "noname"
    ) {
        JRoutine rtn = this.coroutineMaster.Push(
            it, master,
            this.name + ":" + name + "[" + master.name + "]"
        );
        return rtn;
    }

    public JRoutine MoveTo(
        Vector3 position, float speed,
        System.Func<bool> whileTrue = null
    ) {
        whileTrue = whileTrue ?? (() => true);
        JRoutine rtn = this.coroutineMaster.Push( 
            this.FixedUpdateCoroutine(
                () => this.FixedDeltaMoveTo(position, speed),
                () => this.CanMoveTo(position)
                    && JMisc.Distance(this.gameObject, position) > .001f
                    && whileTrue()
            ),
            this.gameObject,
            this.name + ":MoveTo"
        );
        return rtn;
    }

    public JRoutine ChaseTarget(
        GameObject target, float acceptRadius, float speed,
        System.Func<bool> whileTrue = null
    ) { 
        whileTrue = whileTrue ?? (() => true);
        JRoutine rtn = this.coroutineMaster.Push( 
            this.FixedUpdateCoroutine(
                () => this.FixedDeltaMoveTo(target.transform.position, speed),
                () => this.IsVisible(target.transform.position)
                    && this.CanMoveTo(target.transform.position)
                    && JMisc.Distance(this.gameObject, target) > acceptRadius
                    && whileTrue()
            ),
            this.gameObject,
            this.name + ":ChaseTarget"
        );
        return rtn;
    }

    public JRoutine MoveInDirection(
        Vector3 direction, float speed, float duration,
        System.Func<bool> whileTrue = null
    ) {
        whileTrue = whileTrue ?? (() => true);
        float startTime = Time.time;
        JRoutine rtn = this.coroutineMaster.Push( 
            this.FixedUpdateCoroutine(
                () => this.FixedDeltaMoveTo(
                    this.transform.position + direction, speed
                ),
                () => Time.time - startTime < duration 
                    && whileTrue()
            ),
            this.gameObject,
            this.name + ":MoveInDirection"
        );
        return rtn;
    }

    public JRoutine Wander(
        Vector3 pivot, float range, float speed,
        System.Func<bool> whileTrue = null
    ) {
        whileTrue = whileTrue ?? (() => true);
        int iter = 100;
        Vector3 target;

        do {
            target = pivot + JMisc.ToVector3(
                range * JRNG.Xorshift32fInUnitCircle(ref this.rngState)
            );
        } while (--iter > 0 && !this.CanMoveTo(target));

        if (iter <= 0) return null;

        JRoutine rtn = this.coroutineMaster.Push( 
            this.FixedUpdateCoroutine(
                () => this.FixedDeltaMoveTo(target, speed),
                () => JMisc.Distance(this.gameObject, target) > .01f
                    && whileTrue()
            ),
            this.gameObject,
            this.name + ":Wander"
        );
        return rtn;
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
        // if (this.isWandering) {
        //     Gizmos.color = Color.green;
        //     Gizmos.DrawLine(this.transform.position, this.wanderTarget);
        //     Gizmos.DrawSphere(this.wanderTarget, .1f);
        // } 
        // if (this.isChasingTarget) {
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawLine(this.transform.position, this.chaseTarget.position);
        //     Gizmos.DrawSphere(this.chaseTarget.position, .1f);
        // }   
    }
#endif
}
