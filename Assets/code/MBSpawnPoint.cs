using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBSpawnPoint : MonoBehaviour {
    
    [SerializeField]
    private GameObject prefab;

    /* message */ void Start() {
        GameObject go = GameObject.Instantiate(
            prefab, this.transform.position, this.transform.rotation
        );
        GameObject.Destroy(this.gameObject);
    }

}
