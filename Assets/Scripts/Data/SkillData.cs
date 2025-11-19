using UnityEngine;

[CreateAssetMenu(fileName = "New Skill Data", menuName = "Data/Skill Data")]
public class SkillData : ScriptableObject
{
    [Header("스킬 정보")]
    public string skillName;        // 스킬 이름 (예: 금강령+석장 무적)
    [TextArea] public string description; // 스킬 설명

    [Header("비용 및 쿨타임")]
    public float bodhicittaCost;    // 마나 소모량 (기존 25)
    public float cooldown;          // 쿨타임 (기존 1.0)

    [Header("전투 스탯")]
    // 레벨 * damageMultiplier로 최종 데미지 계산
    public float damageMultiplier;  // 데미지 계수 (예: 2.5, 18...)
    public float duration;          // 지속 시간 (무적, 은신 등)
    public int hitCount;            // 연타 횟수

    [Header("리소스")]
    public GameObject skillPrefab;  // 스킬용 투사체나 이펙트 프리팹
    public GameObject hitEffectPrefab; // 타격 이펙트
}