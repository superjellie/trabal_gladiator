using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBAbility : MonoBehaviour {

	public delegate IEnumerator UseHandler(MBEntity master, Vector3 target);
	public delegate void AsyncHandler(MBEntity master, Vector3 target);

	public enum TargetSemantics {
		Target = 0,
		Direction = 1
	}

	[SerializeField]
	public /* readonly */ TargetSemantics targetSemantics;
	
	[SerializeField, HideInInspector]
	public /* readonly */ bool isRunning = false;
	[SerializeField, HideInInspector]
	public /* readonly */ float runStartTime = 0f;
	[SerializeField, HideInInspector]
	public /* set in abilities on awake */ float runDuration = -1f;

	private List<UseHandler> routines = new List<UseHandler>();
	private List<AsyncHandler> startAsyncRoutines = new List<AsyncHandler>();
	private List<AsyncHandler> stopAsyncRoutines = new List<AsyncHandler>();

	public IEnumerator Use(MBEntity entity, Vector3 target) {
		if (entity == null) yield return null;
		if (this.isRunning) yield return null;
		this.isRunning = true;
		this.runStartTime = Time.time;

		foreach (AsyncHandler hdlr in this.startAsyncRoutines)
			hdlr.Invoke(entity, target);

		List<Coroutine> coroutines = new List<Coroutine>();
		foreach (UseHandler rtn in this.routines)
			coroutines.Add(this.StartCoroutine(rtn.Invoke(entity, target)));
		foreach (Coroutine crtn in coroutines)
			yield return crtn;

		foreach (AsyncHandler hdlr in this.stopAsyncRoutines)
			hdlr.Invoke(entity, target);

		this.isRunning = false;
	}

	public void UseAsync(MBEntity entity, Vector3 target) {
		this.StartCoroutine(this.Use(entity, target));
	}

	public void InvokeOnUse(UseHandler routine) {
		this.routines.Add(routine);
	}

	public void InvokeOnUseAsync(AsyncHandler handler) {
		this.startAsyncRoutines.Add(handler);
	}

	public void InvokeOnDoneAsync(AsyncHandler handler) {
		this.stopAsyncRoutines.Add(handler);
	}

	public float GetRunTime() { return Time.time - this.runStartTime; }

	public void PlaceTo(
        ref MBAbility ability, Transform parent
    ) {
        if (ability == this) return;
        if (ability != null) GameObject.Destroy(ability);
        ability = GameObject.Instantiate(this.gameObject, parent)
            .GetComponent<MBAbility>();
    }

}