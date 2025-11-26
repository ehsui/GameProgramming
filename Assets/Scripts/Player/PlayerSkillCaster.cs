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

    // 스킬 데이터를 찾아 반환 (상태 진입용)
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

    // --- [핵심: 스킬 실행 메서드] ---
    public void CastSkill(SkillData skill)
    {
        if (skill == null) return;

        // 마나 소모
        stats.UseBodhicitta(skill.bodhicittaCost);
        Debug.Log($"[Skill Cast] {skill.skillName} ({skill.skillType})");

        // 타입별 로직 분기
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
        }
    }

    // 1. 단일 투사체 (1-1, 2-1)
    private void CastProjectile(SkillData skill)
    {
        if (skill.skillPrefab == null) { Debug.Log("투사체 프리팹 없음"); return; }

        Vector2 dir = controller.IsFacingRight ? Vector2.right : Vector2.left;
        float damage = stats.level * skill.damageMultiplier;

        GameObject proj = Instantiate(skill.skillPrefab, transform.position, Quaternion.identity);

        // 투사체 스크립트 초기화 (Projectile.cs가 있다는 가정)
        Projectile p = proj.GetComponent<Projectile>();
        if (p != null) p.Initialize(damage, 15f, dir);
    }

    // 2. 다중 투사체 (3-1: 5연발)
    private IEnumerator CastMultiProjectile(SkillData skill)
    {
        Vector2 dir = controller.IsFacingRight ? Vector2.right : Vector2.left;
        float damage = stats.level * skill.damageMultiplier;

        for (int i = 0; i < skill.count; i++)
        {
            if (skill.skillPrefab != null)
            {
                GameObject proj = Instantiate(skill.skillPrefab, transform.position, Quaternion.identity);
                proj.transform.localScale *= 0.5f; // 조금 작게

                Projectile p = proj.GetComponent<Projectile>();
                if (p != null) p.Initialize(damage, 15f, dir);
            }
            yield return new WaitForSeconds(0.1f); // 0.1초 간격 발사
        }
    }

    // 3. 버프 (1-2: 무적, 1-3: 은신)
    private IEnumerator CastBuff(SkillData skill)
    {
        // 무적 적용
        stats.isInvincible = true;

        // 시각적 효과 (반투명)
        Color originalColor = controller.spriteRenderer.color;
        controller.spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);

        yield return new WaitForSeconds(skill.duration);

        // 원상 복구
        stats.isInvincible = false;
        controller.spriteRenderer.color = originalColor;
        Debug.Log("버프 종료");
    }

    // 4. 범위 공격 & 넉백 (2-2, 3-2)
    private void CastAreaAttack(SkillData skill)
    {
        Vector2 dir = controller.IsFacingRight ? Vector2.right : Vector2.left;
        Vector2 center = (Vector2)transform.position + (dir * skill.range * 0.5f);
        float damage = stats.level * skill.damageMultiplier;

        // 범위 타격
        Collider2D[] enemies = Physics2D.OverlapCircleAll(center, skill.range * 0.5f, LayerMask.GetMask("Enemy"));

        foreach (var enemy in enemies)
        {
            Debug.Log($"{enemy.name} 범위 스킬 적중! 데미지: {damage}");
            enemy.GetComponent<Health>()?.TakeDamage(damage);

            // 넉백 (PushForce가 있을 때만)
            if (skill.pushForce > 0)
            {
                Rigidbody2D enemyRigid = enemy.GetComponent<Rigidbody2D>();
                if (enemyRigid != null)
                {
                    enemyRigid.AddForce(dir * skill.pushForce, ForceMode2D.Impulse);
                }
            }
        }

        // 디버그 표시
        Debug.DrawRay(center, Vector2.up * 2, Color.magenta, 1.0f);
    }

    // 5. 돌진 (2-3) - 물리 제어는 State와 협력 필요
    private IEnumerator CastDash(SkillData skill)
    {
        float damage = stats.level * skill.damageMultiplier;
        Vector2 dir = controller.IsFacingRight ? Vector2.right : Vector2.left;

        // 중력 무시하고 앞으로 발사
        float originalGravity = controller.rigid.gravityScale;
        controller.rigid.gravityScale = 0;

        // 돌진 중에도 타격을 주기 위해 반복 체크
        float timer = 0f;
        while (timer < skill.duration)
        {
            // 강제로 속도 고정 (Dash)
            controller.rigid.velocity = dir * 20f; // 속도는 20으로 고정

            // 지나가는 길에 있는 적 타격
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 1.0f, LayerMask.GetMask("Enemy"));
            foreach (var enemy in enemies)
            {
                // 데미지 주기 (중복 타격 방지 로직은 생략됨)
                enemy.GetComponent<Health>()?.TakeDamage(damage);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // 복구
        controller.rigid.velocity = Vector2.zero;
        controller.rigid.gravityScale = originalGravity;
    }

    // 6. 연타 (3-3)
    private IEnumerator CastCombo(SkillData skill)
    {
        float damage = stats.level * skill.damageMultiplier;
        Vector2 dir = controller.IsFacingRight ? Vector2.right : Vector2.left;
        Vector2 center = (Vector2)transform.position + (dir * skill.range * 0.5f);

        for (int i = 0; i < skill.count; i++)
        {
            // 매번 타격
            Collider2D[] enemies = Physics2D.OverlapCircleAll(center, skill.range * 0.5f, LayerMask.GetMask("Enemy"));
            foreach (var enemy in enemies)
            {
                enemy.GetComponent<Health>()?.TakeDamage(damage);
                Debug.Log($"연타 {i + 1}회 적중");
            }

            // 이펙트 생성 위치 등도 여기서 처리 가능

            yield return new WaitForSeconds(0.1f); // 0.1초 간격 연타
        }
    }
}