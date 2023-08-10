using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBEnvironmentPG : MonoBehaviour {

    [SerializeField]
    private uint rngState = 0xAABB;

    [SerializeField]
    private GameObject envPrefab;

    [SerializeField]
    private int minCount = 5;

    [SerializeField]
    private int maxCount = 10;

    [SerializeField]
    private MBTag placeOn;

    [SerializeField]
    private MBGrid bgGrid;

    [SerializeField]
    private MBGrid envGrid;

    public void Generate() {
        if (this.bgGrid  == null) return;
        if (this.envGrid == null) return;

        int minX = JMisc.MaxI(this.bgGrid.rect.min.x, this.envGrid.rect.min.x);
        int maxX = JMisc.MinI(this.bgGrid.rect.max.x, this.envGrid.rect.max.x);
        int minY = JMisc.MaxI(this.bgGrid.rect.min.y, this.envGrid.rect.min.y);
        int maxY = JMisc.MinI(this.bgGrid.rect.max.y, this.envGrid.rect.max.y);

        int width  = maxX - minX;
        int height = maxY - minY;
        int countSpan = this.maxCount - this.minCount + 1;

        int count = this.minCount 
            + (int)((long)JRNG.Xorshift32(ref this.rngState) % countSpan);

        Vector2Int[] sample = new Vector2Int[count];
        int index = 0;

        for (int x = minX; x < maxX; ++x)
        for (int y = minY; y < maxY; ++y) {
            Vector2Int pos = new Vector2Int(x, y);
            GameObject tile = bgGrid.GetObjectAt(pos);
            GameObject env = envGrid.GetObjectAt(pos);
            if (env != null || !MBTag.HasTag(tile, this.placeOn)) continue;
            int targetSampleIdx = index < count ? index 
                : (int)((long)JRNG.Xorshift32(ref this.rngState) % (index + 1));
            if (targetSampleIdx < count) sample[targetSampleIdx] = pos;
            index++;
        }

        foreach (Vector2Int pos in sample) 
            envGrid.InstantiateAt(envPrefab, pos);
    }


}
