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
            case SkillType.Laser: // (추가)
                CastLaser(skill);
                break;
            case SkillType.Stealth: // (추가)
                StartCoroutine(CastStealth(skill));
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

    // 3. 버프 (1-2: 무적) - 반투명 효과 제거됨
    private IEnumerator CastBuff(SkillData skill)
    {
        // 1. 무적 적용
        stats.isInvincible = true;

        // (반투명 만드는 코드는 삭제했습니다. 나중에 은신 스킬 만들 때 따로 추가할게요!)

        // 2. 보호막 이펙트 생성
        GameObject buffEffect = null;
        if (skill.skillPrefab != null)
        {
            // 플레이어 위치에 생성하고 자식으로 붙이기
            buffEffect = Instantiate(skill.skillPrefab, transform.position, Quaternion.identity);
            buffEffect.transform.SetParent(this.transform);
            buffEffect.transform.localPosition = Vector3.zero;
        }

        // 3. 지속 시간 대기
        yield return new WaitForSeconds(skill.duration);

        // 4. 원상 복구
        stats.isInvincible = false;

        // 이펙트 삭제
        if (buffEffect != null) Destroy(buffEffect);

        Debug.Log("버프(무적) 종료");
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
    private void CastLaser(SkillData skill)
    {
        Vector2 dir = controller.IsFacingRight ? Vector2.right : Vector2.left;
        float damage = stats.level * skill.damageMultiplier;

        // 1. 레이저 이펙트 생성 (플레이어 약간 앞에서)
        // (spawnOffset이 WeaponData에만 있고 SkillData엔 없으므로 일단 1.0f 고정하거나 추가 필요)
        Vector2 spawnPos = (Vector2)transform.position + (dir * 1.0f);

        if (skill.skillPrefab != null)
        {
            GameObject laser = Instantiate(skill.skillPrefab, spawnPos, Quaternion.identity);

            // 레이저 방향 회전 (오른쪽이 기본인 프리팹 기준)
            if (dir.x < 0)
            {
                // 왼쪽 볼 때: 180도 회전 or 스케일 반전
                // 레이저는 길기 때문에 스케일 반전보다는 회전이 더 안전할 수 있음
                laser.transform.rotation = Quaternion.Euler(0, 180, 0);
            }

            // 레이저도 애니메이션 끝나면 사라져야 함 (AutoDestroy 필수)
        }

        // 2. 히트박스(공격 범위) 계산
        // 박스의 중심점: 발사 위치에서 사거리의 절반만큼 더 나간 곳
        Vector2 boxCenter = spawnPos + (dir * skill.range * 0.5f);

        // 박스 크기: 가로는 사거리(range), 세로는 1.0f (레이저 굵기)
        Vector2 boxSize = new Vector2(skill.range, 1.0f);

        // 3. 히트스캔 판정 (OverlapBoxAll)
        Collider2D[] enemies = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0, LayerMask.GetMask("Enemy"));

        foreach (var enemy in enemies)
        {
            enemy.GetComponent<Health>()?.TakeDamage(damage);
            Debug.Log($"[Laser] {enemy.name} 관통! 데미지: {damage}");
        }

        // 4. 디버그용 박스 그리기
        Debug.DrawRay(spawnPos, dir * skill.range, Color.cyan, 0.5f); // 중심선
    }
    private IEnumerator CastStealth(SkillData skill)
    {
        // 1. 시각적 효과: 반투명
        Color originalColor = controller.spriteRenderer.color;
        controller.spriteRenderer.color = new Color(1f, 1f, 1f, 0.4f);

        // 2. 연기 이펙트 (1회성 펑!)
        if (skill.skillPrefab != null)
        {
            Instantiate(skill.skillPrefab, transform.position, Quaternion.identity);
        }

        // ▼▼▼ [추가] 지속 오라 이펙트 (독구름) 생성 ▼▼▼
        GameObject auraEffect = null;
        if (skill.loopEffectPrefab != null)
        {
            // 생성 후 플레이어 자식으로 설정 (따라다니게)
            auraEffect = Instantiate(skill.loopEffectPrefab, transform.position, Quaternion.identity);
            auraEffect.transform.SetParent(this.transform);
            auraEffect.transform.localPosition = Vector3.zero; // 위치 정렬
        }
        // ▲▲▲ ---------------------------------- ▲▲▲

        // 3. 지속 데미지 루프
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
                nextDamageTime += 0.5f; // 0.5초 간격
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // 4. 종료 처리
        controller.spriteRenderer.color = originalColor;
        stats.isInvincible = false;

        // ▼▼▼ [추가] 오라 이펙트 삭제 ▼▼▼
        if (auraEffect != null) Destroy(auraEffect);
    }
    // [수정] 디버깅용 기즈모 (범위 그리기)
    private void OnDrawGizmos()
    {
        if (controller == null) controller = GetComponent<PlayerController>();

        SkillData skill = GetCurrentSkill();
        if (skill == null) return;

        // ▼▼▼ [추가] 1-3 은신 (원형 범위) ▼▼▼
        else if (skill.skillType == SkillType.Stealth)
        {
            // 독구름 범위를 초록색 원으로 표시
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, skill.range);
        }
    }
}