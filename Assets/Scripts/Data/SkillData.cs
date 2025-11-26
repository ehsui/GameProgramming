using UnityEngine;

// 스킬의 종류를 구분하는 명찰
public enum SkillType
{
    Projectile,     // 투사체 발사 (예: 1-1, 2-1)
    MultiProjectile,// 여러 개 발사 (예: 3-1)
    Buff,           // 버프 (무적, 은신 등) (예: 1-2, 1-3)
    AreaAttack,     // 범위 공격 (예: 2-2, 3-2)
    Dash,           // 돌진 (예: 2-3)
    Combo           // 연타 (예: 3-3)
}

[CreateAssetMenu(fileName = "New Skill Data", menuName = "Data/Skill Data")]
public class SkillData : ScriptableObject
{
    [Header("스킬 구분")]
    public SkillType skillType;     // (추가) 이 스킬이 어떤 로직을 쓸지 결정

    [Header("기본 정보")]
    public string skillName;
    [TextArea] public string description;

    [Header("비용 및 쿨타임")]
    public float bodhicittaCost = 25f; // 기본 코스트
    public float cooldown = 1.0f;

    [Header("전투 수치")]
    public float damageMultiplier = 1.0f; // 레벨 비례 데미지 배율
    public float duration = 0f;           // 지속 시간 (버프, 돌진 등)
    public int count = 1;                 // 발사체 개수 or 연타 횟수
    public float range = 5f;              // 사거리 or 범위
    public float pushForce = 0f;          // 넉백이나 돌진 힘

    [Header("리소스 (나중에 연결)")]
    public GameObject skillPrefab;     // 투사체나 이펙트 프리팹
}