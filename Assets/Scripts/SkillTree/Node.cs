using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public NodeData data;
    public Image nodeImage;
    public Image connectedLine; // 현재 노드와 다음 노드 연결하는 line이미지
    public Color normalColor = new Color(0.5f, 0.5f, 0.5f);
    public Color hoverColor = Color.white;

    private bool isPointerInside = false;   // 마우스가 노드 범위 안에 올려져 있는지
    public bool isPurchased = false;
    public RectTransform tooltipAnchor;

    private bool isPressed = false;

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
        bool isParentUnlocked = (data.preNode == null) || SkillTreeManager.Instance.IsPurchased(data.preNode);
        
        // 레벨이 충분하지 않은 경우
        bool isLevelEnough = SkillTreeManager.Instance.IsLevelAllowed(data);

        bool canHover = isParentUnlocked && isLevelEnough;

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

    // 마우스 눌렀을 때 위치 저장
    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    // 마우스를 뗐을 때 노드 구매
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPressed)
        {
            // 노드 구매
            if (SkillTreeManager.Instance.TryPurchase(data))
            {
                isPurchased = true;
                nodeImage.color = hoverColor;
                Debug.Log("구매 성공!");
            }
        }
        isPressed = false;
    }
}
