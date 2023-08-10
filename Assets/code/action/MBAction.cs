using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBAction : MonoBehaviour {

	// using Routine : System.Func<IEnumerator> { }

	private List<System.Func<IEnumerator>> routines = 
		new List<System.Func<IEnumerator>>();

	public IEnumerator Perform() {
		List<Coroutine> coroutines = new List<Coroutine>();
		foreach (System.Func<IEnumerator> rtn in this.routines)
			coroutines.Add(this.StartCoroutine(rtn.Invoke()));
		foreach (Coroutine crtn in coroutines)
			yield return crtn;
	}

	public void InvokeOnPerform(System.Func<IEnumerator> routine) {
		this.routines.Add(routine);
	}

}