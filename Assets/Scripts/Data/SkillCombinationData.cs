using UnityEngine;

[CreateAssetMenu(fileName = "New Skill Combo", menuName = "Data/Skill Combination")]
public class SkillCombinationData : ScriptableObject
{
    [Header("조합 조건")]
    public int mainWeaponId; 
    public int subWeaponId;  

    [Header("발동 결과")]
    public SkillData skillData; 
}