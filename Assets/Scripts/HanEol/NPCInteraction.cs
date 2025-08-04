using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class NPCInteraction : MonoBehaviour
{
    [SerializeField] bool hadInteracted = false; //대화 한번만 하도록 설정하기 위한 값
    [SerializeField] GameObject conversationCanvas; //대화할 때 켜줄 Canvas 게임 오브젝트
    [SerializeField] GameObject showInteratable; //상호작용 가능시 표시되는 이미지
    [SerializeField] GameObject interactionableButton; //다가가면 표시해주는 키
    [SerializeField] TextMeshPro buttonText;
    [SerializeField] string key = "F";
    [SerializeField]private Text npcName;
    [SerializeField]private Text conversation;
    private string[] conversationLines;
    private NPCInteraction script;
    byte conversationTimes;
    private Vector3 moveShowInteratable = Vector3.zero;
    [SerializeField]private float moveUpAndDown;
   


    //프로퍼티로 설정 가능
    
    public bool Interacted
    {
        get { return hadInteracted; }
    }

    private void Awake()
    {
     
        conversationCanvas = GameObject.Find("InteractCanvus");//이름으로 찾아주기
        showInteratable = GameObject.Find("showInteratable");
        interactionableButton = GameObject.Find("InteractButtonImage");
        buttonText = interactionableButton.GetComponentInChildren<TextMeshPro>();
        npcName = GameObject.Find("Name").GetComponent<Text>();
        conversation = GameObject.Find("Contents").GetComponent<Text>();
        script = GetComponent<NPCInteraction>();
        conversationCanvas.SetActive(false);
        interactionableButton.SetActive(false);
        showInteratable.SetActive(true);
        conversationTimes = 0;
        
        #region 대화 내용
        conversationLines = new string[6];
        conversationLines[0] = ("…그렇게 마음대로 하시는 법이 어딨습니까!\n여기에도 염연히 규칙이란 게 있습니다.\n그러니 먼저...");
        conversationLines[1] = ("아니, 이게 누구야! 회장님이시잖아!\n정말 딱 맞춰 오셨습니다, 회장님.");
        conversationLines[2] = ("온 지옥이 난리가 났어요!\n브래드가 적절한 검토 없이 아무 영혼이나\n거둬들이기로 결정했거든요.");
        conversationLines[3] = ("저희 전 부서가 과도한 업무에\n녹초가 되어 있어요, 저처럼요. 윽.");
        conversationLines[4] = ("동료들과 얘기를 해보세요.\n소로우들 말이에요.\n이 모든 난리에 책임이 있으니까요.");
        conversationLines[5] = ("돌아오셔서 기쁩니다, 회장님.\n그리고 곱게 죽으세요. 헤헤.");
        #endregion
    }

    //private void Update()//테스트용 코드
    //{
    //    if (UnityEngine.Input.GetKeyDown(KeyCode.F))
    //    {
    //        if(hadInteracted == false)
    //        {
    //            Interact();//코루틴 적용시 이거 하나 불러주면 됨 ㅇㅇ
    //        }
    //        if(hadInteracted == true)
    //        {
    //            ContinueConversation();
    //        }
    //    }
    //}

    public void SetButtonText(string text)
    {
        text = key;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") == true)
        {
            if(hadInteracted == false)
            {
                //버튼을 띄우고 대화를 합시다
                if(buttonText.text != key)
                {
                    buttonText.text = key;
                }
                interactionableButton.SetActive(true);
                if(showInteratable.gameObject == true)
                {
                    moveShowInteratable.y = moveUpAndDown;
                    showInteratable.transform.Translate(moveShowInteratable);
                }
     

            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(UnityEngine.Input.GetKeyDown(KeyCode.F))
            {
                if (hadInteracted == false)
                   {
                       Interact();
                   }
            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(hadInteracted == true)
            {
                //대화한 적 있으면 다시 대화 못하게 ㅇㅇ
                interactionableButton.SetActive(false);
                showInteratable.SetActive(false);
            }
            //대화한 적 없으면
            if (interactionableButton == true)
            {
                interactionableButton.SetActive(false);
            }
            if(showInteratable == true)
            {
                if (showInteratable.transform.position.y > 1)
                {
                    moveShowInteratable.y -= moveUpAndDown * 2;
                }
                showInteratable.transform.Translate(moveShowInteratable);
            }
           


        }
    }

    public void Interact()
    {
        hadInteracted=true;
        if (showInteratable == true)
        {
            showInteratable.SetActive(false);
            interactionableButton.SetActive(false);
        }
        conversationCanvas.SetActive(true);

        StartCoroutine(Conversation());
    }

    public void ContinueConversation()
    {
        if (conversationTimes < conversationLines.Length)
        {
            conversation.text = conversationLines[conversationTimes];
            conversationTimes++;
            if(Time.timeScale != 0f) Time.timeScale = 0f;

        }
        else
        {
            EndInteraction();
            script.enabled = false;
        }

        
    }

    private void EndInteraction()
    {
        conversationCanvas.SetActive(!hadInteracted);
        Time.timeScale = 1.0f;
        //StopAllCoroutines();//문제 생기면 currentCoroutine 변수 생성 및 초기화 ㄱㄱ
    }

    IEnumerator Conversation()
    {
        while(conversationCanvas == true)
        {
            if(UnityEngine.Input.GetKeyDown(KeyCode.F) == true)
            {
                if (conversationTimes < conversationLines.Length)
                {
                    conversation.text = conversationLines[conversationTimes];
                    conversationTimes++;
                    if (Time.timeScale != 0f) Time.timeScale = 0f;

                }
                else
                {
                    EndInteraction();
                    script.enabled = false;
                }
            }
           

            yield return null;
        }
        
    }

}
