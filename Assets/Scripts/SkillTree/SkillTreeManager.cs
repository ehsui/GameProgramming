using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeManager : MonoBehaviour
{
    public static SkillTreeManager Instance;    // 싱글톤 인스턴스

    [Header("Data")]
    public SkillTreeSaveData saveData;
    public PlayerLevelSaveData levelData;

    [Header("Line")]
    public GameObject linePrefab;   // 노드 연결할 라인 프리팹 변수
    public Color lockedLineColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    public Color unlockedLineColor = Color.white;

    [Header("무기별 LineParent")]
    public Transform lineParentW1;
    public Transform lineParentW2;
    public Transform lineParentW3;

    [Header("SkillTree Tab Controller")]
    public SkillTreeTabController tabController;

    private Dictionary<NodeData, Node> nodeLookup = new Dictionary<NodeData, Node>();

    public void Awake()
    {
        // 싱글톤
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // [테스트용] 게임 시작할 때마다 구매 목록 싹 비우기
        // 테스트 끝나면 이 줄은 꼭 지우거나 주석 처리!!!!!!!!!!
        if (saveData != null)
        {
            saveData.purchasedNodes.Clear();
            Debug.Log("테스트를 위해 구매 목록을 초기화했습니다.");
        }
    }

    // 저장된 데이터 불러와서 화면에 다시 그리기
    public void Start()
    {
        // 라인 생성 전에 모든 탭 뷰를 활성화해서 UI 계산이 제대로 되게 함
        if (tabController != null)
        {
            tabController.ActivateAllViewsTemporarily();
        }
        else
        {
            Debug.LogError("Tab Controller가 연결되지 않았습니다.");
        }

        // 라인 생성
        GenerateAllLines();

        // 라인 생성이 끝났으니 탭 상태를 초기화 (1번만 켜고 나머지 끔)
        if (tabController != null)
        {
            tabController.InitializeTabs();
        }

        // 2. 저장된 노드 데이터 불러와서 구매한 노드 색 변경
        RefreshUI();
    }

    // 구매 가능한 노드인지 레벨 체크
    public bool IsLevelAllowed(NodeData data)
    {
        // 1레벨마다 노드 2개씩 구매 가능
        int maxOrder = levelData.level * 2;
        return data.orderInBranch <= maxOrder;
    }

    // 구매 가능한 노드인지 확인(마우스 클릭했을 때)
    public bool TryPurchase(NodeData data)
    {
        // 0. 선행 노드가 구매 안됐음
        if (data.preNode != null && !saveData.purchasedNodes.Contains(data.preNode))
        {
            Debug.Log($"현재 레벨: {levelData.level}\n>> 노드 구매 실패: 선행 노드를 먼저 구매해야 합니다.");
            return false;
        }
        // 1. 레벨 제한
        if (!IsLevelAllowed(data))
        {
            Debug.Log($"현재 레벨: {levelData.level}\n>> 노드 구매 실패: 레벨이 부족합니다.");
            return false;
        }
        // 2. 이미 구매한 노드일 때
        if (saveData.purchasedNodes.Contains(data))
        {
            Debug.Log($"현재 레벨: {levelData.level}\n>> 이미 구매한 노드입니다.");
            return false;
        }
        

        // 노드 구매 처리
        saveData.purchasedNodes.Add(data);
        Debug.Log($">> {data.name} 구매 성공!");

        // 색상 변경
        RefreshUI();

        // 노드 클릭하면 스탯 증가
        ApplyNodeEffect(data);

        return true;
    }

    // 구매한 노드인지 확인
    public bool IsPurchased(NodeData data)
    {
        return saveData.purchasedNodes.Contains(data);
    }

    // 노드 연결하는 라인 생성
    void GenerateAllLines()
    {
        // nodeLookup에 등록된 모든 노드를 하나씩 꺼내기
        foreach (KeyValuePair<NodeData, Node> entry in nodeLookup)
        {
            NodeData data = entry.Key;
            Node currentNode = entry.Value;

            // 선행 노드가 없으면(루트 노드면) 선 생성X
            if (data.preNode == null) continue;

            // 선 생성 함수 호출
            CreateLineObject(currentNode, data);
        }
    }

    // UI 새로고침 (색깔 업데이트)
    public void RefreshUI()
    {
        // 모든 노드를 하나씩 검사
        foreach (KeyValuePair<NodeData, Node> entry in nodeLookup)
        {
            NodeData data = entry.Key;
            Node node = entry.Value;

            // 1. 노드 색상 처리 (구매됐으면 밝게, 아니면 어둡게)
            if (IsPurchased(data))
            {
                node.isPurchased = true;
                node.nodeImage.color = node.hoverColor;
            }
            else
            {
                node.isPurchased = false;
                node.nodeImage.color = node.normalColor;
            }

            // 2. 라인 색상 처리
            // connectedLine : 부모 노드 -> 현재 노드 연결하는 라인
            if (node.connectedLine != null && data.preNode != null)
            {
                if (IsPurchased(data.preNode))
                {
                    // 부모 노드가 구매되면 connectedLine 밝게 변경
                    node.connectedLine.color = unlockedLineColor;
                }
                else
                {
                    node.connectedLine.color = lockedLineColor;
                }
            }
        }
    }

    // 라인 오브젝트 실제로 화면에 생성
    void CreateLineObject(Node childNode, NodeData data)
    {
        Node parentNode = FindNodeInstance(data.preNode);
        if (parentNode == null) return;

        // 부모 LineParent 찾기
        Transform targetLineParent = null;

        Debug.Log($"[라인 생성 시도] 노드명: {data.name}, 감지된 BranchType: {data.branchType}, 부모 노드: {parentNode.name}");

        switch (data.branchType) // NodeData의 BranchType
        {
            case BranchType.Weapon1:
            case BranchType.Weapon11:
            case BranchType.Weapon12:
            case BranchType.Weapon13:
                targetLineParent = lineParentW1;
                break;
            case BranchType.Weapon2:
            case BranchType.Weapon21:
            case BranchType.Weapon22:
            case BranchType.Weapon23:
                targetLineParent = lineParentW2;
                break;
            case BranchType.Weapon3:
            case BranchType.Weapon31:
            case BranchType.Weapon32:
            case BranchType.Weapon33:
                targetLineParent = lineParentW3;
                break;
            default:
                Debug.LogError($"[SkillTreeManager] 알 수 없는 BranchType입니다: {data.branchType}, 노드 이름: {data.name}");
                return; // 부모를 못 찾으면 라인 생성 중단
        }

        // 라인 오브젝트 생성
        GameObject lineObj = Instantiate(linePrefab, targetLineParent);
        RectTransform lineRect = lineObj.GetComponent<RectTransform>();
        Image lineImage = lineObj.GetComponent<Image>();

        // 위치, 회전
        Vector2 startPosLocal = targetLineParent.InverseTransformPoint(parentNode.GetComponent<RectTransform>().position);
        Vector2 endPosLocal = targetLineParent.InverseTransformPoint(childNode.GetComponent<RectTransform>().position);

        lineRect.pivot = new Vector2(0.5f, 0f);
        lineRect.anchorMin = new Vector2(0.5f, 0.5f);
        lineRect.anchorMax = new Vector2(0.5f, 0.5f);
        lineRect.anchoredPosition = startPosLocal;

        Vector2 dir = endPosLocal - startPosLocal;
        float distance = dir.magnitude;

        // 길이와 회전
        lineRect.sizeDelta = new Vector2(lineRect.sizeDelta.x, distance);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        lineRect.localRotation = Quaternion.Euler(0, 0, angle);

        lineRect.SetAsLastSibling(); // 맨 앞에 그리기
        if (lineImage != null) lineImage.raycastTarget = false; // 레이캐스트 끄기

        // 처음엔 어두운 색
        lineImage.color = lockedLineColor;

        // 자식 노드에 생성된 라인 이미지 저장
        childNode.connectedLine = lineImage;
    }

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
        // 1. 공격력 증가 효과가 있는지 확인
        if (data.attackPercent <= 0f)
        {
            Debug.Log($">> {data.name}: 공격력 증가 효과 없음 (패시브 노드가 아님)");
            return;
        }

        // 2. 적용 대상 무기(Target Weapons)가 있는지 확인
        if (data.targetWeapons == null || data.targetWeapons.Length == 0)
        {
            Debug.LogWarning($"[{data.name}] 공격력 증가 수치가 있지만, Target Weapons가 비어있습니다");
            return;
        }

        // 3. 등록된 모든 무기에 대해 공격력 증가 적용 (복합 노드 지원)
        foreach (WeaponData weapon in data.targetWeapons)
        {
            if (weapon == null) continue;

            float oldDamage = weapon.damage;

            // [공격력 증가]
            // 기존 공격력 + (기존 공격력 * (증가 퍼센트 / 100))
            float increaseAmount = oldDamage * (data.attackPercent / 100f);
            float newDamage = oldDamage + increaseAmount;

            // 실제 WeaponData 스크립터블 오브젝트 값 수정
            weapon.damage = newDamage;

            Debug.Log($"$$ [스탯 적용됨!] {weapon.weaponName} 공격력 변경: {oldDamage} -> {newDamage} (+{increaseAmount:F1} / +{data.attackPercent}%)");
        }
    }
}
