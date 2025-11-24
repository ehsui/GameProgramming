using UnityEngine;

[CreateAssetMenu(fileName = "New Skill Combo", menuName = "Data/Skill Combination")]
public class SkillCombinationData : ScriptableObject
{
    [Header("조합 조건")]
    public int mainWeaponId; // 주무기 ID (1:금강령, 2:석장, 3:금강저)
    public int subWeaponId;  // 보조무기 ID (1~3)

    [Header("발동 결과")]
    public SkillData skillData; // 실행될 스킬 데이터
}