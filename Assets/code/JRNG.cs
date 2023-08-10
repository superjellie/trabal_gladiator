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

    public static float Xorshift32f(ref uint state) {
        uint rndInt = JRNG.Xorshift32(ref state) % (1 << 20);
        return (float) rndInt / (float) (1 << 20);
    }

    public static Vector2 Xorshift32fInUnitCircle(ref uint state) {
        float r     = Mathf.Sqrt(JRNG.Xorshift32f(ref state));
        float theta = 2 * JRNG.Xorshift32f(ref state) * Mathf.PI;
        return new Vector2(r * Mathf.Cos(theta), r * Mathf.Sin(theta));
    }

}
