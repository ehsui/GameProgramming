using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float damage;
    private float speed;
    private Vector2 direction;
    private Rigidbody2D rigid;

    [Header("Explosion Settings")]
    public bool isExplosive = false;      
    public float explosionRadius = 2.0f;  
    public GameObject explosionEffect;    

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Initialize(float damage, float speed, Vector2 direction)
    {
        this.damage = damage;
        this.speed = speed;
        this.direction = direction;

        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        
        if (direction.x < 0)
        {
            Vector3 scale = transform.localScale;
            scale.y = -Mathf.Abs(scale.y); 
            transform.localScale = scale;
        }

        if (rigid != null)
        {
            rigid.velocity = this.direction * this.speed;
        }

        Destroy(gameObject, 5f); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        bool isEnemy = collision.CompareTag("Enemy") || (1 << collision.gameObject.layer & LayerMask.GetMask("Enemy")) != 0;
        bool isPlatform = (1 << collision.gameObject.layer & LayerMask.GetMask("Platform")) != 0;

        if (isEnemy || isPlatform)
        {
            if (isExplosive)
            {
                Explode(); 
            }
            else if (isEnemy)
            {
                
                Health target = collision.GetComponent<Health>();
                if (target != null) target.TakeDamage(this.damage);
            }

            
            Destroy(gameObject);
        }
    }

    
    private void Explode()
    {
        
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius, LayerMask.GetMask("Enemy"));

        foreach (var enemy in enemies)
        {
            enemy.GetComponent<Health>()?.TakeDamage(this.damage);
            Debug.Log($"[폭발] {enemy.name} 휘말림! 데미지: {damage}");
        }

        
        
    }
}