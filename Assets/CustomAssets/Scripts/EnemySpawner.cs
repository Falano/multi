using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

// enemy spawner.
// the lvlSize that says where they spawn and move is on the colormanager (which is really a game manager)

public class EnemySpawner : NetworkBehaviour {
    public GameObject enemyPrefab;
    public int enemyNumberTest;
    private int enemyNumber;
    private Vector3 lvlSize;

    public override void OnStartServer()
    {

        if (SceneManager.GetActiveScene().name == "testing") // pour que je n'ai pas à repasser par le menu si je veux juste tester un truc rapide
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


    private void Start()
    {
        lvlSize = GameObject.FindGameObjectWithTag("ColorManager").GetComponent<ColorManager>().LvlSize;
    }

    void SpawnEnemy() {
        var pos = new Vector3(Random.Range(-lvlSize.x, lvlSize.x), 1f, Random.Range(-lvlSize.z, lvlSize.z));
        var rot = Quaternion.Euler(0, Random.Range(0, 180), 0);
        var enemy = (GameObject)Instantiate(enemyPrefab, pos, rot);

        NetworkServer.Spawn(enemy);
    }

}
