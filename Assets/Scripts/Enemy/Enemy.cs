using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Status")]
    public float moveSpeed = 3f;
    public float chaseRange = 5f;  // 플레이어 감지 범위
    public float stopDistance = 1f; // 플레이어 바로 앞에서 멈추는 거리

    [Header("References")]
    public Transform player;
    public Transform[] patrolPoints; // 순찰할 지점들

    private int currentPatrolIndex = 0;
    private bool isChasing = false;

    private void Start()
    {
        // 플레이어가 할당되지 않았다면 태그로 찾기 (선택 사항)
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
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
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
        LookAtTarget(targetPoint.position);

        // 순찰 지점에 거의 도착했으면 다음 지점으로 변경
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            currentPatrolIndex++;
            if (currentPatrolIndex >= patrolPoints.Length)
            {
                currentPatrolIndex = 0; // 다시 첫 지점으로 순환
            }
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
    }
}