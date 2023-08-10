using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JMisc {

    public static RectInt FlipRectInt(RectInt rect) {
        return new RectInt(rect.y, rect.x, rect.height, rect.width);
    }

    public static int MinI(int x, int y) { return x < y ? x : y; }
    public static int MaxI(int x, int y) { return x < y ? y : x; }
    
    public static Vector2Int Pos2D(GameObject go) {
        return new Vector2Int(
            Mathf.RoundToInt(go.transform.position.x), 
            Mathf.RoundToInt(go.transform.position.y)
        );
    }

    public static float Distance(GameObject g1, GameObject g2) {
        return Vector3.Distance(g1.transform.position, g2.transform.position);
    } 

    public static float Distance(GameObject g1, Vector3 g2) {
        return Vector3.Distance(g1.transform.position, g2);
    }

    public static float Distance(Vector3 g1, GameObject g2) {
        return Vector3.Distance(g1, g2.transform.position);
    }

    public static Vector3 MinAbsComp(Vector3 a, Vector3 b) {
        return new Vector3(
            Mathf.Abs(a.x) < Mathf.Abs(b.x) ? a.x : b.x,
            Mathf.Abs(a.y) < Mathf.Abs(b.y) ? a.y : b.y,
            Mathf.Abs(a.z) < Mathf.Abs(b.z) ? a.z : b.z
        );
    }

    public static Vector2 ToVector2(Vector3 v) { 
        return new Vector2(v.x, v.y); 
    }

    public static Vector3 ToVector3(Vector2 v) { 
        return new Vector3(v.x, v.y, 0f); 
    }
}
