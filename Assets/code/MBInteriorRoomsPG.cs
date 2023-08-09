using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MBGrid))]
public class MBInteriorRoomsPG: MonoBehaviour {
        
    [SerializeField]
    private uint rngState = 0xAABB;

    [SerializeField]
    private GameObject tileWall;

    [SerializeField]
    private GameObject tileFloor;

    [SerializeField]
    private GameObject tileDoor;

    [SerializeField]
    private Vector2Int minRoomSize = new Vector2Int(5, 5);

    [SerializeField]
    private int maxRecursionLevel = 10;

    private MBGrid grid;
    private CompositeCollider2D collider;
    /* message */ void Awake() {
        this.grid = this.GetComponent<MBGrid>();
        this.collider = this.GetComponent<CompositeCollider2D>();
    }

    /* message */ void Start() {

        for (int x = this.grid.rect.min.x; x < this.grid.rect.max.x; ++x) {
            this.grid.InstantiateAt(tileWall, 
                new Vector2Int(x, this.grid.rect.min.y)
            );

            this.grid.InstantiateAt(tileWall, 
                new Vector2Int(x, this.grid.rect.max.y - 1)
            );
        }

        for (int y = this.grid.rect.min.y + 1; y < this.grid.rect.max.y - 1; ++y) {
            this.grid.InstantiateAt(tileWall, 
                new Vector2Int(this.grid.rect.min.x, y)
            );

            this.grid.InstantiateAt(tileWall, 
                new Vector2Int(this.grid.rect.max.x - 1, y)
            );
        }

        GenerateRect(new RectInt(
                this.grid.rect.min.x + 1, this.grid.rect.min.y + 1,
                this.grid.rect.width - 2, this.grid.rect.height - 2
            ),
            /*false,*/ maxRecursionLevel
        );


        if (this.collider != null)
            this.collider.GenerateGeometry();
    }

    
    // returns wall position
    private int GenerateRect(
        RectInt rect, int recLevel
    ) {
        if (recLevel == 0) {
            for (int x = rect.min.x; x < rect.max.x; ++x)
            for (int y = rect.min.y; y < rect.max.y; ++y)
                this.grid.InstantiateAt(this.tileFloor, new Vector2Int(x, y));
            return 1 << 30;
        }

        bool isVertical = (JRNG.Xorshift32(ref this.rngState) & 1) == 0;

        int minSize = isVertical ? this.minRoomSize.y : this.minRoomSize.x;
        RectInt myRect = isVertical ? JMisc.FlipRectInt(rect) : rect;

        // will divide perpendicular to Ox

        int minWallX = myRect.min.x + minSize;
        int maxWallX = myRect.max.x - minSize;

        if (minWallX >= maxWallX) {
            return this.GenerateRect(rect/*, !isVertical*/, recLevel - 1);
        }

        int wallSpan = maxWallX - minWallX;
        int wallX = minWallX + 
            (int)((long)JRNG.Xorshift32(ref this.rngState) % wallSpan);

        RectInt lftRect = new RectInt(
            myRect.min.x, myRect.y, wallX - myRect.min.x, myRect.height
        );

        RectInt rhtRect = new RectInt(
            wallX + 1, myRect.y, myRect.max.x - wallX - 1, myRect.height
        );

        lftRect = isVertical ? JMisc.FlipRectInt(lftRect) : lftRect;
        rhtRect = isVertical ? JMisc.FlipRectInt(rhtRect) : rhtRect;

        int lftWall = this.GenerateRect(lftRect/*, !isVertical*/, recLevel - 1);
        int rhtWall = this.GenerateRect(rhtRect/*, !isVertical*/, recLevel - 1);

        int doorPos = lftWall;
        while (doorPos == lftWall || doorPos == rhtWall) 
            doorPos = myRect.min.y 
                + (int)((long)JRNG.Xorshift32(ref this.rngState) 
                    % myRect.height
                );

        for (int y = myRect.min.y; y < myRect.max.y; ++y)
            this.grid.InstantiateAt(
                y != doorPos ? this.tileWall : this.tileDoor,
                isVertical ? new Vector2Int(y, wallX) : new Vector2Int(wallX, y)
            );

        return wallX;
    }   
}


