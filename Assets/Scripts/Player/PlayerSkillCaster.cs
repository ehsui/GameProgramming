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
            case SkillType.Heal: // (추가)
                StartCoroutine(CastHeal(skill));
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

        // 발사 위치 (플레이어 조금 앞)
        Vector2 spawnPos = (Vector2)transform.position + (dir * 1.0f);

        for (int i = 0; i < skill.count; i++)
        {
            if (skill.skillPrefab != null)
            {
                // 1. 투사체 생성
                GameObject projObj = Instantiate(skill.skillPrefab, spawnPos, Quaternion.identity);

                // 2. 크기 조절 (선택 사항)
                // 작은 투사체 여러 개를 쏘는 느낌을 위해 크기를 약간 줄임 (0.7배)
                // (프리팹 자체를 작게 만들었다면 이 줄은 지워도 됩니다)
                projObj.transform.localScale *= 0.7f;

                // 3. 투사체 초기화
                Projectile p = projObj.GetComponent<Projectile>();
                if (p != null)
                {
                    // 속도는 조금 빠르게(15f -> 20f)
                    p.Initialize(damage, 20f, dir);
                }
            }

            // 4. 연사 간격 (0.1초)
            yield return new WaitForSeconds(0.1f);
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

        // 공격 중심점 (X축은 앞쪽으로 나감)
        Vector2 center = (Vector2)transform.position + (dir * skill.range * 0.5f);
        float damage = stats.level * skill.damageMultiplier;

        // ▼▼▼ [수정: 발바닥 위치 계산] ▼▼▼
        if (skill.skillPrefab != null)
        {
            // 1. 플레이어의 콜라이더 가져오기
            Collider2D col = GetComponent<Collider2D>();

            // 2. 콜라이더의 가장 아래쪽(Min Y) 좌표 찾기 = 발바닥 높이
            // (콜라이더가 없다면 그냥 현재 위치에서 0.8 정도 내림)
            float feetY = (col != null) ? col.bounds.min.y : transform.position.y - 0.8f;

            // 3. 이펙트 생성 위치: X는 공격 중심, Y는 발바닥
            Vector2 effectPos = new Vector2(center.x, feetY);

            Instantiate(skill.skillPrefab, effectPos, Quaternion.identity);
        }
        // ▲▲▲ -------------------------- ▲▲▲

        // 범위 타격 (데미지 판정은 여전히 '중심'을 기준으로 하는 게 좋습니다)
        Collider2D[] enemies = Physics2D.OverlapCircleAll(center, skill.range * 0.5f, LayerMask.GetMask("Enemy"));

        foreach (var enemy in enemies)
        {
            enemy.GetComponent<Health>()?.TakeDamage(damage);
            // 넉백 로직...
            if (skill.pushForce > 0)
            {
                Rigidbody2D enemyRigid = enemy.GetComponent<Rigidbody2D>();
                if (enemyRigid != null)
                    enemyRigid.AddForce(dir * skill.pushForce, ForceMode2D.Impulse);
            }
        }

        Debug.DrawRay(center, Vector2.up * 2, Color.magenta, 1.0f);
    }

    // 5. 돌진 (2-3) - 3연타 돌진
    private IEnumerator CastDash(SkillData skill)
    {
        float damage = stats.level * skill.damageMultiplier;
        Vector2 dir = controller.IsFacingRight ? Vector2.right : Vector2.left;

        // 1. 물리 설정 변경
        float originalGravity = controller.rigid.gravityScale;
        controller.rigid.gravityScale = 0;

        // 2. 이펙트 생성
        GameObject dashEffect = null;
        if (skill.skillPrefab != null)
        {
            dashEffect = Instantiate(skill.skillPrefab, transform.position, Quaternion.identity);
            dashEffect.transform.SetParent(this.transform); // 플레이어 따라다니게
            dashEffect.transform.localPosition = Vector3.zero;

            // ▼▼▼ [수정된 부분] 원래 크기를 유지하면서 방향만 뒤집기 ▼▼▼
            Vector3 originalScale = dashEffect.transform.localScale;

            if (!controller.IsFacingRight)
            {
                // 왼쪽 볼 때: 원래 X크기에 마이너스를 붙임
                dashEffect.transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            }
            else
            {
                // 오른쪽 볼 때: 원래 X크기 (양수) 유지
                dashEffect.transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            }
            // ▲▲▲ ---------------------------------------------- ▲▲▲
        }

        // 3. 돌진 루프
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

        // 4. 종료 및 복구
        controller.rigid.velocity = Vector2.zero;
        controller.rigid.gravityScale = originalGravity;

        if (dashEffect != null) Destroy(dashEffect);
    }

    // 6. 연타 (3-3)
    private IEnumerator CastCombo(SkillData skill)
    {
        Vector2 dir = controller.IsFacingRight ? Vector2.right : Vector2.left;

        // 공격 중심점 (플레이어 앞쪽)
        // (연타는 범위가 좀 넓어야 잘 맞으므로 range를 넉넉히 잡으세요)
        Vector2 center = (Vector2)transform.position + (dir * skill.range * 0.5f);

        float damage = stats.level * skill.damageMultiplier;

        // skill.count(5회) 만큼 반복
        for (int i = 0; i < skill.count; i++)
        {
            // 1. 이펙트 생성 (매 타격마다 생성!)
            if (skill.skillPrefab != null)
            {
                // 위치를 살짝 랜덤하게 흩뿌려서 '난무'하는 느낌 주기
                Vector2 randomOffset = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
                Vector2 effectPos = center + randomOffset;

                GameObject slash = Instantiate(skill.skillPrefab, effectPos, Quaternion.identity);

                // 방향 반전 (왼쪽 볼 때)
                if (dir.x < 0)
                {
                    Vector3 scale = slash.transform.localScale;
                    scale.x = -Mathf.Abs(scale.x);
                    slash.transform.localScale = scale;
                }

                // (AutoDestroy가 프리팹에 있다면 생략 가능하지만 안전하게 삭제)
                Destroy(slash, 0.3f);
            }

            // 2. 범위 타격 (원형 범위)
            // (금강저 기본 공격은 네모지만, 난무는 사방으로 휘두르니 원형이 자연스럽습니다)
            Collider2D[] enemies = Physics2D.OverlapCircleAll(center, skill.range * 0.5f, LayerMask.GetMask("Enemy"));
            foreach (var enemy in enemies)
            {
                enemy.GetComponent<Health>()?.TakeDamage(damage);
                Debug.Log($"[Combo] 연타 {i + 1}회 적중!");

                // (선택) 적에게 아주 살짝 경직(멈춤)을 주거나 밀어내기 효과 추가 가능
            }

            // 3. 다음 타격까지 대기 (점점 빨라지게 하거나 등속으로)
            // 0.1초 간격으로 매우 빠르게 때립니다.
            yield return new WaitForSeconds(0.1f);
        }
    }
    // [신규 메서드] 지속 회복
    private IEnumerator CastHeal(SkillData skill)
    {
        // 1. 치유 이펙트 생성 (초록색 십자가나 오라)
        GameObject healEffect = null;
        if (skill.skillPrefab != null)
        {
            healEffect = Instantiate(skill.skillPrefab, transform.position, Quaternion.identity);
            healEffect.transform.SetParent(this.transform); // 플레이어 따라다니게
            healEffect.transform.localPosition = Vector3.zero;
        }

        // 2. 지속 회복 루프
        float timer = 0f;
        float interval = 1.0f; // 1초마다 회복
        float nextHealTime = 0f;

        // DamageMultiplier를 '1회당 회복량'으로 사용
        float healAmount = stats.level * skill.damageMultiplier;

        Debug.Log($"지속 치유 시작! (총 {skill.duration}초)");

        while (timer < skill.duration)
        {
            if (timer >= nextHealTime)
            {
                // 플레이어 체력 회복
                stats.Heal(healAmount);

                // (선택) 회복될 때마다 머리 위에 숫자 띄우기 or 반짝임 효과 추가 가능

                nextHealTime += interval;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // 3. 종료 (이펙트 삭제)
        if (healEffect != null) Destroy(healEffect);
        Debug.Log("치유 종료");
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