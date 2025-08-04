using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] float textSpeed;
    [SerializeField] int introDialogueNum;
    [SerializeField] int outroDialogueNum;
    [SerializeField] Text SpeakerText;
    [SerializeField] Text DialogueText;
    // 대화할 때 알파값 조절
    [SerializeField] Image IntroPlayerImage;
    [SerializeField] Image IntroBossImage;
    [SerializeField] Image OutroPlayerImage;
    [SerializeField] Image OutroBossImage;
    [SerializeField] Image NextImage;

    [SerializeField] Dialogue[] dialogues;

    Animator nextAnimator;
    Vector2 nextPos;

    GameObject introParent;
    GameObject outroParent;

    int spaceBarNum;
    int charLength;
    int charIndex;
    int diaIndexLength;

    string currentSpeaker;
    string currentDialogue;

    float textElapsedTime;

    bool isAllOut;
    bool isConversationEnd;
    bool isIntro;

    StringBuilder dialogueBuilder;

    public bool ConverationEnd
    {
        get
        {
            return isConversationEnd;
        }
        set
        {
            isConversationEnd = value;
        }
    }

    void Awake()
    {
        nextAnimator = NextImage.GetComponent<Animator>();
        nextAnimator.enabled = false;
        nextPos = NextImage.transform.position;
    }

    void Start()
    {
        currentSpeaker = dialogues[0].character;
        isAllOut = false;
        DialogueText.text = string.Empty;
        SpeakerText.text = string.Empty;
        isConversationEnd = false;
        isIntro = true;
        diaIndexLength = isIntro ? introDialogueNum : outroDialogueNum;
        SaveDialogue(0);
        ChangeSpeaker(0);
        ChangeImage(0, isIntro);
        introParent = transform.GetChild(1).gameObject;
        outroParent = transform.GetChild(2).gameObject;
        outroParent.SetActive(false);

        dialogueBuilder = new StringBuilder();
    }

    void Update()
    {
        //if (spaceBarNum >= dialogues.Length)
        if (spaceBarNum >= diaIndexLength)
        {
            isConversationEnd = true;
            isIntro = !isIntro;
            if(isIntro == false)
            {
                outroParent.SetActive(true);
                introParent.SetActive(false);
            }
            ChangeImage(spaceBarNum, isIntro);
            diaIndexLength = isIntro ? introDialogueNum : introDialogueNum + outroDialogueNum;
            gameObject.SetActive(false);
        }

        if (isAllOut == false)
        {
            if(nextAnimator.enabled == true)
            {
                NextImage.transform.position = nextPos;
                nextAnimator.enabled = false;
            }

            if(Input.GetKeyDown(KeyCode.Space))
            {
                PrintAllDialogue(charIndex);
            }

            else
            {
                textElapsedTime += Time.deltaTime;
                if (textSpeed < textElapsedTime)
                {
                    PrintDialogue(charIndex, spaceBarNum);
                    textElapsedTime = 0;
                    charIndex++;
                }
            }
        }
        else
        {
            if(nextAnimator.enabled == false)
            {
                nextAnimator.enabled = true;
            }

            if(Input.GetKeyDown(KeyCode.Space))
            {
                DialogueText.text = string.Empty;
                dialogueBuilder.Clear();
                charIndex = 0;
                textElapsedTime = 0;
                spaceBarNum++;
                SaveDialogue(spaceBarNum);
                ChangeSpeaker(spaceBarNum);
                ChangeImage(spaceBarNum, isIntro);
                isAllOut = false;
            }
        }
    }

    void SaveDialogue(int dialogueIndex)
    {
        //if(dialogueIndex < dialogues.Length)
        //{
        //    int length = dialogues[dialogueIndex].dialogue.Length;
        //    charLength = length;
        //    dialogueChar = new char[length];

        //    int num = 0;

        //    foreach (char c in dialogues[dialogueIndex].dialogue)
        //    {
        //        dialogueChar[num] = c;
        //        num++;
        //    }
        //}

        currentDialogue = dialogues[dialogueIndex].dialogue;
        charLength = currentDialogue.Length;

        SpeakerText.text = dialogues[dialogueIndex].character;
    }

    void PrintDialogue(int startIndex, int dialogueIndex)
    {
        if (startIndex < charLength)
        {
            if (startIndex == 27)
            {
                dialogueBuilder.AppendLine();
            }

            dialogueBuilder.Append(currentDialogue[startIndex]);
            DialogueText.text = dialogueBuilder.ToString();

            isAllOut = false;
        }
        else
        {
            isAllOut = true;
        }
    }

    void PrintAllDialogue(int startIndex)
    {
        for(int i= startIndex; i< currentDialogue.Length; i++)
        {
            if (i == 27 || i == 54)
            {
                dialogueBuilder.AppendLine();
            }
            dialogueBuilder.Append(currentDialogue[i]);
        }
        DialogueText.text = dialogueBuilder.ToString();
        isAllOut = true;
    }

    void ChangeSpeaker(int dialogueIndex)
    {
        if (dialogueIndex < dialogues.Length)
        {
            SpeakerText.text = dialogues[dialogueIndex].character;
        }
    }

    void ChangeImage(int dialogueIndex, bool isConversIntro)
    {
        if (dialogueIndex < dialogues.Length)
        {
            currentSpeaker = dialogues[dialogueIndex].character;
        }

        if (currentSpeaker == "데스")
        {
            if(isConversIntro)
            {
                IntroPlayerImage.color = new Color(1, 1, 1, 1);
                IntroBossImage.color = new Color(1, 1, 1, 0.9f);
            }
            else
            {
                OutroPlayerImage.color = new Color(1, 1, 1, 1);
                OutroBossImage.color = new Color(1, 1, 1, 0.9f);
            }
        }
        else
        {
            if(isConversIntro)
            {
                IntroPlayerImage.color = new Color(1, 1, 1, 0.9f);
                IntroBossImage.color = new Color(1, 1, 1, 1);
            }
            else
            {
                OutroPlayerImage.color = new Color(1, 1, 1, 0.9f);
                OutroBossImage.color = new Color(1, 1, 1, 1);
            }
        }
    }
}
