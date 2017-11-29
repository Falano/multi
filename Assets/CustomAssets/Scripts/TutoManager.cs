using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutoManager : ColorManager {

    public static TutoManager singleton;
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
    new void Start () {
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
        tutoSpeech(15, "press MenuManager.InteractKey.ToString() when you're ready. The game starts //n when every logged in player is ready (even if everyone //nyou planned to play with hasn't logged in yet). //n//n This tutorial is offline though, so don't worry about it for now, //njust remember: MenuManager.InteractKey.ToString() when you're ready.", tutoNarr);
    }
	
    public void ChangeCol(GameObject obj, Color col, GameObject attacker)
    {
        base.RpcChangeCol(obj, col, attacker);


        if (obj.GetComponent<PlayerBehaviour>().isLocalPlayer)
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
                tutoSpeech(speechDuration, "so that's what I look like to others...", attacker.
                    transform.parent.GetComponentInChildren<Text>());
        }
    }


    public void tutoSpeech(float time, string sentence, Text textObj)
    {
        StopCoroutine("endSpeak");
        textObj.text = sentence;
        IEnumerator endSpeakNow = endSpeak(time, textObj);
        StartCoroutine("endSpeakNow");
    }

    public void tutoSpeech(float time, string sentence, TextMesh textObj)
    {
        StopCoroutine("endSpeak");
        textObj.text = sentence;
        IEnumerator endSpeakNow = endSpeak(time, textObj);
        StartCoroutine("endSpeakNow");
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


    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////:
    ///// </summary>
    //public void LaunchGameSolo()
    //{
    //    print("launching game: " + localPlayer.name);

    //    Scores = ScoresHolderParent.GetComponentsInChildren<Score>();
    //    foreach (Score sco in Scores)
    //    {
    //        sco.ScoreTx = sco.PlayerObj.GetComponent<PlayerBehaviour>().ScoreTx.GetComponent<Text>();
    //        sco.SetStartTime();
    //    }
    //    numberOfPlayersPlaying = GameObject.FindGameObjectsWithTag("Player").Length;
    //    isGamePlaying = true;
    //    localPlayer.GetComponent<PlayerMove>().speed = localPlayer.GetComponent<PlayerMove>().BaseSpeed;
    //    launchGameTx.text = "";
    //    listOfPlayersParent.SetActive(false);
    //    lobbyCanvas.enabled = false;
    //    if (isServer)
    //    {
    //        foreach (EnemyMover enemy in EnemySpawner.enemyList)
    //        {
    //            IEnumerator wait = enemy.waitForChangeDir(Random.Range(enemy.waitRange.x, enemy.waitRange.y));
    //            StartCoroutine(wait); //it works ONLY IF I create the coroutine on the previous line and set it up in here instead of in EnemyMover
    //        }
    //    }
    //}



    //[ClientRpc]
    //public void RpcChangeCol(GameObject obj, Color col, GameObject attacker)
    //{
    //    Score score = obj.GetComponent<PlayerBehaviour>().ScoreObj.GetComponent<Score>();
    //    obj.GetComponent<PlayerHealth>().TakeDamage();
    //    PlayerChangeCol objChangeCol = obj.GetComponent<PlayerChangeCol>();
    //    if (isGamePlaying)
    //    { // sound stuff
    //        AudioSource sound = obj.GetComponent<AudioSource>();
    //        sound.clip = ChangeColSounds[Random.Range(0, ChangeColSounds.Length)];
    //        sound.Play();
    //    }
    //    if (obj.GetComponent<PlayerHealth>().Hp > 0)
    //    { // pour que la flaque de peinture soit de la dernière couleur vue et pas d'une nouvelle couleur random (cf Kill() ci-dessous)
    //        Renderer rd = obj.GetComponentInChildren<Renderer>();
    //        foreach (Material mat in rd.materials)
    //        {
    //            mat.color = col;
    //        }

    //        IEnumerator paintCooldownNow = paintCooldown(objChangeCol.cooldown, attacker);
    //        StartCoroutine(paintCooldownNow);

    //        if (attacker == obj)
    //        {
    //            score.colorChangesFromSelf += 1;
    //        }
    //        else if (attacker.CompareTag("AttackChangeCol"))
    //        {
    //            score.colorChangesFromMice += 1;
    //        }
    //        else if (attacker.CompareTag("Player"))
    //        {
    //            score.colorChangesFromOthers += 1;
    //            attacker.GetComponent<PlayerBehaviour>().ScoreObj.GetComponent<Score>().colorChangesToOthers += 1;
    //            attacker.GetComponent<PlayerChangeCol>().paintReady = false;
    //        }
    //        if (obj.GetComponent<PlayerBehaviour>().isLocalPlayer)
    //        {
    //            IEnumerator speedBoostNow = speedBoost(objChangeCol.speedBoostDuration, objChangeCol.speedBoostStrength, obj, attacker);
    //            StartCoroutine(speedBoostNow);
    //        }
    //    }
    //}



    private void tutoSpeechLaunch()
    {
        tutoSpeech(speechDuration, "I should try pressing MenuManager.MenuKey.ToString() several times...", tutoText);
    }

    [ClientRpc]
    new public void RpcLaunchGameTx()
    {
        base.RpcLaunchGameTx();
        Invoke("tutoSpeechLaunch", 2);
    }
}
