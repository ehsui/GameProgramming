using UnityEngine;

public class PlayerAirState : PlayerBaseState
{
    public PlayerAirState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        player.animator.SetBool("isJumping", true);
    }

    public override void Exit()
    {
        player.animator.SetBool("isJumping", false);
    }

    public override void Update()
    {
        // 바닥에 닿았고, 하강 중이라면(혹은 속도가 거의 0이라면) Idle로 복귀
        if (player.IsGrounded() && player.rigid.velocity.y <= 0.01f)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void FixedUpdate()
    {
        // 공중에서도 이동 제어 가능하게 하려면 여기서도 velocity 설정
        player.rigid.velocity = new Vector2(player.CurrentMovementInput.x * player.moveSpeed, player.rigid.velocity.y);
    }
}