using UnityEngine;
using System.Collections; // 코루틴

public class Enemy : MonoBehaviour
{
    [Header("Status")]
    public float moveSpeed = 3f;
    public float chaseRange = 10f;
    public float stopDistance = 3f;

    [Header("Attack Settings")]
    public float attackRate = 2f;
    public int damage = 10;
    public float damageDelay = 0.5f; // 공격 동작 시작 후 실제 타격까지 걸리는 시간 (애니메이션에 맞춰 조절)

    [Header("References")]
    public Transform player;
    public Transform[] patrolPoints;
    public Animator anim;

    private int currentPatrolIndex = 0;
    private bool isChasing = false;
    private float nextAttackTime = 0f;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        if (anim == null) anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < chaseRange)
            isChasing = true;
        else
            isChasing = false;

        if (isChasing)
            ChasePlayer(distanceToPlayer);
        else
            Patrol();
    }

    void ChasePlayer(float distance)
    {
        if (distance > stopDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            LookAtTarget(player.position);
            SetAnimationState(true);
        }
        else
        {
            LookAtTarget(player.position);
            SetAnimationState(false);
            AttackPlayer();
        }
    }

    // 공격 로직 (코루틴 시작)
    void AttackPlayer()
    {
        if (Time.time >= nextAttackTime)
        {
            // 1. 애니메이션 먼저 실행 (칼 휘두르기 시작)
            if (anim != null) anim.SetTrigger("Attack");

            // 2. 실제 타격 판정은 잠시 후에 (코루틴 호출)
            StartCoroutine(ApplyDamageWithDelay());

            // 3. 쿨타임 적용
            nextAttackTime = Time.time + attackRate;
        }
    }

    // 딜레이 후 데미지 주는 코루틴
    IEnumerator ApplyDamageWithDelay()
    {
        // 설정한 시간만큼 대기
        yield return new WaitForSeconds(damageDelay);

        // 대기가 끝난 시점에 플레이어가 여전히 있는지 확인!
        if (player != null)
        {
            // 플레이어와의 거리를 다시 계산
            float currentDistance = Vector2.Distance(transform.position, player.position);

            // 공격 범위(stopDistance) + 약간의 여유분(0.5f) 안에 있어야 맞음
            if (currentDistance <= stopDistance + 0.5f)
            {
                PlayerStats pStats = player.GetComponent<PlayerStats>();
                if (pStats != null)
                {
                    pStats.TakeDamage(damage);
                    Debug.Log("Attack!");
                }
            }
            else
            {
                Debug.Log("Attadck failed: Player out of range.");
            }
        }
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            SetAnimationState(false);
            return;
        }

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
        LookAtTarget(targetPoint.position);
        SetAnimationState(true); 

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            currentPatrolIndex++;
            if (currentPatrolIndex >= patrolPoints.Length)
                currentPatrolIndex = 0;
        }
    }
    
    void SetAnimationState(bool isWalking)
    {
        if (anim != null) anim.SetBool("IsWalking", isWalking);
    }

    void LookAtTarget(Vector3 target)
    {
        if (target.x > transform.position.x)
            transform.localScale = new Vector3(-6, 6, 6); 
        else if (target.x < transform.position.x)
            transform.localScale = new Vector3(6, 6, 6); 
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}