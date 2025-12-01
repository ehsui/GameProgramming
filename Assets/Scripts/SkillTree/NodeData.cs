using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BranchType
{
    Weapon1, Weapon2, Weapon3,
    Weapon11, Weapon12, Weapon13,
    Weapon21, Weapon22, Weapon23,
    Weapon31, Weapon32, Weapon33
}

public enum AttackBonusType
{
    None, FlatAdd, Multiplier
}

[CreateAssetMenu(fileName = "NodeData", menuName = "SkillTree/NodeData")]
public class NodeData : ScriptableObject
{
    [Header("노드 정보")]
    public string nodeName;
    [HideInInspector] public string description;    // 노드 설명

    [Header("노드 구조")]
    public BranchType branchType;
    public int orderInBranch;
    public NodeData preNode;

    [Header("공격력 보너스 설정")]
    public AttackBonusType bonusType = AttackBonusType.None;

    // FlatAdd 선택 : 더할 고정값
    // Multiplier 선택 : 곱할 배율
    public float bonusValue = 0f;

    [Header("적용 대상")]   // 이 노드의 효과를 적용할 무기들
    public WeaponData[] targetWeapons;

    // 노드 Description 자동 생성 프로퍼티
    public string AutoDescription
    {
        get
        {
            if (bonusType == AttackBonusType.None || targetWeapons == null || targetWeapons.Length == 0) return "";

            // 1. 대상 무기 이름들 합치기 (예: "금강령 + 석장")
            string weaponNames = "";
            for (int i = 0; i < targetWeapons.Length; i++)
            {
                if (targetWeapons[i] == null) continue;
                weaponNames += targetWeapons[i].weaponName;
                if (i < targetWeapons.Length - 1) weaponNames += " + ";
            }

            // 2. 보너스 수치 문자열 만들기 (예: "공격력 +1" 또는 "공격력 2배")
            string bonusString = "";
            switch (bonusType)
            {
                case AttackBonusType.FlatAdd:
                    // F1: 소수점 한 자리까지
                    bonusString = $"공격력 +{bonusValue:F1}";
                    break;
                case AttackBonusType.Multiplier:
                    bonusString = $"공격력 x{bonusValue:F1}";
                    break;
            }

            // 3. 최종 합체 (예: "금강령 + 석장 공격력 +1")
            return $"{weaponNames} {bonusString}";
        }
    }

    // 유니티 에디터에서 값을 바꿀 때마다 자동으로 description 변수를 업데이트해서
    // 인스펙터에서 바로 확인할 수 있게 해주는 마법의 함수
    private void OnValidate()
    {
        description = AutoDescription;
    }
}
