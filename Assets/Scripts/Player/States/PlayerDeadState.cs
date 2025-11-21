using UnityEngine;

public class PlayerDeadState : PlayerBaseState
{
    public PlayerDeadState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        Debug.Log("DEAD STATE 진입");

        // 1. 모든 이동 정지
        player.rigid.velocity = Vector2.zero;
        player.rigid.simulated = false; // 물리 연산 중단 (선택 사항)

        // 2. 충돌 무시 (시체를 적이 밟고 지나갈 수 있게)
        // player.GetComponent<Collider2D>().enabled = false; 

        // 3. 사망 애니메이션 재생
        player.animator?.SetTrigger("die"); // 파라미터 필요

        // 4. 입력 차단은 PlayerController에서 상태 확인으로 자연스럽게 됨
    }

    public override void Exit()
    {
        // 부활 로직이 있다면 여기서 물리/충돌 다시 켜기
    }

    public override void Update() { } // 죽었으니 아무것도 안 함
    public override void FixedUpdate() { }
}