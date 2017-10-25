using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class EnemySpawner : NetworkBehaviour {
    public GameObject enemyPrefab;
    public int enemyNumberTest;
    private int enemyNumber;

    public override void OnStartServer()
    {

        if (SceneManager.GetActiveScene().name == "testing")
        {
            enemyNumber = enemyNumberTest;
        }
        else
        {
            enemyNumber = MenuManager.enemyNumber;
        }
		for (int i = 0; i < enemyNumber-1; i++) {
            SpawnEnemy();
        }

        Invoke("SpawnEnemy", 0);
        
    }

    void SpawnEnemy() {
        var pos = new Vector3(Random.Range(-ColorManager.LvlSize.x, ColorManager.LvlSize.x), 1f, Random.Range(-ColorManager.LvlSize.z, ColorManager.LvlSize.z));
        var rot = Quaternion.Euler(0, Random.Range(0, 180), 0);
        var enemy = (GameObject)Instantiate(enemyPrefab, pos, rot);

        NetworkServer.Spawn(enemy);
    }

}
