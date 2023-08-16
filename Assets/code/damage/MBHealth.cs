using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MBHealth : MonoBehaviour {

	[SerializeField]
	private float maxHealth = 10.0f;

	[SerializeField]
	private float currentHealth = 10.0f;

	[SerializeField]
	public /* listen only */ UnityEventSDamage onRecieveDamage = 
		new UnityEventSDamage();

	[SerializeField]
	public /* listen only */ UnityEvent onDeath = new UnityEvent();

	[SerializeField]
	private GameObject replaceOnDeath;

	/* message */ void Awake() {
		this.onRecieveDamage.AddListener(damage => 
			this.StartCoroutine(this.DelayDamage(damage))
		);
		
		this.onDeath.AddListener(() => {
			if (this.replaceOnDeath != null)
				GameObject.Instantiate(
					this.replaceOnDeath, 
					this.transform.position, 
					this.transform.rotation
				);
			GameObject.Destroy(this.gameObject);
		});
	}

	public void ApplyDamage(SDamage damage) {
		this.onRecieveDamage.Invoke(damage);
	}

	private IEnumerator DelayDamage(SDamage damage) {
		yield return null;
		this.currentHealth -= damage.amount;
		this.currentHealth = this.currentHealth >= this.maxHealth?
			this.maxHealth : this.currentHealth;
		if (this.currentHealth <= 0f) 
			this.onDeath.Invoke();
	}


}