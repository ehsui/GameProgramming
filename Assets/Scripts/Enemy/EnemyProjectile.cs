using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 10f;
    public int damage = 10; 
    public float lifeTime = 3f; 
    
    public Vector3 defaultScale = new Vector3(5, 5, 5); 

    private Rigidbody2D rb;

    void Start()
    {
        transform.localScale = defaultScale;
        rb = GetComponent<Rigidbody2D>();
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            rb.velocity = direction * speed;
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어와 충돌했는지 확인
        if (collision.CompareTag("Player"))
        {
            // 플레이어의 PlayerStats 스크립트를 가져옴
            PlayerStats pStats = collision.GetComponent<PlayerStats>();

            // 스크립트가 잘 붙어있다면 데미지 주기
            if (pStats != null)
            {
                pStats.TakeDamage(damage); 
            }
            
            Debug.Log("플레이어 명중! 데미지: " + damage);
            Destroy(gameObject); // 총알 삭제
        }
        // 땅이나 벽에 닿으면 삭제
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) 
        {
            Destroy(gameObject);
        }
    }
}