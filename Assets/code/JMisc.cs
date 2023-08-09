using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JMisc {

    public static RectInt FlipRectInt(RectInt rect) {
        return new RectInt(rect.y, rect.x, rect.height, rect.width);
    }

}
