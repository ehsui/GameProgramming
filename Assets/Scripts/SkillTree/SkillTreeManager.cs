using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    public static SkillTreeManager Instance;

    public int currentUp = 100;

    private HashSet<NodeData> purchasedNodes = new HashSet<NodeData>();

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

        // 스탯 적용 테스트
        ApplyStats(data);
        Debug.Log("노드 구매 완료");

        return true;
    }

    // 구매한 노드인지 확인
    public bool IsPurchased(NodeData data)
    {
        return purchasedNodes.Contains(data);
    }

    private void ApplyStats(NodeData data)
    {
        if (data.attackPowerPercent > 0)
            PlayerStats.Instance.AttackPowerUpgrade(data.attackPowerPercent);
        if (data.defensePowerPercent > 0)
            PlayerStats.Instance.DefensePowerUpgrade(data.defensePowerPercent);
    }
}
