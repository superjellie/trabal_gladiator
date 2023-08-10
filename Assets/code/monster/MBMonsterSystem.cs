using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBMonsterSystem : MonoBehaviour {

    // [SerializeField]
    // private MBTag tagTileFloor;

    [SerializeField]
    private LayerMask viewBlockMask;

    [SerializeField]
    private LayerMask walkBlockMask;

    [SerializeField]
    private LayerMask playerMask;
    
    [SerializeField]
    private MBGrid grid;

    // DO NOT USE INSTANCE
    // Use GetSystemFor instead
    private static MBMonsterSystem instance;
    /* message */ void Awake() {
        MBMonsterSystem.instance = this;
    }

    private static MBMonsterSystem GetSystemFor(GameObject monster) {
        if (MBMonsterSystem.instance == null) 
            Debug.Log("MBMonsterSystem: <color=red>No system in scene</color>");
        return MBMonsterSystem.instance;
    }

    public static GameObject[] GetEnemiesInView(GameObject me, float range) { 
        MBMonsterSystem system = MBMonsterSystem.GetSystemFor(me);
        Vector2 pos = new Vector2(
            me.transform.position.x, me.transform.position.y
        );
        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            pos, range, system.playerMask.value
        );
        List<GameObject> enemiesInView = new List<GameObject>();
        foreach (Collider2D enemy in enemies) {
            Vector2 enemyPos = new Vector2(
                enemy.transform.position.x, enemy.transform.position.y
            );
            Vector2 direction = enemyPos - pos;
            if (Physics2D.Raycast(
                    pos, direction, direction.magnitude, system.viewBlockMask 
                ).collider == null
            ) enemiesInView.Add(enemy.gameObject);
        }
        
        return enemiesInView.ToArray();
    }

    public static GameObject GetClosestEnemyInView(GameObject me, float range) {
        GameObject[] enemies = MBMonsterSystem.GetEnemiesInView(me, range);
        if (enemies.Length == 0) return null;
        GameObject min = enemies[0];
        for (int i = 1; i < enemies.Length; ++i)
            if (JMisc.Distance(min, me) > JMisc.Distance(enemies[i], me))
                min = enemies[i];
        return min;
    }

    public enum MoveType {
        Walk = 0,
        Fly  = 1
    };

    public static IEnumerator ChaseTarget(
        GameObject me, GameObject target, float acceptRadius, float speed, 
        MoveType moveType = MoveType.Walk
    ) { 
        Rigidbody2D body = me.GetComponent<Rigidbody2D>();
        if (body == null) {
            Debug.Log(
                "MBMonsterSystem.ChaseTarget:<color=red>" 
                + " monster have no Rigidbody2D</color>"
            );
            yield return new WaitForSeconds(1f);
        }   
        while (JMisc.Distance(me, target) > acceptRadius) {
            yield return new WaitForFixedUpdate();
            Vector3 offset = target.transform.position - me.transform.position;
            Vector3 moveby = JMisc.MinAbsComp(
                offset.normalized * speed * Time.fixedDeltaTime, offset
            );
            body.MovePosition(me.transform.position + moveby);
        }
    }

    public static IEnumerator Wander(
        GameObject me, Vector3 pivot, float range, float speed, 
        MoveType moveType = MoveType.Walk
    ) {
        MBMonsterSystem system = MBMonsterSystem.GetSystemFor(me);

        Rigidbody2D body = me.GetComponent<Rigidbody2D>();
        if (body == null) {
            Debug.Log(
                "MBMonsterSystem.Wander:<color=red>" 
                + " monster have no Rigidbody2D</color>"
            );
            yield return new WaitForSeconds(1f);
        }   
        Vector3 target;
        Vector2 targetOffset;
        Vector2 pos = new Vector2(
            me.transform.position.x, me.transform.position.y
        );
        int iter = 100;
        do {    
            Vector2 rnd = Random.insideUnitCircle;
            target = pivot + range * new Vector3(rnd.x, rnd.y, 0f);
            targetOffset = new Vector2(target.x, target.y) - pos;
        } while (--iter > 0 && Physics2D.Raycast(
                pos, targetOffset, targetOffset.magnitude + 1.41f, 
                system.walkBlockMask 
            ).collider != null);

        if (iter <= 0) yield return null;

        while (JMisc.Distance(me, target) > .01f) {
            yield return new WaitForFixedUpdate();
            Vector3 offset = target - me.transform.position;
            Vector3 moveby = JMisc.MinAbsComp(
                offset.normalized * speed * Time.fixedDeltaTime, offset
            );
            body.MovePosition(me.transform.position + moveby);
        }
    }
}
