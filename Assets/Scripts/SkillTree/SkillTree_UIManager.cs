using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTree_UIManager : MonoBehaviour
{
    public static SkillTree_UIManager Instance;

    [Header("UI 패널")]
    public GameObject skillTreePanel; // 스킬트리 전체 패널

    private bool isActive = false;

    void Awake()
    {
        // 씬이 넘어가도 파괴되지 않게 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 UI 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 처음엔 꺼두기
        skillTreePanel.SetActive(false);
    }

    void Update()
    {
        // 탭 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleUI();
        }
    }

    public void ToggleUI()
    {
        isActive = !isActive;
        skillTreePanel.SetActive(isActive);

        if (isActive)
        {
            // UI 켜짐 -> 게임 일시정지
            Time.timeScale = 0f;
        }
        else
        {
            // UI 꺼짐 -> 게임 재개
            Time.timeScale = 1f;
        }
    }
}
