using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 사용
using TMPro; // TextMeshPro 사용 (필수)
using UnityEngine.SceneManagement; // 씬 이동용

public class CutsceneManager : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI nameText; // 이름 텍스트
    public TextMeshProUGUI dialogueText; // 대사 텍스트
    public Image portraitImage; // 캐릭터 일러스트 이미지

    [Header("Data")]
    public List<Dialogue> dialogues; // 대사 리스트 (인스펙터에서 채움)
    public string nextSceneName; // 대화 끝나고 이동할 씬 이름

    [Header("Settings")]
    public float typingSpeed = 0.05f; // 글자 나오는 속도

    private int currentIndex = -1;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        NextDialogue(); // 시작하자마자 첫 대사 출력
    }

    void Update()
    {
        // 마우스 클릭 또는 스페이스바 입력
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            // 타이핑 중이라면 -> 즉시 완성
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = dialogues[currentIndex].sentence;
                isTyping = false;
            }
            // 타이핑이 끝난 상태라면 -> 다음 대사로
            else
            {
                NextDialogue();
            }
        }
    }

    public void NextDialogue()
    {
        currentIndex++;

        // 대사가 더 남아있는지 확인
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

        // 2. 이미지 설정 (이미지가 있을 때만 바꿈)
        if (data.portrait != null)
        {
            portraitImage.sprite = data.portrait;
            portraitImage.gameObject.SetActive(true); // 이미지가 있다면 켜기
        }
        else
        {
            // 이미지가 없으면 숨기기 (선택사항)
            // portraitImage.gameObject.SetActive(false); 
        }

        // 3. 타이핑 효과 시작
        typingCoroutine = StartCoroutine(TypeSentence(data.sentence));
    }

    // 한 글자씩 나오는 코루틴
    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = ""; // 텍스트 초기화

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void EndCutscene()
    {
        Debug.Log("컷씬 종료! 다음 씬으로 이동합니다.");
        
        // 씬 이동이 필요하다면 주석 해제
        // SceneManager.LoadScene(nextSceneName);
        
        // 혹은 그냥 이 UI를 끄고 게임을 시작하려면
        // gameObject.SetActive(false);
    }
}