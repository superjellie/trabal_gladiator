using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBTag : MonoBehaviour { 

    [SerializeField]
    private MBTag parentTag;

    public static bool HasTag(GameObject go, MBTag tag, int rec = 5) {
        if (rec > 0 && go.TryGetComponent<MBTag>(out MBTag goTag)) {
            return goTag == tag 
                || (goTag.parentTag != null
                    && MBTag.HasTag(goTag.parentTag.gameObject, tag, rec - 1)
                );
        }
        return false;
    }

}
