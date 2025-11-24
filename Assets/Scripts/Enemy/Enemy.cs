using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Status")]
    public float moveSpeed = 3f;
    public float chaseRange = 10f;  // 플레이어 감지 범위
    public float stopDistance = 3f; // 플레이어 바로 앞에서 멈추는 거리

    [Header("Attack Settings")]
    public float attackRate = 2f; //  예: 2초당 1회 공격
    
    [Header("References")]
    public Transform player;
    public Transform[] patrolPoints; // 순찰할 지점들
    public Animator anim;           // Animator 컴포넌트 참조

    private int currentPatrolIndex = 0;
    private bool isChasing = false;
    private float nextAttackTime = 0f; // 다음 공격 가능 시간

    private void Start()
    {
        // 플레이어가 할당되지 않았다면 태그로 찾기
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        // Animator 컴포넌트 가져오기
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 1. 상태 결정 (추격 vs 순찰)
        if (distanceToPlayer < chaseRange)
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
        }

        // 2. 행동 실행
        if (isChasing)
        {
            ChasePlayer(distanceToPlayer);
        }
        else
        {
            Patrol();
        }
    }

    void ChasePlayer(float distance)
    {
        // 공격 범위 밖이라면 이동
        if (distance > stopDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            LookAtTarget(player.position);
            SetAnimationState(true);    // 걷기 애니메이션 실행
        }
        else
        {
            // 멈춰서 플레이어를 바라봄 (공격 실행)
            LookAtTarget(player.position);
            // 걷기 애니메이션을 끄고 공격 로직 실행
            SetAnimationState(false); 
            AttackPlayer(); // 공격 메서드 호출
        }
    }

    // 공격 로직 및 애니메이션
    void AttackPlayer()
    {
        // 공격 주기가 되었는지 확인
        if (Time.time >= nextAttackTime)
        {
            // 애니메이터에 Attack 트리거를 실행
            if (anim != null)
            {
                anim.SetTrigger("Attack"); // Animator에 "Attack" Trigger 필요
            }
            
            // 실제 데미지 로직은 여기에 구현  위치

            // 다음 공격 시간 설정
            nextAttackTime = Time.time + attackRate;
        }
    }
    
    void Patrol()
    {
        if (patrolPoints.Length == 0)
        {
            SetAnimationState(false); 
            return;
        }

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
        LookAtTarget(targetPoint.position);
        SetAnimationState(true); 

        // 순찰 지점에 거의 도착했으면 다음 지점으로 즉시 변경
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            currentPatrolIndex++;
            if (currentPatrolIndex >= patrolPoints.Length)
            {
                currentPatrolIndex = 0; // 다시 첫 지점으로 순환
            }
        }
    }
    
    // 애니메이션 상태를 설정하는 메서드 (IsWalking)
    void SetAnimationState(bool isWalking)
    {
        if (anim != null)
        {
            // 공격 애니메이션이 재생 중일 때는 걷기/대기 상태를 변경하지 않도록 
            // 더 복잡한 상태 관리가 필요할 수 있으나, 여기서는 단순화하여 구현합니다.
            anim.SetBool("IsWalking", isWalking);
        }
    }

    // 적이 이동하는 방향을 바라보게 함
    void LookAtTarget(Vector3 target)
    {
        if (target.x > transform.position.x)
            transform.localScale = new Vector3(-6, 6, 6); // 오른쪽 보기
        else if (target.x < transform.position.x)
            transform.localScale = new Vector3(6, 6, 6); // 왼쪽 보기
    }

    // 에디터에서 감지 범위를 시각적으로 보여줌
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        
        // 공격 범위 시각화
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}