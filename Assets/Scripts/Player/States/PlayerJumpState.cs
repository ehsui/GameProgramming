using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        // (추가) 점프 횟수 증가
        player.currentJumpCount++;

        // 애니메이션 초기화 (이단 점프 시 애니메이션을 처음부터 다시 재생하기 위함)
        player.animator?.Play("Jump", 0, 0f);
        player.animator?.SetBool("isJumping", true);

        // 점프 힘 가하기 (기존 속도 Y를 0으로 만들어야 이단 점프가 깔끔하게 됨)
        player.rigid.velocity = new Vector2(player.rigid.velocity.x, 0);
        player.rigid.AddForce(Vector2.up * player.jumpPower, ForceMode2D.Impulse);

        stateMachine.ChangeState(player.AirState);
    }

    public override void Exit() { }
    public override void Update() { }
    public override void FixedUpdate() { }
}