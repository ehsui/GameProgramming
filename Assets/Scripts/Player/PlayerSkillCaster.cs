using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillCaster : MonoBehaviour
{
    [Header("Settings")]
    public List<SkillCombinationData> combinations;

    private PlayerController controller;
    private PlayerStats stats;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        stats = GetComponent<PlayerStats>();
    }

    
    public SkillData GetCurrentSkill()
    {
        int mainId = controller.CurrentMainWeapon != null ? controller.CurrentMainWeapon.weaponId : -1;
        int subId = controller.CurrentSubWeapon != null ? controller.CurrentSubWeapon.weaponId : -1;

        foreach (var combo in combinations)
        {
            if (combo.mainWeaponId == mainId && combo.subWeaponId == subId)
            {
                if (CanUseSkill(combo.skillData)) return combo.skillData;
                else return null;
            }
        }
        return null;
    }

    private bool CanUseSkill(SkillData data)
    {
        if (data == null) return false;
        if (stats.CurrentBodhicitta < data.bodhicittaCost)
        {
            Debug.Log("보리심 부족!");
            return false;
        }
        return true;
    }

    
    public void CastSkill(SkillData skill)
    {
        if (skill == null) return;

        
        stats.UseBodhicitta(skill.bodhicittaCost);
        Debug.Log($"[Skill Cast] {skill.skillName} ({skill.skillType})");

        
        switch (skill.skillType)
        {
            case SkillType.Projectile:
                CastProjectile(skill);
                break;
            case SkillType.MultiProjectile:
                StartCoroutine(CastMultiProjectile(skill));
                break;
            case SkillType.Buff:
                StartCoroutine(CastBuff(skill));
                break;
            case SkillType.AreaAttack:
                CastAreaAttack(skill);
                break;
            case SkillType.Dash:
                StartCoroutine(CastDash(skill));
                break;
            case SkillType.Combo:
                StartCoroutine(CastCombo(skill));
                break;
            case SkillType.Laser: 
                CastLaser(skill);
                break;
            case SkillType.Stealth: 
                StartCoroutine(CastStealth(skill));
                break;
            case SkillType.Heal: 
                StartCoroutine(CastHeal(skill));
                break;
        }
    }

    
    private void CastProjectile(SkillData skill)
    {
        if (skill.skillPrefab == null) { Debug.Log("투사체 프리팹 없음"); return; }

        Vector2 dir = controller.IsFacingRight ? Vector2.right : Vector2.left;
        float damage = stats.level * skill.damageMultiplier;

        GameObject proj = Instantiate(skill.skillPrefab, transform.position, Quaternion.identity);

        
        Projectile p = proj.GetComponent<Projectile>();
        if (p != null) p.Initialize(damage, 15f, dir);
    }

    
    private IEnumerator CastMultiProjectile(SkillData skill)
    {
        Vector2 dir = controller.IsFacingRight ? Vector2.right : Vector2.left;
        float damage = stats.level * skill.damageMultiplier;

        
        Vector2 spawnPos = (Vector2)transform.position + (dir * 1.0f);

        for (int i = 0; i < skill.count; i++)
        {
            if (skill.skillPrefab != null)
            {
                
                GameObject projObj = Instantiate(skill.skillPrefab, spawnPos, Quaternion.identity);

                
                
                
                projObj.transform.localScale *= 0.7f;

                
                Projectile p = projObj.GetComponent<Projectile>();
                if (p != null)
                {
                    
                    p.Initialize(damage, 20f, dir);
                }
            }

            
            yield return new WaitForSeconds(0.1f);
        }
    }

    
    private IEnumerator CastBuff(SkillData skill)
    {
        
        stats.isInvincible = true;

        

        
        GameObject buffEffect = null;
        if (skill.skillPrefab != null)
        {
            
            buffEffect = Instantiate(skill.skillPrefab, transform.position, Quaternion.identity);
            buffEffect.transform.SetParent(this.transform);
            buffEffect.transform.localPosition = Vector3.zero;
        }

        
        yield return new WaitForSeconds(skill.duration);

        
        stats.isInvincible = false;

        
        if (buffEffect != null) Destroy(buffEffect);

        Debug.Log("버프(무적) 종료");
    }

    
    private void CastAreaAttack(SkillData skill)
    {
        Vector2 dir = controller.IsFacingRight ? Vector2.right : Vector2.left;

        
        Vector2 center = (Vector2)transform.position + (dir * skill.range * 0.5f);
        float damage = stats.level * skill.damageMultiplier;

        
        if (skill.skillPrefab != null)
        {
            
            Collider2D col = GetComponent<Collider2D>();

            
            
            float feetY = (col != null) ? col.bounds.min.y : transform.position.y - 0.8f;

            
            Vector2 effectPos = new Vector2(center.x, feetY);

            Instantiate(skill.skillPrefab, effectPos, Quaternion.identity);
        }
        

        
        Collider2D[] enemies = Physics2D.OverlapCircleAll(center, skill.range * 0.5f, LayerMask.GetMask("Enemy"));

        foreach (var enemy in enemies)
        {
            enemy.GetComponent<Health>()?.TakeDamage(damage);
            
            if (skill.pushForce > 0)
            {
                Rigidbody2D enemyRigid = enemy.GetComponent<Rigidbody2D>();
                if (enemyRigid != null)
                    enemyRigid.AddForce(dir * skill.pushForce, ForceMode2D.Impulse);
            }
        }

        Debug.DrawRay(center, Vector2.up * 2, Color.magenta, 1.0f);
    }

    
    private IEnumerator CastDash(SkillData skill)
    {
        float damage = stats.level * skill.damageMultiplier;
        Vector2 dir = controller.IsFacingRight ? Vector2.right : Vector2.left;

        
        float originalGravity = controller.rigid.gravityScale;
        controller.rigid.gravityScale = 0;

        
        GameObject dashEffect = null;
        if (skill.skillPrefab != null)
        {
            dashEffect = Instantiate(skill.skillPrefab, transform.position, Quaternion.identity);
            dashEffect.transform.SetParent(this.transform); 
            dashEffect.transform.localPosition = Vector3.zero;

            
            Vector3 originalScale = dashEffect.transform.localScale;

            if (!controller.IsFacingRight)
            {
                
                dashEffect.transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            }
            else
            {
                
                dashEffect.transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            }
            
        }

        
        float timer = 0f;
        float hitInterval = skill.duration / 3.0f;
        float nextHitTime = 0f;

        while (timer < skill.duration)
        {
            float dashSpeed = skill.pushForce > 0 ? skill.pushForce : 20f;
            controller.rigid.velocity = dir * dashSpeed;

            if (timer >= nextHitTime)
            {
                Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 1.0f, LayerMask.GetMask("Enemy"));
                foreach (var enemy in enemies)
                {
                    enemy.GetComponent<Health>()?.TakeDamage(damage);
                    Debug.Log($"[Dash] {enemy.name} 돌진 타격!");

                    Rigidbody2D eRigid = enemy.GetComponent<Rigidbody2D>();
                    if (eRigid != null) eRigid.AddForce(dir * 5f, ForceMode2D.Impulse);
                }
                nextHitTime += hitInterval;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        
        controller.rigid.velocity = Vector2.zero;
        controller.rigid.gravityScale = originalGravity;

        if (dashEffect != null) Destroy(dashEffect);
    }

    
    private IEnumerator CastCombo(SkillData skill)
    {
        Vector2 dir = controller.IsFacingRight ? Vector2.right : Vector2.left;

        
        
        Vector2 center = (Vector2)transform.position + (dir * skill.range * 0.5f);

        float damage = stats.level * skill.damageMultiplier;

        
        for (int i = 0; i < skill.count; i++)
        {
            
            if (skill.skillPrefab != null)
            {
                
                Vector2 randomOffset = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
                Vector2 effectPos = center + randomOffset;

                GameObject slash = Instantiate(skill.skillPrefab, effectPos, Quaternion.identity);

                
                if (dir.x < 0)
                {
                    Vector3 scale = slash.transform.localScale;
                    scale.x = -Mathf.Abs(scale.x);
                    slash.transform.localScale = scale;
                }

                
                Destroy(slash, 0.3f);
            }

            
            
            Collider2D[] enemies = Physics2D.OverlapCircleAll(center, skill.range * 0.5f, LayerMask.GetMask("Enemy"));
            foreach (var enemy in enemies)
            {
                enemy.GetComponent<Health>()?.TakeDamage(damage);
                Debug.Log($"[Combo] 연타 {i + 1}회 적중!");

                
            }

            
            
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    private IEnumerator CastHeal(SkillData skill)
    {
        
        GameObject healEffect = null;
        if (skill.skillPrefab != null)
        {
            healEffect = Instantiate(skill.skillPrefab, transform.position, Quaternion.identity);
            healEffect.transform.SetParent(this.transform); 
            healEffect.transform.localPosition = Vector3.zero;
        }

        
        float timer = 0f;
        float interval = 1.0f; 
        float nextHealTime = 0f;

        
        float healAmount = stats.level * skill.damageMultiplier;

        Debug.Log($"지속 치유 시작! (총 {skill.duration}초)");

        while (timer < skill.duration)
        {
            if (timer >= nextHealTime)
            {
                
                stats.Heal(healAmount);

                

                nextHealTime += interval;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        
        if (healEffect != null) Destroy(healEffect);
        Debug.Log("치유 종료");
    }
    private void CastLaser(SkillData skill)
    {
        Vector2 dir = controller.IsFacingRight ? Vector2.right : Vector2.left;
        float damage = stats.level * skill.damageMultiplier;

        
        
        Vector2 spawnPos = (Vector2)transform.position + (dir * 1.0f);

        if (skill.skillPrefab != null)
        {
            GameObject laser = Instantiate(skill.skillPrefab, spawnPos, Quaternion.identity);

            
            if (dir.x < 0)
            {
                
                
                laser.transform.rotation = Quaternion.Euler(0, 180, 0);
            }

            
        }

        
        
        Vector2 boxCenter = spawnPos + (dir * skill.range * 0.5f);

        
        Vector2 boxSize = new Vector2(skill.range, 1.0f);

        
        Collider2D[] enemies = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0, LayerMask.GetMask("Enemy"));

        foreach (var enemy in enemies)
        {
            enemy.GetComponent<Health>()?.TakeDamage(damage);
            Debug.Log($"[Laser] {enemy.name} 관통! 데미지: {damage}");
        }

        
        Debug.DrawRay(spawnPos, dir * skill.range, Color.cyan, 0.5f); 
    }
    private IEnumerator CastStealth(SkillData skill)
    {
        
        Color originalColor = controller.spriteRenderer.color;
        controller.spriteRenderer.color = new Color(1f, 1f, 1f, 0.4f);

        
        if (skill.skillPrefab != null)
        {
            Instantiate(skill.skillPrefab, transform.position, Quaternion.identity);
        }

        
        GameObject auraEffect = null;
        if (skill.loopEffectPrefab != null)
        {
            
            auraEffect = Instantiate(skill.loopEffectPrefab, transform.position, Quaternion.identity);
            auraEffect.transform.SetParent(this.transform);
            auraEffect.transform.localPosition = Vector3.zero; 
        }
        

        
        float timer = 0f;
        float nextDamageTime = 0f;
        float damage = stats.level * skill.damageMultiplier;

        while (timer < skill.duration)
        {
            if (timer >= nextDamageTime)
            {
                Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, skill.range, LayerMask.GetMask("Enemy"));
                foreach (var enemy in enemies)
                {
                    enemy.GetComponent<Health>()?.TakeDamage(damage);
                    Debug.Log($"[Stealth DoT] {enemy.name} 독 데미지: {damage}");
                }
                nextDamageTime += 0.5f; 
            }

            timer += Time.deltaTime;
            yield return null;
        }

        
        controller.spriteRenderer.color = originalColor;
        stats.isInvincible = false;

        
        if (auraEffect != null) Destroy(auraEffect);
    }
    
    private void OnDrawGizmos()
    {
        if (controller == null) controller = GetComponent<PlayerController>();

        SkillData skill = GetCurrentSkill();
        if (skill == null) return;

        
        else if (skill.skillType == SkillType.Stealth)
        {
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, skill.range);
        }
    }
}