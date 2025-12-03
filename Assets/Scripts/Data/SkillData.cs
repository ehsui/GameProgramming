using UnityEngine;


public enum SkillType
{
    Projectile,     
    MultiProjectile,
    Buff,           
    AreaAttack,     
    Dash,           
    Combo,           
    Laser,
    Stealth,
    Heal
}

[CreateAssetMenu(fileName = "New Skill Data", menuName = "Data/Skill Data")]
public class SkillData : ScriptableObject
{
    [Header("스킬 구분")]
    public SkillType skillType;     

    [Header("기본 정보")]
    public string skillName;
    [TextArea] public string description;

    [Header("비용 및 쿨타임")]
    public float bodhicittaCost = 25f; 
    public float cooldown = 1.0f;

    [Header("전투 수치")]
    public float damageMultiplier = 1.0f; 
    public float duration = 0f;           
    public int count = 1;                 
    public float range = 5f;              
    public float pushForce = 0f;          

    [Header("리소스 (나중에 연결)")]
    public GameObject skillPrefab;     
    public GameObject loopEffectPrefab; 
}