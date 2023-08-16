using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MBAbility))]
public class MBAbilityVFX : MonoBehaviour {
    
    [Header("Particles")]
    [SerializeField]
    private ParticleSystem startParticles;

    [SerializeField]
    private ParticleSystem loopParticles;

    [SerializeField]
    private ParticleSystem endParticles;

    private MBAbility ability;

    /* message */ void Awake() {
        this.ability = this.GetComponent<MBAbility>();
    }
    
    /* message */ void Start() {
        this.ability.InvokeOnUseAsync((entity, target) => {
            if (this.startParticles != null) this.startParticles.Play();
            if (this.loopParticles != null) this.loopParticles.Play();
        });

        this.ability.InvokeOnDoneAsync((entity, target) => {
            if (this.startParticles != null) this.endParticles.Play();
            if (this.loopParticles != null) this.loopParticles.Stop();
        });
    }
}
