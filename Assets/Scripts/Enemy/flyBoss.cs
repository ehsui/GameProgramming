using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class flyBoss : MonoBehaviour
{
    [Header("Target & Status")]
    public Transform player;
    public float moveSpeed = 3f;
    public float stopDistance = 1.5f;

    [Header("Detection (감지 설정)")] 
    public float detectionRange = 15f; // 이 거리 안에 들어와야 움직이기 시작함
    private bool hasAggro = false;     // 플레이어를 발견했는지 여부

    [Header("Attack 1: Bite (물기)")]
    public GameObject biteHitbox;
    public float biteDelay = 0.3f;
    public float biteDuration = 0.2f;

    [Header("Attack 2: Spin (회전 돌진)")]
    public GameObject spinHitbox;
    public float spinDashSpeed = 8f;
    public float spinDuration = 1.5f;

    [Header("Cooldown")]
    public float attackCooldown = 3.0f;
    private float nextAttackTime = 0f;

    private Animator anim;
    private Rigidbody2D rb;
    private EnemyHealth health;
    private bool isAttacking = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<EnemyHealth>();
        
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (biteHitbox != null) biteHitbox.SetActive(false);
        if (spinHitbox != null) spinHitbox.SetActive(false);

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        // 1. 죽었거나 플레이어가 없으면 정지
        if (health.currentHealth <= 0 || player == null)
        {
            rb.velocity = Vector2.zero;
            if (biteHitbox != null) biteHitbox.SetActive(false);
            if (spinHitbox != null) spinHitbox.SetActive(false);
            return;
        }

        // 2. 공격 중이면 이동 로직 중단
        if (isAttacking) return;

        // 추가된 로직: 플레이어 감지 전에는 움직이지 않음 ★★★
        if (hasAggro == false)
        {
            float distToPlayer = Vector2.Distance(transform.position, player.position);

            if (distToPlayer <= detectionRange)
            {
                // 범위 안에 들어왔으니 추격 시작!
                hasAggro = true;
                Debug.Log("보스가 플레이어를 발견했습니다!"); 
            }
            else
            {
                // 아직 발견 못했으면 제자리 정지 후 리턴
                rb.velocity = Vector2.zero;
                return; 
            }
        }
        // --------------------------------------------------------

        // 3. 방향 전환 (감지된 이후에만 실행됨)
        FlipTowardsPlayer();

        float dist = Vector2.Distance(transform.position, player.position);

        // 4. 평소 이동 로직 (기존과 동일)
        if (dist > stopDistance)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.velocity = dir * moveSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        // 5. 공격 쿨타임 체크
        if (Time.time >= nextAttackTime && dist <= stopDistance + 3f)
        {
            StartCoroutine(AttackRoutine());
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void FlipTowardsPlayer()
    {
        if (isAttacking) return;

        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(15, 15, 15);
        else
            transform.localScale = new Vector3(-15, 15, 15);
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero; 

        int rand = Random.Range(0, 2);

        if (rand == 0) // Attack 1
        {
            anim.SetTrigger("Attack");
            yield return new WaitForSeconds(biteDelay);
            
            if (biteHitbox != null) biteHitbox.SetActive(true);
            yield return new WaitForSeconds(biteDuration);
            if (biteHitbox != null) biteHitbox.SetActive(false);
            
            yield return new WaitForSeconds(0.5f);
        }
        else // Attack 2
        {
            anim.SetTrigger("Attack2");
            yield return new WaitForSeconds(0.3f);

            if (spinHitbox != null) spinHitbox.SetActive(true);
            
            Vector2 dashDir = (player.position - transform.position).normalized;
            rb.velocity = dashDir * spinDashSpeed;

            yield return new WaitForSeconds(spinDuration);

            rb.velocity = Vector2.zero; 
            if (spinHitbox != null) spinHitbox.SetActive(false);
            
            yield return new WaitForSeconds(0.5f);
        }

        isAttacking = false;
    }

    // 에디터에서 감지 범위를 눈으로 보기 위한 기능 (선택사항)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}