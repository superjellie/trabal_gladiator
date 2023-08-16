using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MBEffect : MonoBehaviour { 

	[SerializeField, HideInInspector]
	public MBEntity target;

	[SerializeField]
	private ApplyBehaviour applyBehaviour;

	[SerializeField]
	public UnityEvent onApply = new UnityEvent();

	[SerializeField]
	public UnityEvent onDestroy = new UnityEvent();

	public enum ApplyBehaviour {
		Stack = 0,
		KeepNew = 1,
		KeepOld = 2
	}

	/* message */ void Awake() {
		this.target = this.GetComponentInParent<MBEntity>();
		if (this.target == null) 
			Debug.Log("MBEffect: <color=red>No Target</color>");
	}

	/* message */ void Start() {
		this.onApply.Invoke();
	}

	/* message */ void Destroy() {
		this.onDestroy.Invoke();
	}

	public static T ApplyFromPrefab<T> (T prefab, MBEntity target)
		where T : MonoBehaviour {

		MBEffect effect = prefab.GetComponent<MBEffect>();
		if (effect == null) return null;

		if (effect.applyBehaviour == ApplyBehaviour.KeepNew) {
			IEnumerable<T> old = target.GetComponentsInChildren<T>();
			foreach (T ef in old) GameObject.Destroy(ef.gameObject);
		} 

		if (effect.applyBehaviour == ApplyBehaviour.KeepOld
			&& target.GetComponentInChildren<T>() != null) return null;

		GameObject go = GameObject.Instantiate(
			prefab.gameObject, target.transform
		);

		return go.GetComponent<T>();
	}

}
