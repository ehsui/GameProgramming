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
            enemy.GetComponent<Health>()?.TakeDamage(weapon.damage);
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

    // 3번 무기 (금강저) - Box 형태 + 이펙트 포함
    private void DoMeleeAttack(WeaponData weapon)
    {
        Vector2 direction = controller.IsFacingRight ? Vector2.right : Vector2.left;

        // 1. 히트박스 중심점 (SpawnOffset 사용)
        Vector2 center = (Vector2)transform.position + (direction * weapon.spawnOffset);

        // ▼▼▼ [사라졌던 이펙트 로직 복구!] ▼▼▼
        if (weapon.meleeSlashPrefab != null)
        {
            // 이펙트는 플레이어 몸에서 조금만 앞에서 나오게 (히트박스 중심보다 약간 뒤쪽이 자연스러움)
            // 필요하면 0.5f 대신 weapon.spawnOffset을 써도 됩니다.
            Vector2 effectPos = (Vector2)transform.position + (direction * 0.5f);

            GameObject slash = Instantiate(weapon.meleeSlashPrefab, effectPos, Quaternion.identity);

            // 방향 반전 (왼쪽 볼 때 뒤집기)
            if (!controller.IsFacingRight)
            {
                Vector3 scale = slash.transform.localScale;
                scale.x = -Mathf.Abs(scale.x);
                slash.transform.localScale = scale;
            }

            // (만약 AutoDestroy 스크립트가 없다면 안전하게 삭제)
            Destroy(slash, 0.5f);
        }
        // ▲▲▲ ---------------------------- ▲▲▲

        // 2. 네모난 히트박스 설정
        // 가로: 사거리(attackRange), 세로: 1.5f
        Vector2 boxSize = new Vector2(weapon.attackRange, 1.5f);

        // 3. 판정 (OverlapBoxAll)
        Collider2D[] enemies = Physics2D.OverlapBoxAll(center, boxSize, 0, LayerMask.GetMask("Enemy"));

        foreach (var enemy in enemies)
        {
            enemy.GetComponent<Health>()?.TakeDamage(weapon.damage);
            Debug.Log($"[금강저/Box] {enemy.name} 베기! 데미지: {weapon.damage}");
        }
    }
    // 에디터에서 공격 범위를 눈으로 확인하는 기능
    private void OnDrawGizmosSelected()
    {
        if (controller == null) controller = GetComponent<PlayerController>();

        // 현재 들고 있는 무기 가져오기
        WeaponData weapon = controller.CurrentMainWeapon;
        if (weapon == null) return;

        Vector2 direction = controller.IsFacingRight ? Vector2.right : Vector2.left;
        Gizmos.color = Color.red;

        // 1번 금강령 (원형)
        if (weapon.weaponId == 1)
        {
            Vector2 spawnPos = (Vector2)transform.position + (direction * weapon.spawnOffset);
            Gizmos.DrawWireSphere(spawnPos, weapon.attackRange * 0.5f);
        }
        // 3번 금강저 (지금은 원형이지만 곧 네모로 바꿀 예정)
        else if (weapon.weaponId == 3)
        {
            Vector2 center = (Vector2)transform.position + (direction * weapon.attackRange * 0.5f);
            Vector2 boxSize = new Vector2(weapon.attackRange, 1.5f);
            Gizmos.DrawWireCube(center, boxSize); // 네모 그리기
        }
    }
}