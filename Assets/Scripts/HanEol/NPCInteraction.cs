using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class NPCInteraction : MonoBehaviour
{
    [SerializeField] bool hadInteracted = false; //��ȭ �ѹ��� �ϵ��� �����ϱ� ���� ��
    [SerializeField] GameObject conversationCanvas; //��ȭ�� �� ���� Canvas ���� ������Ʈ
    [SerializeField] GameObject showInteratable; //��ȣ�ۿ� ���ɽ� ǥ�õǴ� �̹���
    [SerializeField] GameObject interactionableButton; //�ٰ����� ǥ�����ִ� Ű
    [SerializeField] TextMeshPro buttonText;
    [SerializeField] string key = "F";
    [SerializeField]private Text npcName;
    [SerializeField]private Text conversation;
    private string[] conversationLines;
    private NPCInteraction script;
    byte conversationTimes;
    private Vector3 moveShowInteratable = Vector3.zero;
    [SerializeField]private float moveUpAndDown;
   


    //������Ƽ�� ���� ����
    
    public bool Interacted
    {
        get { return hadInteracted; }
    }

    private void Awake()
    {
     
        conversationCanvas = GameObject.Find("InteractCanvus");//�̸����� ã���ֱ�
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
        
        #region ��ȭ ����
        conversationLines = new string[6];
        conversationLines[0] = ("���׷��� ������� �Ͻô� ���� ������ϱ�!\n���⿡�� ������ ��Ģ�̶� �� �ֽ��ϴ�.\n�׷��� ����...");
        conversationLines[1] = ("�ƴ�, �̰� ������! ȸ����̽��ݾ�!\n���� �� ���� ���̽��ϴ�, ȸ���.");
        conversationLines[2] = ("�� ������ ������ �����!\n�귡�尡 ������ ���� ���� �ƹ� ��ȥ�̳�\n�ŵֵ��̱�� �����߰ŵ��.");
        conversationLines[3] = ("���� �� �μ��� ������ ������\n���ʰ� �Ǿ� �־��, ��ó����. ��.");
        conversationLines[4] = ("������ ��⸦ �غ�����.\n�ҷο�� ���̿���.\n�� ��� ������ å���� �����ϱ��.");
        conversationLines[5] = ("���ƿ��ż� ��޴ϴ�, ȸ���.\n�׸��� ���� ��������. ����.");
        #endregion
    }

    //private void Update()//�׽�Ʈ�� �ڵ�
    //{
    //    if (UnityEngine.Input.GetKeyDown(KeyCode.F))
    //    {
    //        if(hadInteracted == false)
    //        {
    //            Interact();//�ڷ�ƾ ����� �̰� �ϳ� �ҷ��ָ� �� ����
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
                //��ư�� ���� ��ȭ�� �սô�
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
                //��ȭ�� �� ������ �ٽ� ��ȭ ���ϰ� ����
                interactionableButton.SetActive(false);
                showInteratable.SetActive(false);
            }
            //��ȭ�� �� ������
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
        //StopAllCoroutines();//���� ����� currentCoroutine ���� ���� �� �ʱ�ȭ ����
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
