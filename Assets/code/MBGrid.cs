using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
               
public class MBGrid : MonoBehaviour {
    
    [SerializeField]
    public /* readonly */ RectInt rect;

    private GameObject[,] map;

    /* message */ void Awake() {
        this.map = new GameObject[this.rect.width, this.rect.height];
    } 

    // May return null
    public GameObject InstantiateAt(GameObject prefab, Vector2Int pos) {
        // Debug.Log("MBGrid: Instantiate " + prefab + " At " + pos.ToString());
        if (!this.rect.Contains(pos)) return null;
        if (this.GetObjectAt(pos) != null) return null;
        GameObject instance = GameObject.Instantiate(prefab, this.transform);
        instance.transform.localPosition = new Vector3(pos.x, pos.y, 0f);
        this.map[pos.x - this.rect.min.x, pos.y - this.rect.min.y] = instance;
        return instance;
    }

    // May return null
    public GameObject GetObjectAt(Vector2Int pos) {
        // Debug.Log("MBGrid: Get At " + pos.ToString());
        if (!this.rect.Contains(pos)) return null;
        return this.map[
            pos.x - this.rect.min.x, 
            pos.y - this.rect.min.y
        ];
    }


#if UNITY_EDITOR

    [SerializeField]
    private bool drawGizmo = true;

    /* message */ void OnDrawGizmos() {
        if (!this.drawGizmo) return;
        Handles.color = Color.yellow;
        for (int x = this.rect.min.x; x <= this.rect.max.x; ++x) {
            Vector3 localNx = new Vector3(x - .5f, this.rect.max.y - .5f, 0f);
            Vector3 localSx = new Vector3(x - .5f, this.rect.min.y - .5f, 0f);
            Vector3 Nx = this.transform.TransformPoint(localNx);
            Vector3 Sx = this.transform.TransformPoint(localSx);
            Handles.DrawLine(Nx, Sx, 0.2f);
        }

        for (int y = this.rect.min.y; y <= this.rect.max.y; ++y) {
            Vector3 localyW = new Vector3(this.rect.min.x - .5f, y - .5f, 0f);
            Vector3 localyE = new Vector3(this.rect.max.x - .5f, y - .5f, 0f);
            Vector3 yW = this.transform.TransformPoint(localyW);
            Vector3 yE = this.transform.TransformPoint(localyE);
            Handles.DrawLine(yW, yE, 0.2f);
        }
    }
#endif

}
