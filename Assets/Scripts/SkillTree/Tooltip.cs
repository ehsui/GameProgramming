using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance;

    public GameObject panel;
    public Text tooltipTextGreen;
    public Text tooltipTextRed;

    // 현재 툴팁이 표시되고 있는 대상
    private RectTransform currentAnchor;

    void Awake()
    {
        Instance = this;
        Hide();
    }

    // 프레임마다 대상 노드의 상태 감시
    void Update()
    {
        // 스킬트리패널이 켜져 있을 때만 검사
        if (panel.activeSelf)
        {
            // 1. 대상 노드가 파괴되었거나 (null)
            // 2. 대상 노드가 계층 구조상 비활성화 되었다면 (!activeInHierarchy)
            // (activeInHierarchy는 부모가 꺼져서 자식이 같이 꺼진 경우도 감지)
            if (currentAnchor == null || !currentAnchor.gameObject.activeInHierarchy)
            {
                Hide(); // 툴팁도 같이 숨기기
            }
        }
    }

    public void Show(NodeData data, RectTransform anchor, bool canPurchase)
    {
        currentAnchor = anchor; // 툴팁이 따라다닐 대상
        panel.SetActive(true);
        tooltipTextGreen.gameObject.SetActive(false);
        tooltipTextRed.gameObject.SetActive(false);

        // NodeData에서 자동으로 생성된 설명 가져오기
        string info = data.AutoDescription;

        if (canPurchase)
        {
            tooltipTextGreen.text = info;
            tooltipTextGreen.gameObject.SetActive(true);
        }
        else
        {
            tooltipTextRed.text = info;
            tooltipTextRed.gameObject.SetActive(true);
        }

        // 툴팁 위치 노드 옆으로
        RectTransform panelRect = panel.transform as RectTransform;

        // 노드 아래쪽에 툴팁 띄우기
        Vector3 worldPos = anchor.transform.position;
        panelRect.position = worldPos + new Vector3(0f, -100f, 0f);
    }

    public void Hide()
    {
        currentAnchor = null;   // 대상 초기화해서 툴팁 숨기기
        panel.SetActive(false);
    }
}
