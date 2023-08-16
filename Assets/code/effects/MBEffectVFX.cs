using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MBEffect))]
public class MBEffectVFX : MonoBehaviour {

    private MBEffect effect;

    /* message */ void Awake() {
        this.effect = this.GetComponent<MBEffect>();
    }
    
    /* message */ void Start() {
        
    }
}
