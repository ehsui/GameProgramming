using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NodeData", menuName = "SkillTree/NodeData")]
public class NodeData : ScriptableObject
{
    // 노드 정보
    public string description;

    // 스탯 증가량
    public float attackPowerPercent;
    public float defensePowerPercent;
    public float attackSpeedPercent;
    public float projectileRangePercent;
    public float explosionRangePercent;

    // 업 소모량
    public int cost;

    // 선행 노드
    public NodeData preNode;
}
