using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SDamage {
	public EDamageTypeMask typeMask = EDamageTypeMask.Physical;
	public float amount = 1f;
}

[System.Serializable]
public class UnityEventSDamage : UnityEvent<SDamage> { }

[System.Flags]
public enum EDamageTypeMask {
	None     = 0b00000000,
	Physical = 0b00000001,
	Fire     = 0b00000010
} 