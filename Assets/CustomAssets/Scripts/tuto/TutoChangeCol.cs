using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


// à mettre sur tous les moutons dans le tuto
// change la couleur des gens

public class TutoChangeCol : MonoBehaviour
{
    Renderer rd;
    GameObject deathAnim;
    public Color[] colors;
    Color prevColor;
    AudioSource source;
    NavMeshAgent ag;
    Image healthGUI;
    [SerializeField]
    private Sprite[] sprites;
    int spritesIndex;
    public TextMesh speech;
    private TutoChangeCol PLchangeCol;

    public int StartHp = 20;
    float hp = 10;

    float speedBoostStrengthFactor = 2;
    float speedBoostStrength;
    float SpeedBoostDuration = 1;
    int currBoost = 0;

    public int team;
    public string localName;
    public bool sharing;
    public static TutoChangeCol playerChangeCol;
    public Score score;

    void Start()
    {
        sprites = TutoManager.singleton.sprites;
        score = GetComponent<Score>();
        if (CompareTag("Player"))
        {
            TutoPLMove mover = GetComponent<TutoPLMove>();
            speedBoostStrength = mover.baseSpeed * speedBoostStrengthFactor;
            playerChangeCol = this;
        }

        else if (CompareTag("NPS"))
        {
            NavMeshAgent mover = GetComponent<NavMeshAgent>();
            speedBoostStrength = mover.speed * speedBoostStrengthFactor;
        }
        ag = GetComponent<NavMeshAgent>();
        colors = TutoManager.colors;
        rd = GetComponentInChildren<Renderer>();
        deathAnim = GetComponentInChildren<DieWhenFinishedAnim>(true).gameObject;
        prevColor = rd.materials[0].color;
        speech = GetComponentInChildren<TextMesh>();
        speech.text = "";
        PLchangeCol = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<TutoChangeCol>();
        if (CompareTag("NPS"))
        {
            ChangeCol(colors[0]);
        }
        hp = StartHp;
        healthGUI = TutoManager.singleton.healthGUI;
        //TutoManager.singleton.speak("Press <b>"+MenuManager.interact+"</b> when you\'re ready. The game starts when every <i>logged in</i> player\nis ready (<i>even if</i> everyone you planned to play with hasn\'t logged in yet).\nThis tutorial is offline though,so don\'t worry about forgetting people,\n just remember: <b>"+MenuManager.interact+"</b> when you\'re ready.", TutoManager.singleton.textNarr, 50);
        if (CompareTag("Player"))
        {
            spritesIndex = (int)Mathf.Floor((hp / StartHp) * 10) - 1;
            healthGUI.sprite = sprites[spritesIndex];
            //TutoManager.singleton.speak("I should try pressing <b>"+MenuManager.menu+"</b> several times...", speech, 10);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("AttackChangeCol"))
        {
            TutoManager.singleton.speak("D:", speech, 2);
        }
    }

    public void ChangeCol(GameObject attacker)
    {
        //TutoChangeCol atkChangeCol = attacker.GetComponent<TutoChangeCol>();
        if (hp <= 1)
        {
            if (CompareTag("Player") && TutoManager.singleton.currTask != TutoManager.toDo.L_die)
                hp += 20;
            else
            {
                score.SetTimeOfDeath();
                Kill();
                return;
            }
        }

        int damage = 0;

        if (attacker.CompareTag("Player") && attacker != gameObject && team == playerChangeCol.team)
        {
            if (sharing)
            {
                print("Oh they're sharing so cute");
                print("But: sharing To be implemented");
                // take the one with lower life and give them some of the other's
                if (TutoManager.singleton.currTask == TutoManager.toDo.K_willingTam)
                    TutoManager.singleton.instructions("They were also trying to interact with you at the same time, so the one of you\n with more color changes left gifted some to the other.\n Now turn to paint (press <b>"+MenuManager.selfChange+"</b> or run into mice)", TutoManager.toDo.L_die);

            }
            else if (TutoManager.singleton.currTask == TutoManager.toDo.J_unwillingTeam)
                TutoManager.singleton.instructions("They weren't trying to interact with you, so nothing happened.\n Now find one that stares at what you do,\n and press <b>" + MenuManager.interact + "</b> to try to interact with them", TutoManager.toDo.K_willingTam);
        }
        else
        {

            damage = 2;
            source = GetComponent<AudioSource>();
            source.clip = TutoManager.singleton.ChangeColSounds[Random.Range(0, TutoManager.singleton.ChangeColSounds.Length)];
            source.Play();


            while (prevColor == rd.material.color)
            {
                prevColor = colors[Random.Range(0, colors.Length)];
            }

            rd.materials[0].color = prevColor;
            if (CompareTag("NPS"))
            {
                rd.materials[1].color = prevColor;
                
                if (attacker.CompareTag("Player"))
                {
                    score.colorChangesFromOthers += 1;
                    playerChangeCol.score.colorChangesToOthers += 1;
                    TutoManager.singleton.speak(":D", attacker.GetComponent<TutoChangeCol>().speech, 2);
                    if (TutoManager.singleton.currTask == TutoManager.toDo.E_bully)
                        TutoManager.singleton.instructions("The ball above their head changed (yours is in \nthe top right corner). They have one less color change now.\n Press <b>" + MenuManager.interact + "</b> again. And again. Again.", TutoManager.toDo.F_kill);

                }

                if (team == playerChangeCol.team)
                {
                    rd.materials[1].color = Color.white;
                }
            }

            else if (attacker == gameObject)
            {
                if (CompareTag("Player"))
                {
                    TutoManager.singleton.speak(":/", speech, 2);
                    damage = 1;
                }
                score.colorChangesFromSelf += 1;
            }


            else
            {
                TutoManager.singleton.speak(":(", speech, 2);


                if (attacker.CompareTag("AttackChangeCol"))
                {
                    score.colorChangesFromMice += 1;
                    if (CompareTag("Player") && TutoManager.singleton.currTask == TutoManager.toDo.G_mice)
                    TutoManager.singleton.instructions("If you stay too long on the same ground colour \n you'll change automatically, be careful.\n Test it now.", TutoManager.toDo.H_ground);
                }
                if (attacker.CompareTag("Untagged")) //si c'est le sol qui l'a attaqué
                {
                    score.colorChangesFromGround += 1;
                    if(CompareTag("Player") && TutoManager.singleton.currTask == TutoManager.toDo.H_ground)
                    TutoManager.singleton.instructions("Press " + MenuManager.selfChange + " to change your own color. \nIt only costs you half a colour change.\n Any color change makes you go faster for a bit.", TutoManager.toDo.I_selfChange);
                }
            }
            if (CompareTag("Player"))
            {
                spritesIndex = (int)Mathf.Floor((hp / StartHp) * 10);
                healthGUI.sprite = sprites[spritesIndex];
            }
            StartCoroutine(speedBoost(SpeedBoostDuration, speedBoostStrength, gameObject, attacker));
        }
        hp -= damage;
    }





    IEnumerator speedBoost(float duration, float strength, GameObject obj, GameObject attacker)
    {
        TutoChangeCol objChangeCol = obj.GetComponent<TutoChangeCol>();
        if (obj == attacker) // so it's twice as expensive to speedBoost to chase someone (if you changed your own colour) as it is if you're running away (if you've been attacked)
        {
            duration *= .5f;

        }
        objChangeCol.currBoost += 1;
        int prevBoost = objChangeCol.currBoost;
        if (obj.CompareTag("Player"))
        {
            TutoPLMove playerMove = obj.GetComponent<TutoPLMove>();
            Animator animator = playerMove.animator;
            playerMove.speed = strength;
            animator.speed = 2;

            yield return new WaitForSeconds(duration);
            if (playerMove.speed == strength && prevBoost == objChangeCol.currBoost) // pour qu'il ne ralentisse pas dû à un speedbosst qui n'est plus d'actualité
            {
                playerMove.speed = playerMove.BaseSpeed;
                animator.speed = 1;
            }

        }
        else if (obj.CompareTag("NPS"))
        {
            NavMeshAgent playerMove = obj.GetComponent<NavMeshAgent>();
            Animator animator = playerMove.gameObject.GetComponent<Animator>();

            playerMove.speed = strength;
            animator.speed = 2;

            yield return new WaitForSeconds(duration);
            if (playerMove.speed == strength && prevBoost == objChangeCol.currBoost) // pour qu'il ne ralentisse pas dû à un speedbosst qui n'est plus d'actualité
            {
                playerMove.speed = 3.5f;
                animator.speed = 1;
            }
        }
    }




    public void ChangeCol(Color betterColor)
    {
        print("changing color of " + gameObject.name);
        rd.materials[0].color = betterColor;
        if (this == playerChangeCol)
            betterColor = Color.black;
        else if (team == PLchangeCol.team)
            betterColor = Color.white;
        rd.materials[1].color = betterColor;
    }


    void Kill()
    {
        StopAllCoroutines();
        speech.text = ":(";
        if (TutoManager.singleton.currTask == TutoManager.toDo.F_kill)
            TutoManager.singleton.instructions("It has no more colour changes: it turned to paint.\n And you should avoid the mice. Touch one.", TutoManager.toDo.G_mice);
        if (CompareTag("Player"))
        {
            GetComponent<TutoPLMove>().speed = 0;
            TutoManager.singleton.currState = TutoManager.gameState.deadPlayer;
            TutoManager.singleton.instructions("Press <b>" + MenuManager.interact + "</b> to see the others.", TutoManager.toDo.M_stalker);
        }
        else
        {
            ag.speed = 0;
        }
        rd.gameObject.SetActive(false);
        deathAnim.SetActive(true);
        deathAnim.GetComponent<SpriteRenderer>().color = rd.GetComponent<Renderer>().material.color;
        GetComponent<BoxCollider>().enabled = false; //careful il y a deux box colliders, l'un trigger; ne pas changer leur place

    }
}