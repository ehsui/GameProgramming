using UnityEngine;
using System.Collections; // 1. 코루틴을 쓰기 위해 반드시 필요!

public class Enemy2 : MonoBehaviour
{
    [Header("Status")]
    public float moveSpeed = 3f;
    public float chaseRange = 10f;
    public float stopDistance = 5f;

    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float attackRate = 2f;
    public float attackDelay = 0.01f; // 2. 공격 동작 후 총알 발사까지의 대기 시간
    
    [Header("References")]
    public Transform player;
    public Transform[] patrolPoints;
    public Animator anim;

    private int currentPatrolIndex = 0;
    private bool isChasing = false;
    private float nextAttackTime = 0f;
    
    private Vector3 defaultScale; 

    private void Start()
    {
        defaultScale = transform.localScale;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        if (anim == null) anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // 플레이어가 없어도 순찰은 돌도록 수정 (이전 피드백 반영)
        if (player == null)
        {
            Patrol();
            return;
        }

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
            // 플레이어의 X좌표 + 적(나)의 현재 Y좌표를 합쳐서 새로운 목표지점을 만듦
            Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);

            // player.position 대신 targetPosition으로 이동
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            
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

    void AttackPlayer()
    {
        if (Time.time >= nextAttackTime)
        {
            // 3. 애니메이션은 즉시 실행
            if (anim != null) anim.SetTrigger("Attack");

            // 4. 총알 발사는 1초 뒤에 실행 (코루틴 시작)
            StartCoroutine(ShootDelay());

            nextAttackTime = Time.time + attackRate;
        }
    }

    // 5. 딜레이를 주는 코루틴 함수
    IEnumerator ShootDelay()
    {
        // 설정한 시간(1초)만큼 대기
        yield return new WaitForSeconds(attackDelay);

        // 대기가 끝난 후 총알 생성
        if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("투사체 프리팹이나 발사 위치(FirePoint)가 설정되지 않았습니다!");
        }
    }
    
    void Patrol()
    {
        // 순찰 지점이 아예 없거나(null), 비어있다면(Length == 0)
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            SetAnimationState(false); // 걷는 모션 끄기 (Idle)
            return; // 아래의 이동 코드를 실행하지 않고 함수 종료 -> 제자리 멈춤
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
            transform.localScale = new Vector3(Mathf.Abs(defaultScale.x), defaultScale.y, defaultScale.z); 
        else if (target.x < transform.position.x)
            transform.localScale = new Vector3(-Mathf.Abs(defaultScale.x), defaultScale.y, defaultScale.z); 
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}