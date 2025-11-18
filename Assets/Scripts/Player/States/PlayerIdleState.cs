using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        player.rigid.velocity = Vector2.zero; // 미끄러짐 방지
        player.animator.SetBool("isWalking", false);
        player.animator.SetBool("isJumping", false);
    }

    public override void Exit() { }

    public override void Update()
    {
        // 이동 입력이 들어오면 MoveState로 전환
        if (player.CurrentMovementInput.x != 0)
        {
            stateMachine.ChangeState(player.MoveState);
        }

        // (추가) 떨어지기 시작하면 AirState로 전환 (절벽 등)
        if (player.rigid.velocity.y < -0.1f && !player.IsGrounded())
        {
            stateMachine.ChangeState(player.AirState);
        }
    }

    public override void FixedUpdate() { }
}