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
    int i;

    GameObject ratKing;

    public static EnemyMover[] enemyList;

    public EnemySpawner singleton;

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(this);
        }
    }

    public override void OnStartServer()
    {
        //RpcMakeNetworkedRatKing();
        
        ratKing = makeSpawnable(ColorManager.singleton.emptyNetwPrefab, "ratKing");
        NetworkServer.Spawn(ratKing);

        lvlSize = ColorManager.singleton.LvlSize;

        if (SceneManager.GetActiveScene().name == "testing") // pour que je n'ai pas à repasser par le menu si je veux juste tester un truc rapide
        {
            enemyNumber = enemyNumberTest;
        }
        else
        {
            enemyNumber = MenuManager.enemyNumber;
        }

        enemyList = new EnemyMover[enemyNumber - 1];
		for (i = 0; i < enemyNumber-1; i++) {
            spawnEnemy();
        }        
    }

    public GameObject makeSpawnable(GameObject prefab, string name, string tag = "Untagged")
    {
        GameObject obj = Instantiate(prefab);
        obj.name = name;
        obj.tag = tag;
        return obj;
    }

    /*
    [ClientRpc]
    public void RpcMakeNetworkedRatKing() { MakeNetworkedRatKingSolo(); }

    public void MakeNetworkedRatKingSolo()
    {
        ratKing = Instantiate(ColorManager.singleton.emptyNetwPrefab);
        ratKing.name = "ratKing";
    }
    */


    public void spawnEnemy()
    {
        Vector3 pos = new Vector3(Random.Range(-lvlSize.x, lvlSize.x), Random.Range(0, lvlSize.y), Random.Range(-lvlSize.z, lvlSize.z));
        Quaternion rot = Quaternion.Euler(0, Random.Range(0, 180), 0);
        GameObject enemy = Instantiate(enemyPrefab, pos, rot, ratKing.transform);
        NetworkServer.Spawn(enemy);
        enemyList[i] = enemy.GetComponent<EnemyMover>();
    }
}
