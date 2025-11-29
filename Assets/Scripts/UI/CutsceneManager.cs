using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using UnityEngine.SceneManagement; 

public class CutsceneManager : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI nameText; 
    public TextMeshProUGUI dialogueText; 
    public Image portraitImage; 
    
    public Image backgroundImage; 

    [Header("Data")]
    public List<Dialogue> dialogues; 

    [Header("Settings")]
    public float typingSpeed = 0.05f; 

    [Header("Move To Scene")]
    public string nextSceneName = "GameScene"; 


    private int currentIndex = -1;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        NextDialogue(); 
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = dialogues[currentIndex].sentence;
                isTyping = false;
            }
            else
            {
                NextDialogue();
            }
        }
    }

    public void NextDialogue()
    {
        currentIndex++;

        if (currentIndex < dialogues.Count)
        {
            ShowDialogue(dialogues[currentIndex]);
        }
        else
        {
            EndCutscene();
        }
    }

    void ShowDialogue(Dialogue data)
    {
        // 1. 이름 설정
        nameText.text = data.speakerName;

        // 2. 캐릭터 이미지 설정
        if (data.portrait != null)
        {
            portraitImage.sprite = data.portrait;
            portraitImage.gameObject.SetActive(true); 
        }
        else
        {
            // portraitImage.gameObject.SetActive(false);
        }

        // 배경 이미지 설정 logic
        if (data.background != null)
        {
            backgroundImage.sprite = data.background;
        }

        // 4. 타이핑 효과 시작
        typingCoroutine = StartCoroutine(TypeSentence(data.sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = ""; 

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void EndCutscene()
    {
        Debug.Log("컷씬 종료! 게임으로 이동합니다.");
        SceneManager.LoadScene(nextSceneName);
    }
}