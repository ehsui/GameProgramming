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
    // 노드 정보
    public string description;

    // 노드 분기 및 순서
    public BranchType branchType;
    public int orderInBranch;   // 분기 안에서 몇 번째 노드인지
    public NodeData preNode;    // 선행 노드
    
    public float attackPercent; // 증가량

    public WeaponData targetWeapon; // 적용할 무기 데이터
    public SkillData targetSkill;   // 적용할 스킬 데이터
}
