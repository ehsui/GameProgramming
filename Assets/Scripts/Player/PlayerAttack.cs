using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerController controller;
    // [1203 추가] 오디오 소스 컴포넌트
    private AudioSource audioSource;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        // [1203 추가] 오디오 소스 컴포넌트 가져오기
        audioSource = GetComponent<AudioSource>();
    }

    public void PerformAttack(WeaponData weapon)
    {
        if (weapon == null) return;

        switch (weapon.weaponId)
        {
            case 1: 
                DoInstantAreaAttack(weapon);
                break;
            case 2: 
                DoProjectileAttack(weapon);
                break;
            case 3: 
                DoMeleeAttack(weapon);
                break;
        }
    }

    
    private void DoInstantAreaAttack(WeaponData weapon)
    {
        Vector2 direction = controller.IsFacingRight ? Vector2.right : Vector2.left;

        
        Vector2 spawnPosition = (Vector2)transform.position + (direction * weapon.spawnOffset);

        
        if (weapon.projectilePrefab != null) 
        {
            GameObject effect = Instantiate(weapon.projectilePrefab, spawnPosition, Quaternion.identity);

            
            if (direction.x < 0)
            {
                Vector3 scale = effect.transform.localScale;
                scale.x = -Mathf.Abs(scale.x);
                effect.transform.localScale = scale;
            }

            
            
            
            Destroy(effect, 0.5f); 
        }

        
        Collider2D[] enemies = Physics2D.OverlapCircleAll(spawnPosition, weapon.attackRange * 0.5f, LayerMask.GetMask("Enemy"));

        
        float finalDamage = SkillTreeManager.Instance.GetFinalWeaponDamage(weapon);

        foreach (var enemy in enemies)
        {
            Debug.Log($"[금강령] {enemy.name} 적중! 데미지: {finalDamage}");
            enemy.GetComponent<Health>()?.TakeDamage(finalDamage);
        }
        

        //[1203 추가] 금강령 공격 사운드 재생
        if (audioSource != null && weapon.attackSound != null)
        {
            // 연사 시 소리 끊김 방지
            audioSource.PlayOneShot(weapon.attackSound); 
        }

        // 디버그 시각화
        Debug.DrawRay(spawnPosition, Vector3.up, Color.cyan, 0.5f);
    }

    
    private void DoProjectileAttack(WeaponData weapon)
    {
        if (weapon.projectilePrefab != null)
        {
            Vector2 direction = controller.IsFacingRight ? Vector2.right : Vector2.left;

            
            Vector2 spawnPosition = (Vector2)transform.position + (direction * weapon.spawnOffset);

            GameObject projectileObj = Instantiate(weapon.projectilePrefab, spawnPosition, Quaternion.identity);

            Projectile projectile = projectileObj.GetComponent<Projectile>();
            if (projectile != null)
            {
                
                float finalDamage = SkillTreeManager.Instance.GetFinalWeaponDamage(weapon);
                projectile.Initialize(finalDamage, 10f, direction);
                
            }

            // [1203 수정] 투사체 발사 사운드 재생
            if (audioSource != null && weapon.attackSound != null)
            {
                // 연사 시 소리 끊김 방지
                audioSource.PlayOneShot(weapon.attackSound); 
            }
        }
    }

    
    private void DoMeleeAttack(WeaponData weapon)
    {
        Vector2 direction = controller.IsFacingRight ? Vector2.right : Vector2.left;

        
        Vector2 center = (Vector2)transform.position + (direction * weapon.spawnOffset);

        
        if (weapon.meleeSlashPrefab != null)
        {
            
            
            Vector2 effectPos = (Vector2)transform.position + (direction * 0.5f);

            GameObject slash = Instantiate(weapon.meleeSlashPrefab, effectPos, Quaternion.identity);

            
            if (!controller.IsFacingRight)
            {
                Vector3 scale = slash.transform.localScale;
                scale.x = -Mathf.Abs(scale.x);
                slash.transform.localScale = scale;
            }

            //[1203 추가] 금강저 공격 사운드 재생
            if (audioSource != null && weapon.attackSound != null)
            {
                // 연사 시 소리 끊김 방지
                audioSource.PlayOneShot(weapon.attackSound); 
            }

            // (만약 AutoDestroy 스크립트가 없다면 안전하게 삭제)
            Destroy(slash, 0.5f);
        }
        

        
        
        Vector2 boxSize = new Vector2(weapon.attackRange, 1.5f);

        
        Collider2D[] enemies = Physics2D.OverlapBoxAll(center, boxSize, 0, LayerMask.GetMask("Enemy"));

        
        float finalDamage = SkillTreeManager.Instance.GetFinalWeaponDamage(weapon);

        foreach (var enemy in enemies)
        {
            enemy.GetComponent<Health>()?.TakeDamage(finalDamage);
            Debug.Log($"[금강저/Box] {enemy.name} 베기! 데미지: {finalDamage}");
        }
        

    }
    
    private void OnDrawGizmosSelected()
    {
        if (controller == null) controller = GetComponent<PlayerController>();

        
        WeaponData weapon = controller.CurrentMainWeapon;
        if (weapon == null) return;

        Vector2 direction = controller.IsFacingRight ? Vector2.right : Vector2.left;
        Gizmos.color = Color.red;

        
        if (weapon.weaponId == 1)
        {
            Vector2 spawnPos = (Vector2)transform.position + (direction * weapon.spawnOffset);
            Gizmos.DrawWireSphere(spawnPos, weapon.attackRange * 0.5f);
        }
        
        else if (weapon.weaponId == 3)
        {
            Vector2 center = (Vector2)transform.position + (direction * weapon.attackRange * 0.5f);
            Vector2 boxSize = new Vector2(weapon.attackRange, 1.5f);
            Gizmos.DrawWireCube(center, boxSize); 
        }
    }
}