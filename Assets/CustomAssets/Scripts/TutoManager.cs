using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutoManager : ColorManager
{

    new public static TutoManager singleton;
    public static bool isInTuto;
    public static string tutoName = "7";
    public Text tutoNarr;
    public float speechDuration = 3;
    public static TextMesh tutoText;


    new void Awake()
    {
        base.Awake();

        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(this);
        }

        if (SceneManager.GetActiveScene().name == (tutoName))// cuz I'm using it in EnemySpawner's Awake
        {
            isInTuto = true;
        }
    }

    // Use this for initialization
    new void Start()
    {
        base.Start();

        MenuManager.soloGame = true;
        foreach (GameObject gui in GameObject.FindGameObjectsWithTag("GUI"))
        {
            switch (gui.name)
            {
                case "tutoNarr":
                    tutoNarr = gui.GetComponent<Text>();
                    break;
            }
        }
        tutoSpeech(5, "press //nMenuManager.InteractKey.ToString() when you're ready. The game starts //n when every logged in player is ready (even if everyone //nyou planned to play with hasn't logged in yet). //n//n This tutorial is offline though, so don't worry about it for now, //njust remember: MenuManager.InteractKey.ToString() when you're ready.", tutoNarr);
    }

    public void ChangeCol(GameObject obj, Color col, GameObject attacker)
    {
        Score score;

        score = obj.GetComponent<TutoBehaviour>().ScoreObj.GetComponent<Score>();

        obj.GetComponent<TutoHealth>().TakeDamage();
        TutoChangeCol objChangeCol = obj.GetComponent<TutoChangeCol>();
        if (isGamePlaying)
        { // sound stuff
            AudioSource sound = obj.GetComponent<AudioSource>();
            sound.clip = ChangeColSounds[Random.Range(0, ChangeColSounds.Length)];
            sound.Play();
        }
        if (obj.GetComponent<TutoHealth>().Hp > 0)
        { // pour que la flaque de peinture soit de la dernière couleur vue et pas d'une nouvelle couleur random (cf Kill() ci-dessous)
            Renderer rd = obj.GetComponentInChildren<Renderer>();
            foreach (Material mat in rd.materials)
            {
                mat.color = col;
            }

            IEnumerator paintCooldownNow = paintCooldown(objChangeCol.cooldown, attacker);
            StartCoroutine(paintCooldownNow);

            if (attacker == obj)
            {
                score.colorChangesFromSelf += 1;
            }
            else if (attacker.CompareTag("AttackChangeCol"))
            {
                score.colorChangesFromMice += 1;
            }
            else if (attacker.CompareTag("Player"))
            {
                score.colorChangesFromOthers += 1;
                attacker.GetComponent<TutoBehaviour>().ScoreObj.GetComponent<Score>().colorChangesToOthers += 1;
                attacker.GetComponent<TutoChangeCol>().paintReady = false;
            }
            if (obj.GetComponent<TutoBehaviour>().isLocalPlayer)
            {
                IEnumerator speedBoostNow = speedBoost(objChangeCol.speedBoostDuration, objChangeCol.speedBoostStrength, obj, attacker);
                StartCoroutine(speedBoostNow);
            }
        }

        if (obj.CompareTag("Player"))
        {
            tutoSpeech(speechDuration, "I can run, and I can hide!", obj.GetComponentInChildren<Text>());
        }

        if (attacker == obj)
        {
            tutoSpeech(speechDuration, "Ow, I better not do that too often.", attacker.GetComponentInChildren<Text>());
            tutoSpeech(speechDuration * 3, "See the ball in the top-right corner? That's how many//ncolour changes you have left before you turn back to paint.", tutoNarr);
        }
        else if (attacker.CompareTag("Player"))
        {
            tutoSpeech(speechDuration, "so that's what I look like to others...", attacker.transform.GetComponentInChildren<Text>());
        }
    }


    new IEnumerator paintCooldown(float cooldown, GameObject attacker)
    {
        yield return new WaitForSeconds(cooldown);
        if (attacker.CompareTag("Player"))
        {
            attacker.GetComponent<TutoChangeCol>().paintReady = true;
        }
    }

    new protected IEnumerator speedBoost(float duration, float strength, GameObject obj, GameObject attacker)
    {
        TutoChangeCol objChangeCol = obj.GetComponent<TutoChangeCol>();
        if (obj == attacker) // so it's twice as expensive to speedBoost to chase someone (if you changed your own colour) as it is if you're running away (if you've been attacked)
        {
            duration *= .5f;
        }
        objChangeCol.currBoost += 1;
        int prevBoost = objChangeCol.currBoost;
        PlayerMove playerMove = obj.GetComponent<PlayerMove>();
        Animator animator = playerMove.animator;
        playerMove.speed = strength;
        animator.speed = 2;
        yield return new WaitForSeconds(duration);
        if (playerMove.speed == strength && prevBoost == objChangeCol.currBoost) // pour qu'il ne sache pas re-bouger s'il est en train de mourir
        {
            playerMove.speed = playerMove.BaseSpeed;
            animator.speed = 1;
        }
    }

    [ClientRpc]
    new public void RpcKill(GameObject obj)
    {
        obj.GetComponent<TutoHealth>().KillSolo();
    }


    new public void LaunchGameSolo()
    {
        print("launching game: " + localPlayer.name);

        Scores = ScoresHolderParent.GetComponentsInChildren<Score>();
        foreach (Score sco in Scores)
        {
            sco.ScoreTx = sco.PlayerObj.GetComponent<TutoBehaviour>().ScoreTx.GetComponent<Text>();
            sco.SetStartTime();
        }
        numberOfPlayersPlaying = GameObject.FindGameObjectsWithTag("Player").Length;
        isGamePlaying = true;
        localPlayer.GetComponent<PlayerMove>().speed = localPlayer.GetComponent<PlayerMove>().BaseSpeed;
        launchGameTx.text = "";
        listOfPlayersParent.SetActive(false);
        lobbyCanvas.enabled = false;
        if (isServer)
        {
            foreach (EnemyMover enemy in EnemySpawner.enemyList)
            {
                IEnumerator wait = enemy.waitForChangeDir(Random.Range(enemy.waitRange.x, enemy.waitRange.y));
                StartCoroutine(wait); //it works ONLY IF I create the coroutine on the previous line and set it up in here instead of in EnemyMover
            }
        }
    }


    new public void RefreshListOfPlayersSolo()
    {
        print("refreshing list of players");
        int numberOfPlayersReady = 0;
        GameObject[] listNPCs = GameObject.FindGameObjectsWithTag("tutoPlayerNPC");
        GameObject[] listPlayersGO = new GameObject[listNPCs.Length + 1];
        for (int i = 0; i < listPlayersGO.Length; i++)
        {
            if (i == listNPCs.Length)
            {
                listPlayersGO[i] = GameObject.FindGameObjectWithTag("Player");
            }
            else
            {
                listPlayersGO[i] = listNPCs[i];
            }
        }

        TutoBehaviour[] listPlayers = new TutoBehaviour[listPlayersGO.Length];
        for (int i = 0; i < listPlayers.Length; i++)
        {
            listPlayers[i] = listPlayersGO[i].GetComponent<TutoBehaviour>();
            listPlayers[i].idNumber = i;
            listPlayers[i].ScoreObj.GetComponent<Score>().idNumber = i;
            if (listPlayers[i].localName == null || listPlayers[i].localName == "")
            {
                listPlayers[i].localName = "Player" + i.ToString();
            }
            string readyState = "not ready...";
            Color txColor = Color.white;
            if (listPlayers[i].IsReady == true)
            {
                readyState = "ready!";
                numberOfPlayersReady++;
                txColor = Color.green;
            }
            //print(listPlayers[i].localName + " : " + readyState);
            if (listPlayers[i].ScoreTx == null)
            {
                float posX = listOfPlayersParent.transform.position.x;
                float posY = listOfPlayersParent.transform.position.y;
                int offset = (int)Mathf.Round(0.05f * Screen.height);
                listPlayers[i].ScoreTx = Instantiate(playerStatePrefab, listOfPlayersParent.transform);
                listPlayers[i].ScoreTx.transform.position = new Vector2(posX, posY - 20 + i * -offset);
            }
            listPlayers[i].ScoreTx.GetComponent<Text>().text = listPlayers[i].localName + " : " + readyState;
            listPlayers[i].ScoreTx.GetComponent<Text>().color = txColor;
        }
        if (numberOfPlayersReady == listPlayersGO.Length && isServer)
        {
            StartCoroutine("launchingGame");
        }
    }


    IEnumerator waitForGameEnd()
    {
        yield return new WaitForSeconds(1);
        /* //pour si on veut un titre; mais c'est chiant à aligner, donc non.
        launchGameTx.text = "Name/ Time of Death/ Player Changes/ Players Changed/ Mice changes/ Self Changes";
        launchGameTx.fontSize = (int) Mathf.Round(launchGameTx.fontSize*0.7f);
        launchGameTx.transform.position = new Vector2(launchGameTx.transform.position.x - Screen.width*2/5, launchGameTx.transform.position.y + Screen.height * 2 / 5);
        */
        lobbyCanvas.enabled = true;
        launchGameTx.text = "You're probably ready to play now!//nPress Escape then clic on Stop to go back to the menu";



    }


    [ClientRpc]
    new public void RpcSetLocalName(string name, GameObject obj)
    {
        obj.GetComponent<TutoBehaviour>().SetLocalNameSolo(name);
    }



    [ClientRpc]
    new public void RpcTogglePlayerReady(GameObject player, bool state)
    {
        player.GetComponent<TutoBehaviour>().ToggleReadySolo(state);
    }


    [ClientRpc]
    new public void RpcLaunchGameTx()
    {
        launchGameTx.text = "Launching Game...";
        localPlayer.GetComponent<TutoChangeCol>().startWhite();
    }

    public void tutoSpeech(float time, string sentence, Text textObj)
    {
        StopCoroutine("endSpeak");
        textObj.text = sentence;
        StartCoroutine(endSpeak(time, textObj));
    }

    public void tutoSpeech(float time, string sentence, TextMesh textObj)
    {
        StopCoroutine("endSpeak");
        textObj.text = sentence;
        StartCoroutine(endSpeak(time, textObj));
    }

    IEnumerator endSpeak(float time, Text textObj)
    {
        yield return new WaitForSeconds(time);
        textObj.text = "";
    }

    IEnumerator endSpeak(float time, TextMesh textObj)
    {
        yield return new WaitForSeconds(time);
        textObj.text = "";
    }

    private void tutoSpeechLaunch()
    {
        tutoSpeech(speechDuration, "I should try pressing MenuManager.MenuKey.ToString() several times...", tutoText);
    }


}