using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JMisc {

    public static RectInt FlipRectInt(RectInt rect) {
        return new RectInt(rect.y, rect.x, rect.height, rect.width);
    }

    public static int MinI(int x, int y) { return x < y ? x : y; }
    public static int MaxI(int x, int y) { return x < y ? y : x; }
}
