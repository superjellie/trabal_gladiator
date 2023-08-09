using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JRNG {

    public static uint Xorshift32(ref uint state) {
        /* Algorithm "xor" from p. 4 of Marsaglia, "Xorshift RNGs" */
        uint x = state;
        x ^= x << 13;
        x ^= x >> 17;
        x ^= x << 5;
        state = x;
        return x;
    }
    

}
