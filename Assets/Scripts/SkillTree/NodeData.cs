using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BranchType
{
    Weapon1, Weapon2, Weapon3,
    Weapon11, Weapon12, Weapon13,
    Weapon21, Weapon22, Weapon23,
    Weapon31, Weapon32, Weapon33
}

[CreateAssetMenu(fileName = "NodeData", menuName = "SkillTree/NodeData")]
public class NodeData : ScriptableObject
{
    [Header("노드 정보")]
    [TextArea] public string description; // TextArea로 변경해서 줄바꿈 가능하게

    [Header("노드 분기 및 순서")]
    public BranchType branchType;
    public int orderInBranch;
    public NodeData preNode;

    [Header("스탯 증가 효과")]
    public float attackPercent; // 증가량

    [Header("적용 대상")]   // 이 노드의 효과를 적용할 무기들
    public WeaponData[] targetWeapons;
}
