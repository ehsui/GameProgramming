using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeManager : MonoBehaviour
{
    public static SkillTreeManager Instance;    // 싱글톤 인스턴스
    public int currentUp = 100;

    private HashSet<NodeData> purchasedNodes = new HashSet<NodeData>(); // 구매한 노드 리스트

    public GameObject linePrefab;   // 노드 연결할 라인 프리팹 변수
    public Transform lineParent;    // 라인들 담을 오브젝트
    public RectTransform centerAnchor;  // 센터 노드 위치

    private Dictionary<NodeData, Node> nodeLookup = new Dictionary<NodeData, Node>();

    public void Awake()
    {
        // 싱글톤
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 구매 가능한 노드인지 확인(마우스 클릭했을 때)
    public bool TryPurchase(NodeData data)
    {
        // 구매 불가능한 경우
        // 1. 이미 구매한 노드일 때
        if (purchasedNodes.Contains(data)) return false;
        // 2. 선행 노드가 구매 안됐음
        if (data.preNode != null && !purchasedNodes.Contains(data.preNode)) return false;
        // 3. 업이 부족함
        if (currentUp < data.cost) return false;

        // 업 소모
        currentUp -= data.cost;

        // 노드 구매 처리
        purchasedNodes.Add(data);

        // 연결해주기
        CreateConnection(data);

        return true;
    }

    // 구매한 노드인지 확인
    public bool IsPurchased(NodeData data)
    {
        return purchasedNodes.Contains(data);
    }

    // 노드 간 연결하는 가지 생성
    public void CreateConnection(NodeData child)
    {
        Node childNode = FindNodeInstance(child);
        if (childNode == null) return;

        RectTransform childRect = childNode.GetComponent<RectTransform>();
        RectTransform parentRect = null;

        // 중앙 노드와 연결하는 경우 (preNode == null)
        if (child.preNode == null)
        {
            parentRect = centerAnchor;   // 중앙 노드 위치 사용
        }
        else
        {
            Node parentNode = FindNodeInstance(child.preNode);
            if (parentNode == null) return;

            parentRect = parentNode.GetComponent<RectTransform>();
        }

        GameObject lineObj = Instantiate(linePrefab, lineParent);
        RectTransform lineRect = lineObj.GetComponent<RectTransform>();

        Vector3 startPos = parentRect.position;
        Vector3 endPos = childRect.position;

        Vector3 dir = endPos - startPos;
        float distance = dir.magnitude;

        lineRect.position = startPos;
        lineRect.sizeDelta = new Vector2(distance, 4f);
        lineRect.rotation = Quaternion.FromToRotation(Vector3.right, dir);

        lineObj.GetComponent<Image>().color = Color.green;
    }


    public void RegisterNode(NodeData data, Node node)
    {
        if (!nodeLookup.ContainsKey(data))
            nodeLookup.Add(data, node);
    }

    public Node FindNodeInstance(NodeData data)
    {
        if (nodeLookup.ContainsKey(data))
            return nodeLookup[data];
        return null;
    }
}
