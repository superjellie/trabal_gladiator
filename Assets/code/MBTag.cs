using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBTag : MonoBehaviour { 

    [SerializeField]
    private MBTag parentTag;

    public static bool HasTag(GameObject go, MBTag tag, int rec = 5) {
        if (go == null || tag == null) return false;
        if (rec == 0) return false;

        IEnumerable<MBTag> goTags = go.GetComponents<MBTag>();
        foreach (MBTag goTag in goTags)
            if (goTag == tag || (
                    goTag.parentTag != null
                    && MBTag.HasTag(goTag.parentTag.gameObject, tag, rec - 1)
                )
            ) return true;
        
        return false;
    }

}
