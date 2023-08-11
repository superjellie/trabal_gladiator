using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBAbility : MonoBehaviour {

	public delegate IEnumerator UseHandler(MBEntity master, Vector3 target);

	public enum TargetSemantics {
		Target = 0,
		Direction = 1
	}

	[SerializeField]
	public /* readonly */ TargetSemantics targetSemantics;

	private List<UseHandler> routines = new List<UseHandler>();

	public IEnumerator Use(MBEntity entity, Vector3 target) {
		Debug.Log("Ability Use");
		List<Coroutine> coroutines = new List<Coroutine>();
		foreach (UseHandler rtn in this.routines)
			coroutines.Add(this.StartCoroutine(rtn.Invoke(entity, target)));
		foreach (Coroutine crtn in coroutines)
			yield return crtn;
	}

	public void InvokeOnUse(UseHandler routine) {
		this.routines.Add(routine);
	}
}