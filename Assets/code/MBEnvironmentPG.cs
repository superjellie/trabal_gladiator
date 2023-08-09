using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBEnvironmentPG : MonoBehaviour {

    [SerializeField]
    private MBTag tagFloor;

    [SerializeField]
    private MBTag tagWall;

    [SerializeField]
    private GameObject envTorch;

    public struct MonsterSpawner {
        public GameObject spawner;
        public int minCount;
        public int maxCount;

        public MonsterSpawner(int x = 2) {
            this.spawner = null;
            this.minCount = 5;
            this.maxCount = 15;
        }
    }

    [SerializeField]
    private List<MonsterSpawner> monsterSpawners = new List<MonsterSpawner>();

    public void Generate() {

    }


}
