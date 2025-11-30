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

    // 무기 강화 스탯
    private class DamageBonus
    {
        public float flatAddTotal = 0f;     // 고정값 합계
        public float multiplierTotal = 1f;  // 배율 곱계
    }

    private Dictionary<NodeData, Node> nodeLookup = new Dictionary<NodeData, Node>();
    private Dictionary<WeaponData, DamageBonus> weaponBonusMap = new Dictionary<WeaponData, DamageBonus>();

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
        // 효과가 없거나 대상 무기가 없으면 패스
        if (data.bonusType == AttackBonusType.None || data.bonusValue == 0f || data.targetWeapons == null) return;

        // 적용 대상 무기 이름들 수집(금강령 + 석장)
        string weaponNamesStr = "";
        for (int i = 0; i < data.targetWeapons.Length; i++)
        {
            if (data.targetWeapons[i] == null) continue;
            weaponNamesStr += data.targetWeapons[i].weaponName;
            if (i < data.targetWeapons.Length - 1) weaponNamesStr += " + ";
        }

        // 각 무기별로 보너스 적용
        foreach (WeaponData weapon in data.targetWeapons)
        {
            if (weapon == null) continue;

            // 1. 이 무기의 보너스 기록이 없으면 새로 만듦
            if (!weaponBonusMap.ContainsKey(weapon))
            {
                weaponBonusMap.Add(weapon, new DamageBonus());
            }

            // 2. 보너스 타입에 따라 값을 누적 기록
            DamageBonus currentBonus = weaponBonusMap[weapon];
            switch (data.bonusType)
            {
                case AttackBonusType.FlatAdd:
                    currentBonus.flatAddTotal += data.bonusValue;
                    Debug.Log($"[{weaponNamesStr}] 공격력 +{data.bonusValue} (현재 무기 '{weapon.weaponName}'의 총합: +{currentBonus.flatAddTotal})");
                    break;
                case AttackBonusType.Multiplier:
                    currentBonus.multiplierTotal *= data.bonusValue;
                    Debug.Log($"[{weaponNamesStr}] 공격력 x{data.bonusValue}(현재 무기 '{weapon.weaponName}'의 총배율: x{currentBonus.multiplierTotal:F2})");
                    break;
            }
        }
    }

    // 외부에서 최종 데미지를 요청할 때 계산하는 함수
    public float GetFinalWeaponDamage(WeaponData weapon)
    {
        float baseDamage = weapon.damage; // 원본 공격력

        // 보너스 기록이 없으면 원본 그대로 반환
        if (!weaponBonusMap.ContainsKey(weapon))
        {
            return baseDamage;
        }

        // 기록이 있으면 계산 시작
        DamageBonus bonus = weaponBonusMap[weapon];

        // 최종 데미지 = (원본 + 고정값 총합) * 배율 총곱
        float finalDamage = (baseDamage + bonus.flatAddTotal) * bonus.multiplierTotal;

        return finalDamage;
    }
}
