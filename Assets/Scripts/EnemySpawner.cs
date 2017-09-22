using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemySpawner : NetworkBehaviour {
    public GameObject enemyPrefab;
    public int numEnemies = 10;

    public override void OnStartServer()
    {
        for (int i = 0; i < numEnemies; i++) {
            SpawnEnemy();
        }

        InvokeRepeating("SpawnEnemy", 10, 10);
    }

    void SpawnEnemy() {
        var pos = new Vector3(Random.Range(-10.0f, 10.0f), 1f, Random.Range(-10.0f, 10.0f));
        var rot = Quaternion.Euler(0, Random.Range(0, 180), 0);
        var enemy = (GameObject)Instantiate(enemyPrefab, pos, rot);

        NetworkServer.Spawn(enemy);
    }

}
