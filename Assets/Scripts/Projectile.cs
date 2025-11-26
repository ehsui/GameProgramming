using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float damage;
    private float speed;
    private Vector2 direction;
    private Rigidbody2D rigid;

    [Header("Explosion Settings")]
    public bool isExplosive = false;      // 폭발 여부 체크
    public float explosionRadius = 2.0f;  // 폭발 반경
    public GameObject explosionEffect;    // 폭발 시 나올 이펙트 (필수)

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Initialize(float damage, float speed, Vector2 direction)
    {
        this.damage = damage;
        this.speed = speed;
        this.direction = direction;

        // 투사체 방향 회전 (날아가는 방향을 바라보게)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 왼쪽을 볼 때 스프라이트 뒤집기 (필요시)
        if (direction.x < 0)
        {
            Vector3 scale = transform.localScale;
            scale.y = -Mathf.Abs(scale.y); // X축 회전 대신 Y 스케일을 뒤집어 상하 반전을 막음
            transform.localScale = scale;
        }

        if (rigid != null)
        {
            rigid.velocity = this.direction * this.speed;
        }

        Destroy(gameObject, 5f); // 5초 뒤 삭제
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 적이나 벽에 부딪혔을 때
        bool isEnemy = collision.CompareTag("Enemy") || (1 << collision.gameObject.layer & LayerMask.GetMask("Enemy")) != 0;
        bool isPlatform = (1 << collision.gameObject.layer & LayerMask.GetMask("Platform")) != 0;

        if (isEnemy || isPlatform)
        {
            if (isExplosive)
            {
                Explode(); // 폭발 로직 실행
            }
            else if (isEnemy)
            {
                // 일반 단일 타격 (기존 로직)
                Health target = collision.GetComponent<Health>();
                if (target != null) target.TakeDamage(this.damage);
            }

            // 투사체 자체는 삭제
            Destroy(gameObject);
        }
    }

    // 💥 폭발 로직
    private void Explode()
    {
        // 1. 폭발 이펙트 생성
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // 2. 범위 데미지 (OverlapCircle)
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius, LayerMask.GetMask("Enemy"));

        foreach (var enemy in enemies)
        {
            enemy.GetComponent<Health>()?.TakeDamage(this.damage);
            Debug.Log($"[폭발] {enemy.name} 휘말림! 데미지: {damage}");
        }

        // 디버그 (빨간 원)
        // (OnDrawGizmos는 MonoBehaviour 함수라 여기서는 즉시 그릴 수 없지만, 로직은 맞습니다)
    }
}