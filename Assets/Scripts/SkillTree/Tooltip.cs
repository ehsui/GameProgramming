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

    public void Show(string info, RectTransform anchor, bool canPurchase)
    {
        panel.SetActive(true);
        tooltipTextGreen.gameObject.SetActive(false);
        tooltipTextRed.gameObject.SetActive(false);

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

        // ÅøÆÁ À§Ä¡ ³ëµå ¿·À¸·Î
        RectTransform panelRect = panel.transform as RectTransform;

        // ³ëµå ¿ÞÂÊ¿¡ ÅøÆÁ ¶ç¿ì±â
        Vector3 worldPos = anchor.transform.position;
        panelRect.position = worldPos + new Vector3(0f, -60f, 0f);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}
