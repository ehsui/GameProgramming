using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponBranch
{
    Weapon1,Weapon2,Weapon3
}

[CreateAssetMenu(fileName = "NodeData", menuName = "SkillTree/NodeData")]
public class NodeData : ScriptableObject
{
    // 노드 정보
    public string description;

    // 노드 분기 및 순서
    public WeaponBranch branch; // 어떤 무기 분기인지
    public int orderInBranch;   // 분기 안에서 몇 번째 노드인지

    // 스탯 증가량
    public float attackPercent;

    // 선행 노드
    public NodeData preNode;

    // 적용할 무기 데이터
    public WeaponData targetWeapon;
}
