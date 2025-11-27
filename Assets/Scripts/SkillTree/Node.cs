using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public NodeData data;

    // 노드 이미지
    public Image nodeImage;
    public Color normalColor = new Color(0.5f, 0.5f, 0.5f);
    public Color hoverColor = Color.white;

    private bool isPointerInside = false;   // 마우스가 노드 범위 안에 올려져 있는지
    public bool isPurchased = false;

    public RectTransform tooltipAnchor;

    public void Start()
    {
        nodeImage.color = normalColor;
        SkillTreeManager.Instance.RegisterNode(data, this);
    }

    // 노드에 마우스 올렸을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isPointerInside) return;    // 이미 마우스가 노드 범위 안에 있으면 무시
        isPointerInside = true;

        // 선행 노드가 없거나 선행 노드가 구매된 경우 Hover 가능
        bool canHover = (data.preNode == null) || SkillTreeManager.Instance.IsPurchased(data.preNode);

        if (canHover)
        {
            nodeImage.color = hoverColor;    // 노드 밝은 색으로 변경
        }
        else
        {
            nodeImage.color = normalColor;
        }

        Tooltip.Instance.Show(data.description, tooltipAnchor, canHover); // 툴팁 띄우기
    }

    // 노드 밖으로 마우스 이동했을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isPointerInside) return;   // 마우스가 이미 노드를 떠났으면 무시
        isPointerInside = false;

        Tooltip.Instance.Hide();    // 툴팁 숨기기

        if (isPurchased)    // 이미 구매한 노드이면 노드 밝은 색 유지
        {
            nodeImage.color = hoverColor;
        }
        else    // 구매 안한 노드이면 노드 어두운 색으로 변경
        {
            nodeImage.color = normalColor;
        }
    }

    // 노드 클릭했을 때
    public void OnPointerClick(PointerEventData eventData)
    {
        if (SkillTreeManager.Instance.TryPurchase(data))
        {
            isPurchased = true;     // 구매된 노드로 변경
            nodeImage.color = hoverColor;    // 노드 밝은 색으로 변경
            Debug.Log("clicked");
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        Debug.Log("DOWN on node");
    }

}
