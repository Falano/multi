using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// à mettre sur le color manager
// dit à tout le monde que le mouton a changé de couleur (rpc) (probs car n'est pas localPlayerAuthority?)
// dit à tout le monde que le player a changé d'état ready (rpc) (probs car n'est pas localPlayerAuthority?)
// contient la fonction finale Kill qui désactive le renderer du mouton et active death anim; the object destroy itself is on a script on the child
// LaunchGame and RefreshListOfPlayers are essentially a lobby

[RequireComponent(typeof(NetworkIdentity))] //everything unchecked
public class ColorManager : NetworkBehaviour
{
    public Vector3 LvlSize;
    public static ColorManager singleton;
    public bool isPlayerDead = false;
    public static bool isGamePlaying = false;
    public static Score[] listPlayers;
    [Header("scripts drag-and-drop variables")]
    public Canvas lobbyCanvas;
    public Text launchGameTx;
    public GameObject listOfPlayersParent;
    public GameObject playerStatePrefab;

    private float refreshFrequency = 2.5f;

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
        listPlayers = new Score[MenuManager.maxPlayersNumber];
        InvokeRepeating("RefreshListOfPlayers", 0, refreshFrequency);
        launchGameTx.text = "";
    }



    [ClientRpc]
    public void RpcChangeCol(GameObject obj, Color col/*, GameObject attacker*/)
    {
        obj.GetComponent<PlayerHealth>().TakeDamage();
        if (obj.GetComponent<PlayerHealth>().Hp > 0)
        { // pour que la flaque de peinture soit de la dernière couleur vue et pas d'une nouvelle couleur random (cf Kill() ci-dessous)
            Renderer rd = obj.GetComponentInChildren<Renderer>();
            foreach (Material mat in rd.materials)
            {
                mat.color = col;
            }

            // if I keep this, after the second player attack, sheep bleed to death FOR SOME REASON
            /*
            if (attacker == obj)
            {
                obj.GetComponent<ScoreKeeper>().currentPlayer.colorChangesFromSelf += 1;
            }
            else if (attacker.CompareTag("AttackChangeCol"))
            {
                obj.GetComponent<ScoreKeeper>().currentPlayer.colorChangesFromMice += 1;
            }
            else if (attacker.CompareTag("Player"))
            {
                obj.GetComponent<ScoreKeeper>().currentPlayer.colorChangesFromOthers += 1;
                attacker.GetComponent<ScoreKeeper>().currentPlayer.colorChangesToOthers += 1;
            }
            */
        }
    }



    public void KillSolo(GameObject obj)
    {
        if (isLocalPlayer)
        {
            isPlayerDead = true;
        }
        obj.GetComponent<PlayerMove>().speed = 0;
        GameObject mesh = obj.transform.GetChild(0).gameObject;
        mesh.SetActive(false);
        GameObject death = obj.transform.GetChild(2).gameObject;
        death.SetActive(true);
        //        Score player = obj.GetComponent<ScoreKeeper>().currentPlayer;
        //        player.SetTimeOfDeath(); // pour le score
        //        CameraMover.singleton.activePlayer = null; // pour si la caméra ne comprend pas qu'il est mort
        //        print(player.PlayerName + " est mort après " + player.TimeOfDeath + "secondes." );
        //        print("You dissolved into paint after " + player.TimeOfDeath.ToString("F1") + " seconds. You changed colour " + player.colorChangesFromMice + " times because of mice, " + player.colorChangesFromOthers + " times because of other players, " + player.colorChangesFromSelf  + " times of your own volition, and you made other players change colour " + player.colorChangesToOthers + " times." );
        death.GetComponent<SpriteRenderer>().color = mesh.GetComponent<Renderer>().material.color;
        //print("deathcol = " + death.GetComponent<SpriteRenderer>().color);
        //print("meshcol = " + mesh.GetComponent<Renderer>().material.color);
        obj.GetComponent<BoxCollider>().enabled = false; //careful il y a deux box colliders, l'un trigger; ne pas changer leur place
                                                         //the object destroy itself is on a script on the child
    }
    [Command]
    void CmdKill(GameObject obj) { RpcKill(obj); }
    [ClientRpc]
    void RpcKill(GameObject obj) { KillSolo(obj); }
    public void Kill(GameObject obj) { CmdKill(obj); }


    public IEnumerator launchingGame() // un message d'erreur dit qu'il ne sait pas se lancer sur le host? mais ça marche quand même, so whatever
    {
        launchGameTx.text = "Launching Game...";
        yield return new WaitForSeconds(4);
        LaunchGame();
    }

    public void LaunchGameSolo()
    {
        CancelInvoke("RefreshListOfPlayers");
        isGamePlaying = true;
        launchGameTx.text = "";
        listOfPlayersParent.SetActive(false);
        if (isServer)
        {
            foreach (EnemyMover enemy in EnemySpawner.enemyList)
            {
                IEnumerator wait = enemy.waitForChangeDir(Random.Range(enemy.waitRange.x, enemy.waitRange.y));
                StartCoroutine(wait); //it works ONLY IF I create the coroutine on the previous line and set it up in here instead of in EnemyMover
            }
        }
        lobbyCanvas.enabled = false;
    }
    [Command]
    void CmdLaunchGame()
    {
        RpcLaunchGame(); // 2) de dire aux clients
    }
    [ClientRpc]
    void RpcLaunchGame()
    {
        LaunchGameSolo(); // 3) de lancer le jeu
    }
    void LaunchGame()
    {
        CmdLaunchGame(); // 1) on dit au server
    }


    [ClientRpc]
    public void RpcTogglePlayerReady(GameObject player, bool state)
    {
        player.GetComponent<Score>().ToggleReadySolo(state);
    }



    public void RefreshListOfPlayers()
    {
        print("refreshing list of players");
        int numberOfPlayersReady = 0;
        GameObject[] listPlayersGO = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < listPlayers.Length; i++)
        {
            if (i >= listPlayersGO.Length)
            {
                listPlayers[i] = null;
            }
            else
            {
                listPlayers[i] = listPlayersGO[i].GetComponent<Score>();
                listPlayers[i].SetI(i);
                if (listPlayers[i].PlayerName == null)
                {
                    listPlayers[i].SetPlayersName("Player" + i.ToString());
                }
                string readyState = "not ready...";
                Color txColor = Color.white;
                if (listPlayers[i].IsReady == true)
                {
                    readyState = "ready!";
                    numberOfPlayersReady++;
                    txColor = Color.green;
                }
                //print(listPlayers[i].PlayerName + " : " + readyState);
                if(listPlayers[i].ScoreTx == null)
                {
                    float posX = listOfPlayersParent.transform.position.x;
                    float posY = listOfPlayersParent.transform.position.y;
                    listPlayers[i].ScoreTx = Instantiate(playerStatePrefab, listOfPlayersParent.transform);
                    listPlayers[i].ScoreTx.transform.position = new Vector2(posX, posY - 20 + i * -20);
                }
                listPlayers[i].ScoreTx.GetComponent<Text>().text = listPlayers[i].PlayerName + " : " + readyState;
                listPlayers[i].ScoreTx.GetComponent<Text>().color = txColor;
            }            
        }
        if (numberOfPlayersReady == listPlayersGO.Length)
        {
            StartCoroutine("launchingGame");
        }
    }
}