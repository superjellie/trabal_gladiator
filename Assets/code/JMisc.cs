using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JMisc {

    public static RectInt FlipRectInt(RectInt rect) {
        return new RectInt(rect.y, rect.x, rect.height, rect.width);
    }

    [System.Serializable]
    public struct Tuple<T1, T2> {
        public T1 item1;
        public T2 item2;
    }

    [System.Serializable]
    public struct Tuple<T1, T2, T3> {
        public T1 item1;
        public T2 item2;
        public T3 item3;
    }

}
