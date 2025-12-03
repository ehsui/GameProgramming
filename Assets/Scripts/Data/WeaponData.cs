using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Data/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("기본 정보")]
    public string weaponName;       
    public int weaponId;            
    public Sprite icon;             
    public Sprite weaponSprite;     

    [Header("전투 스탯")]
    public float damage;                public float attackRate;            public float attackRange;           public float spawnOffset = 1.0f;    

    [Header("리소스")]
    
    public RuntimeAnimatorController animatorController;

    
    public GameObject effectPrefab;

    
    public GameObject projectilePrefab;
    public GameObject meleeSlashPrefab;

    // [1203 추가] 공격 사운드 
    [Header("공격 사운드")]
    public AudioClip attackSound;
}