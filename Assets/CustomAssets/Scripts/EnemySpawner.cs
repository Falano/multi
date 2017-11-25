using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

// enemy spawner.
// the lvlSize that says where they spawn and move is on the colormanager (which is really a game manager)

public class EnemySpawner : NetworkBehaviour
{
    public GameObject enemyPrefab;
    public int enemyNumberTest;
    private int enemyNumber;
    private Vector3 lvlSize;
    int i;

    private Vector3 pos;
    private Quaternion rot;

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
        lvlSize = LvlSize.singleton.size;
        if (SceneManager.GetActiveScene().name == "testing") // pour que je n'ai pas à repasser par le menu si je veux juste tester un truc rapide
        {
            enemyNumber = enemyNumberTest;
        }
        else
        {
            enemyNumber = MenuManager.enemyNumber;
        }
        enemyList = new EnemyMover[enemyNumber - 1];
        for (i = 0; i < enemyNumber - 1; i++)
        {
            spawnEnemy();
        }
    }

    public void spawnEnemy()
    {
        NavMeshHit hit;
        pos = new Vector3(Random.Range(-lvlSize.x, lvlSize.x), Random.Range(0, lvlSize.y), Random.Range(-lvlSize.z, lvlSize.z));
        while (!NavMesh.SamplePosition(pos, out hit, 1, NavMesh.AllAreas)) // so they don't spawn up in the air or down belowhill
        {
            pos = new Vector3(Random.Range(lvlSize.x, -lvlSize.x), Random.Range(lvlSize.y, -lvlSize.y), Random.Range(lvlSize.z, -lvlSize.z));
        }
        rot = Quaternion.Euler(0, Random.Range(0, 180), 0);
        GameObject enemy = Instantiate(enemyPrefab, pos, rot);
        NetworkServer.Spawn(enemy);
        enemyList[i] = enemy.GetComponent<EnemyMover>();
    }
}
