using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    private float speed;
    private Vector2 direction;
    private Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // PlayerAttack에서 이 함수를 호출해 값을 세팅해줍니다.
    public void Initialize(float damage, float speed, Vector2 direction)
    {
        this.damage = damage;
        this.speed = speed;
        this.direction = direction;

        // 투사체 이미지 좌우 반전 (왼쪽 쏠 때 뒤집기)
        if (direction.x < 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        // 날아가기 시작! (Rigidbody 속도 설정)
        if (rigid != null)
        {
            rigid.velocity = this.direction * this.speed;
        }

        // 3초 뒤에 자동으로 사라짐 (메모리 관리)
        Destroy(gameObject, 3f);
    }

    // 충돌 처리 (Trigger 사용 권장)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 적(Enemy)과 부딪혔을 때
        if (collision.CompareTag("Enemy") || (1 << collision.gameObject.layer & LayerMask.GetMask("Enemy")) != 0)
        {
            Debug.Log($"투사체 명중! 데미지: {damage}");

            // 실제 적 체력 깎기 (나중에 구현되면 주석 해제)
            // collision.GetComponent<Health>()?.TakeDamage(damage);

            // 투사체 삭제 (맞았으니 사라짐)
            Destroy(gameObject);
        }
    }
}