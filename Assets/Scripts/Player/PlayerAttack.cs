using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerController controller;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    public void PerformAttack(WeaponData weapon)
    {
        if (weapon == null) return;

        switch (weapon.weaponId)
        {
            case 1: // 금강령: 즉발 범위 공격 (히트스캔 이펙트)
                DoInstantAreaAttack(weapon);
                break;
            case 2: // 석장: 투사체 발사 (기존 유지)
                DoProjectileAttack(weapon);
                break;
            case 3: // 금강저: 근접 공격 (기존 유지)
                DoMeleeAttack(weapon);
                break;
        }
    }

    // [수정됨] 1번 무기용: 이펙트 소환 + 즉시 데미지
    private void DoInstantAreaAttack(WeaponData weapon)
    {
        Vector2 direction = controller.IsFacingRight ? Vector2.right : Vector2.left;

        // 이펙트가 생성될 위치 (플레이어 앞쪽 일정 거리)
        Vector2 spawnPosition = (Vector2)transform.position + (direction * weapon.spawnOffset);

        // 1. 이펙트 프리팹 생성 (투사체처럼 날아가지 않음)
        if (weapon.projectilePrefab != null) // WeaponData의 ProjectilePrefab 슬롯에 '이펙트'를 넣으세요
        {
            GameObject effect = Instantiate(weapon.projectilePrefab, spawnPosition, Quaternion.identity);

            // 이펙트 좌우 반전 (왼쪽 볼 때)
            if (direction.x < 0)
            {
                Vector3 scale = effect.transform.localScale;
                scale.x = -Mathf.Abs(scale.x);
                effect.transform.localScale = scale;
            }

            // 이펙트는 제자리에 가만히 있다가 사라짐 (Projectile 스크립트 필요 없음)
            // 만약 Projectile 스크립트가 붙어있다면 Rigidbody 속도를 0으로 만들어야 함.
            // 가장 좋은 건 Projectile 스크립트 없는 순수 이펙트 프리팹을 쓰는 것입니다.
            Destroy(effect, 0.5f); // 0.5초 뒤 삭제
        }

        // 2. 데미지 판정 (OverlapCircle)
        Collider2D[] enemies = Physics2D.OverlapCircleAll(spawnPosition, weapon.attackRange * 0.5f, LayerMask.GetMask("Enemy"));

        foreach (var enemy in enemies)
        {
            Debug.Log($"[금강령] {enemy.name} 적중! 데미지: {weapon.damage}");
            // enemy.GetComponent<Health>()?.TakeDamage(weapon.damage);
        }

        // 디버그 시각화
        Debug.DrawRay(spawnPosition, Vector3.up, Color.cyan, 0.5f);
    }

    // 2번 무기용 (투사체) - 기존 유지
    private void DoProjectileAttack(WeaponData weapon)
    {
        if (weapon.projectilePrefab != null)
        {
            Vector2 direction = controller.IsFacingRight ? Vector2.right : Vector2.left;

            // [수정] 투사체도 spawnOffset 만큼 앞에서 생성되도록 변경
            Vector2 spawnPosition = (Vector2)transform.position + (direction * weapon.spawnOffset);

            GameObject projectileObj = Instantiate(weapon.projectilePrefab, spawnPosition, Quaternion.identity);

            Projectile projectile = projectileObj.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Initialize(weapon.damage, 10f, direction);
            }
        }
    }

    // 3번 무기용 (근접) - 기존 유지
    private void DoMeleeAttack(WeaponData weapon)
    {
        Vector2 direction = controller.IsFacingRight ? Vector2.right : Vector2.left;
        Vector2 center = (Vector2)transform.position + (direction * weapon.attackRange * 0.5f);
        Collider2D[] enemies = Physics2D.OverlapCircleAll(center, weapon.attackRange * 0.5f, LayerMask.GetMask("Enemy"));

        foreach (var enemy in enemies)
        {
            Debug.Log($"[금강저] {enemy.name} 베기! 데미지: {weapon.damage}");
            // enemy.GetComponent<Health>()?.TakeDamage(weapon.damage);
        }
    }
}