using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 10f;
    public int damage = 1;
    public float lifeTime = 3f; // 3초 뒤 자동 삭제
    
    // 🔥 크기 설정 변수 추가 (기본값 5, 5, 5)
    public Vector3 defaultScale = new Vector3(5, 5, 5); 

    private Rigidbody2D rb;

    void Start()
    {
        // 1. 시작하자마자 크기 적용
        transform.localScale = defaultScale;

        rb = GetComponent<Rigidbody2D>();
        
        // 생성되자마자 플레이어 방향으로 날아가게 설정
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            rb.velocity = direction * speed;
            
            // 총알이 날아가는 방향을 바라보게 회전
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        
        // 일정 시간 후 삭제 (메모리 관리)
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어와 충돌했는지 확인
        if (collision.CompareTag("Player"))
        {
            // 여기에 플레이어 HP 깎는 코드 추가 
            // 예: collision.GetComponent<PlayerHealth>().TakeDamage(damage);
            Debug.Log("플레이어 명중!"); 
            Destroy(gameObject); // 총알 삭제
        }
        // 땅이나 벽에 닿으면 삭제 (Layer 확인 필요)
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) 
        {
            Destroy(gameObject);
        }
    }
}