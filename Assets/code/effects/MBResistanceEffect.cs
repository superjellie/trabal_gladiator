using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MBEffect))]
public class MBResistanceEffect : MonoBehaviour {

    [SerializeField]
    private EDamageTypeMask resistanceTo;

    [SerializeField]
    private float reduceDamageBy = 1.0f;
    
    [SerializeField]
    private float scaleDamageBy = 1.0f; 
        
    private MBEffect effect;
    private MBHealth targetHealth;

    /* message */ void Awake() {
        this.effect = this.GetComponent<MBEffect>();
    }

    /* message */ void Start() {
        this.targetHealth = this.effect.target.GetComponent<MBHealth>();
        this.targetHealth?.onRecieveDamage.AddListener(damage => {
            if ((damage.typeMask & this.resistanceTo) != 0) {
                damage.amount -= this.reduceDamageBy;
                damage.amount *= this.scaleDamageBy;
            }
        });
    }


}
