using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeTabController : MonoBehaviour
{
    [Header("스킬 트리 콘텐츠 그룹 (View) 연결")]
    public GameObject weapon1View;
    public GameObject weapon2View;
    public GameObject weapon3View;

    [Header("탭 버튼 이미지 컴포넌트 연결")]
    public Image tab1ButtonImage;
    public Image tab2ButtonImage;
    public Image tab3ButtonImage;

    [Header("버튼 색상 설정")]
    public Color activeColor = Color.white;                         // 선택됐을 때 색 (밝게)
    public Color inactiveColor = new Color(0.7f, 0.7f, 0.7f, 1f); // 선택 안됐을 때 색 (어둡게)

    // 라인 생성을 위해 잠시 모든 뷰를 활성화하는 함수
    public void ActivateAllViewsTemporarily()
    {
        weapon1View.SetActive(true);
        weapon2View.SetActive(true);
        weapon3View.SetActive(true);
    }

    // 초기화 함수 (SkillTreeManager가 호출)
    public void InitializeTabs()
    {
        // 처음에는 1번 탭만 보여주며 시작
        ShowTab1();
    }

    // 1번 탭 버튼 클릭 시 호출
    public void ShowTab1()
    {
        // 콘텐츠 활성/비활성화
        weapon1View.SetActive(true);
        weapon2View.SetActive(false);
        weapon3View.SetActive(false);

        // 버튼 색상 업데이트 (1번 활성화)
        UpdateButtonColors(1);
    }

    // 2번 탭 버튼 클릭 시 호출
    public void ShowTab2()
    {
        weapon1View.SetActive(false);
        weapon2View.SetActive(true);
        weapon3View.SetActive(false);

        // 버튼 색상 업데이트 (2번 활성화)
        UpdateButtonColors(2);
    }

    // 3번 탭 버튼 클릭 시 호출
    public void ShowTab3()
    {
        weapon1View.SetActive(false);
        weapon2View.SetActive(false);
        weapon3View.SetActive(true);

        // 버튼 색상 업데이트 (3번 활성화)
        UpdateButtonColors(3);
    }

    // 버튼 색상 변경하는 함수
    private void UpdateButtonColors(int activeTabNum)
    {
        tab1ButtonImage.color = (activeTabNum == 1) ? activeColor : inactiveColor;
        tab2ButtonImage.color = (activeTabNum == 2) ? activeColor : inactiveColor;
        tab3ButtonImage.color = (activeTabNum == 3) ? activeColor : inactiveColor;
    }
}