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

    void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show(NodeData data, RectTransform anchor, bool canPurchase)
    {
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
        panel.SetActive(false);
    }
}
