using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class JRoutine {
	public /* debugonly */ string name;
	public /* readonly */ IEnumerator it;
	public /* readonly */ UnityEvent onFinish = new UnityEvent();
	public /* private */ bool moveMe = true;
	public /* private */ bool wasInterrupted = false;
	public /* private */ bool removeMe = false;
	public /* private */ GameObject master;
};

public class MBCoroutineMaster : MonoBehaviour {

	public /* readonly */ bool debug = false;

	[SerializeField]
	private Stack<JRoutine> routineStack = new Stack<JRoutine>();

	public JRoutine Push(
		IEnumerator it, GameObject master, string name = "noname"
	) {
		JRoutine rtn = new JRoutine () { 
			name = name, 
			it = it,
			master = master
		};
		this.routineStack.Push(rtn);
		this.StartCoroutine(this.MoveByUnityWait(rtn, null));
		return rtn;
	}

	public void Stop(JRoutine rtn) {
		rtn.removeMe = true;
	}

	private void Pop() {
		if (this.routineStack.TryPeek(out JRoutine rtn)) {
			rtn.onFinish.Invoke();
			this.routineStack.Pop();
		}
	}

	/* message */ void Update() {

		if (this.routineStack.Count == 0) return;
		JRoutine rtn = this.routineStack.Peek();

		if (rtn.removeMe || rtn.master == null) {
			this.Pop();
			return;
		}
			
		if (!rtn.moveMe) return;

		rtn.moveMe = false;
		if (this.debug) Debug.Log("MBCoroutineMaster: Advance " + rtn.name);
		bool running = rtn.it.MoveNext();

		object waitFor = rtn.it.Current;

		if (!running) {
			this.Pop();
			return;
		}
		
		if (waitFor is UnityEvent && waitFor != null) {
			if (this.debug) Debug.Log("MBCoroutineMaster: " 
				+ rtn.name + " waits for UnityEvent " 
				+ ((UnityEvent)waitFor).ToString());
			((UnityEvent)waitFor).AddListener(
				() => rtn.moveMe = true
			);
		} else if (waitFor is JRoutine && waitFor != null) { 
			if (this.debug) Debug.Log("MBCoroutineMaster: " 
				+ rtn.name + " waits for " + ((JRoutine)waitFor).name);
			((JRoutine)waitFor).onFinish.AddListener(
				() => rtn.moveMe = true
			);
		} else {
			if (this.debug) Debug.Log("MBCoroutineMaster: " 
				+ rtn.name + " waits for " + waitFor.ToString());
			this.StartCoroutine(this.MoveByUnityWait(rtn, waitFor));
		}
	}

	public bool WasInterrupted() {
		if (this.routineStack.Count == 0) return false;
		JRoutine rtn = this.routineStack.Peek();
		bool result = rtn.wasInterrupted;
		rtn.wasInterrupted = false;
		return result;
	}

	public void InterruptParentRoutine() {
		if (this.routineStack.Count < 2) return;
		JRoutine top = this.routineStack.Pop();
		JRoutine rtn = this.routineStack.Peek();
		this.routineStack.Push(top);
		if (this.debug) Debug.Log("MBCoroutineMaster: " + top.name 
			+ " was interrupted by " + rtn.name);
		rtn.wasInterrupted = true;
	}

	private IEnumerator MoveByUnityWait (JRoutine rtn, object waitFor) {
		yield return waitFor;
		rtn.moveMe = true;
	}
}
