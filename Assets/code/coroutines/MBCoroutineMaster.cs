using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MBCoroutineMaster : MonoBehaviour {

	public /* readonly */ bool debug = false;

	[System.Serializable]
	public class Routine {
		public /* debugonly */ string name;
		public /* readonly */ IEnumerator it;
		public /* readonly */ UnityEvent onFinish = new UnityEvent();
		public /* private */ bool moveMe = true;
		public /* private */ bool wasInterrupted = false;
	};

	[SerializeField]
	private Stack<Routine> routineStack = new Stack<Routine>();

	public Routine Push(string name, IEnumerator it) {
		Routine rtn = new Routine () { 
			name = name, 
			it = it
		};
		this.routineStack.Push(rtn);
		this.StartCoroutine(this.MoveByUnityWait(rtn, null));
		return rtn;
	}

	private void Pop() {
		if (this.routineStack.TryPeek(out Routine rtn)) {
			rtn.onFinish.Invoke();
			this.routineStack.Pop();
		}
	}

	private void Move() {

		while (this.routineStack.TryPeek(out Routine rtn)) {
			if (!rtn.moveMe) return;

			rtn.moveMe = false;
			if (this.debug) Debug.Log("MBCoroutineMaster: Advance " + rtn.name);
			bool running = rtn.it.MoveNext();


			object waitFor = rtn.it.Current;

			if (!running) {
				this.Pop();
				continue;
			}
			
			if (waitFor is UnityEvent) {
				if (this.debug) Debug.Log("MBCoroutineMaster: " 
					+ rtn.name + " waits for UnityEvent " 
					+ ((UnityEvent)waitFor).ToString());
				((UnityEvent)waitFor).AddListener(
					() => this.MoveRoutine(rtn)
				);
			} else if (waitFor is Routine) { 
				if (this.debug) Debug.Log("MBCoroutineMaster: " 
					+ rtn.name + " waits for " + ((Routine)waitFor).name);
				((Routine)waitFor).onFinish.AddListener(
					() => this.MoveRoutine(rtn)
				);
			} else {
				if (this.debug) Debug.Log("MBCoroutineMaster: " 
					+ rtn.name + " waits for " + waitFor.ToString());
				this.StartCoroutine(this.MoveByUnityWait(rtn, waitFor));
			}
		}
	}

	public bool WasInterrupted() {
		if (this.routineStack.Count == 0) return false;
		Routine rtn = this.routineStack.Peek();
		bool result = rtn.wasInterrupted;
		rtn.wasInterrupted = false;
		return result;
	}

	public void InterruptParentRoutine() {
		if (this.routineStack.Count < 2) return;
		Routine top = this.routineStack.Peek();
		Routine rtn = this.routineStack.Skip(1).First();
		if (this.debug) Debug.Log("MBCoroutineMaster: " + top.name 
			+ " was interrupted by " + rtn.name);
		rtn.wasInterrupted = true;
	}

	private void MoveRoutine(Routine rtn) {
		rtn.moveMe = true;
		if (this.routineStack.TryPeek(out Routine topRtn))
			if (topRtn == rtn)
				this.Move();
	}

	private IEnumerator MoveByUnityWait (Routine rtn, object waitFor) {
		yield return waitFor;
		this.MoveRoutine(rtn);
	}
}
