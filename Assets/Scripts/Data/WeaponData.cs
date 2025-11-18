using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Data/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("기본 정보")]
    public string weaponName;       // 무기 이름 (예: 금강령, 석장)
    public int weaponId;            // 무기 ID (1, 2, 3...)
    public Sprite icon;             // UI용 아이콘 (선택사항)
    public Sprite weaponSprite;     // 플레이어 손에 들릴 스프라이트

    [Header("전투 스탯")]
    public float damage;            // 공격력
    public float attackRate;        // 공격 속도 (딜레이)
    public float attackRange;       // 사거리

    [Header("리소스")]
    // 무기별 애니메이션 컨트롤러 (장착 시 교체용)
    public RuntimeAnimatorController animatorController;

    // 공격 이펙트 (금강령용)
    public GameObject effectPrefab;

    // 투사체 프리팹 (석장용)
    public GameObject projectilePrefab;
}