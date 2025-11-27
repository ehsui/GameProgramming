using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeManager : MonoBehaviour
{
    public static SkillTreeManager Instance;    // 싱글톤 인스턴스

    public SkillTreeSaveData saveData;
    public PlayerLevelSaveData levelData;

    public GameObject linePrefab;   // 노드 연결할 라인 프리팹 변수
    public Transform lineParent;    // 라인들 담을 오브젝트

    private Dictionary<NodeData, Node> nodeLookup = new Dictionary<NodeData, Node>();

    public void Awake()
    {
        // 싱글톤
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 구매 가능한 노드인지 레벨 체크
    bool IsLevelAllowed(NodeData data)
    {
        // 1레벨마다 노드 2개씩 구매 가능
        int maxOrder = levelData.level * 2;
        return data.orderInBranch <= maxOrder;
    }

    // 구매 가능한 노드인지 확인(마우스 클릭했을 때)
    public bool TryPurchase(NodeData data)
    {
        // 구매 불가능한 경우

        // 0. 레벨 제한
        if (!IsLevelAllowed(data)) return false;
        // 1. 이미 구매한 노드일 때
        if (saveData.purchasedNodes.Contains(data)) return false;
        // 2. 선행 노드가 구매 안됐음
        if (data.preNode != null && !saveData.purchasedNodes.Contains(data.preNode)) return false;

        // 노드 구매 처리
        saveData.purchasedNodes.Add(data);
        // 연결해주기
        CreateConnection(data);
        // 노드 클릭하면 스탯 증가
        ApplyNodeEffect(data);

        return true;
    }

    // 구매한 노드인지 확인
    public bool IsPurchased(NodeData data)
    {
        return saveData.purchasedNodes.Contains(data);
    }

    // 노드 간 연결하는 가지 생성
    public void CreateConnection(NodeData child)
    {
        if (child.preNode == null) return;

        Node childNode = FindNodeInstance(child);
        Node parentNode = FindNodeInstance(child.preNode);

        if (childNode == null || parentNode == null) return;

        RectTransform childRect = childNode.GetComponent<RectTransform>();
        RectTransform parentRect = parentNode.GetComponent<RectTransform>();

        // 가지 오브젝트 생성
        GameObject lineObj = Instantiate(linePrefab, lineParent);
        RectTransform lineRect = lineObj.GetComponent<RectTransform>();

        // 시작점 - 끝점
        Vector3 startPos = parentRect.position;
        Vector3 endPos = childRect.position;
        Vector3 dir = endPos - startPos;
        float distance = dir.magnitude;

        lineRect.position = startPos;
        lineRect.sizeDelta = new Vector2(distance, lineRect.sizeDelta.y);
        lineRect.rotation = Quaternion.FromToRotation(Vector3.right, dir);
    }

    // 노드 연결하는 가지 그리기
    // 노드 데이터 등록
    public void RegisterNode(NodeData data, Node node)
    {
        if (!nodeLookup.ContainsKey(data))
            nodeLookup.Add(data, node);
    }

    // NodeData에 해당하는 노드 오브젝트 반환
    public Node FindNodeInstance(NodeData data)
    {
        if (nodeLookup.ContainsKey(data))
            return nodeLookup[data];
        return null;
    }

    // 노드 구매하면 스탯 증가
    void ApplyNodeEffect(NodeData data)
    {
        if (data.targetWeapon != null && data.attackPercent != 0f)
        {
            // Weapon 데이터의 damage 증가
            data.targetWeapon.damage *= (1f + (data.attackPercent/100f));
        }
    }
}
