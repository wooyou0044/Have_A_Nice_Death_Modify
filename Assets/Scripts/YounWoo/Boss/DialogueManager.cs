using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject dialogueCanavas;
    [SerializeField] GameObject bossCanavas;
    [SerializeField] GameObject desk;
    [SerializeField] GameObject clearCanvas;
    [SerializeField] SpriteRenderer backGround;

    GameObject canvas;
    GameObject boss;
    GameObject clearWindow;

    BossMovement getBossMove;
    GargoyleBrain getBossAi;
    DialogueUI getDialogueUI;
    DeskMovement getDeskMove;
    Controller playerController;

    BossCamera getCamera;

    void Awake()
    {
        boss = GameObject.FindWithTag("Enemy");
        getBossMove = boss.GetComponent<BossMovement>();
        getBossAi = boss.GetComponent<GargoyleBrain>();

        canvas = Instantiate(dialogueCanavas);
        getDialogueUI = canvas.GetComponent<DialogueUI>();
        canvas.SetActive(false);

        getDeskMove = desk.GetComponent<DeskMovement>();

        getCamera = Camera.main.GetComponent<BossCamera>();

        clearWindow = Instantiate(clearCanvas);
        clearWindow.SetActive(false);
    }

    void Start()
    {

    }

    void Update()
    {
        if(getCamera.isArrive == true)
        {
            getBossMove.PlayerEnterBossStage();
            getCamera.isArrive = false;
        }
        if(getBossMove.IsMeetPlayer == true && getBossMove.IsEndAnimation() == true)
        {
            canvas.SetActive(true);
            GameManager.input = false;
            getBossMove.IsMeetPlayer = false;
        }
        if(getBossMove.isAlive == true && getDialogueUI.ConverationEnd == true)
        {
            GameManager.input = true;
            getBossMove.FightParticipation();
            getDeskMove.TurnOnAnimator();
            StartCoroutine(DoStop());
            IEnumerator DoStop()
            {
                yield return new WaitForSeconds(1.5f);
                getBossAi.enabled = true;
                getBossMove.MoveStop();
                bossCanavas.SetActive(true);
            }
            getDialogueUI.ConverationEnd = false;
        }

        if (getBossMove.IsDead == true && getBossMove.IsEndAnimation() == true)
        {
            StartCoroutine(PlayDialogue());
            IEnumerator PlayDialogue()
            {
                yield return new WaitForSeconds(1.0f);
                GameManager.input = false;
                canvas.SetActive(true);
                bossCanavas.SetActive(false);
            }
            getBossMove.deathState = BossMovement.DeathType.Awake;
            getBossMove.IsDead = false;
        }

        if (getBossMove.deathState == BossMovement.DeathType.Awake && getDialogueUI.ConverationEnd == true)
        {
            GameManager.input = true;
            getBossMove.DeathAnimation(getBossMove.deathState);
            getBossMove.deathState = BossMovement.DeathType.Be_AnotherBoss;
            getDialogueUI.ConverationEnd = false;
        }

        if(getBossMove.deathState == BossMovement.DeathType.Be_AnotherBoss && getBossMove.IsEndAnimation() == true)
        {
            backGround.color = new Color(1,0.5f, 1);
            getBossMove.DeathAnimation(getBossMove.deathState);
            StartCoroutine(DoStop());
            IEnumerator DoStop()
            {
                yield return new WaitForSeconds(1.5f);
                getBossMove.MoveStop();
                getBossMove.SetActiveTornado(false);
            }
            getBossMove.deathState = BossMovement.DeathType.GoOutside;
        }

        if (getBossMove.deathState == BossMovement.DeathType.GoOutside && getBossMove.IsEndAnimation() == true)
        {
            backGround.color = new Color(1, 1, 1);
            getBossMove.DeathAnimation(getBossMove.deathState);
            StartCoroutine(DoStop());
            IEnumerator DoStop()
            {
                yield return new WaitForSeconds(0.8f);
                getBossMove.MoveStop();
            }
            getBossMove.deathState = BossMovement.DeathType.Death;
        }

        if (getBossMove.deathState == BossMovement.DeathType.Death && getBossMove.IsEndAnimation() == true)
        {
            getBossMove.DeathAnimation(getBossMove.deathState);
        }

        if(getBossMove.IsDisappear && clearWindow.activeSelf == false)
        {
            clearWindow.SetActive(true);
        }
    }
}
