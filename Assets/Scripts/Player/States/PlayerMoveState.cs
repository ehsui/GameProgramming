using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    public PlayerMoveState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        player.animator.SetBool("isWalking", true);
    }

    public override void Exit()
    {
        player.animator.SetBool("isWalking", false);
    }

    public override void Update()
    {
        // 입력이 멈추면 IdleState로 전환
        if (player.CurrentMovementInput.x == 0)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void FixedUpdate()
    {
        // 실제 이동 로직
        player.rigid.velocity = new Vector2(player.CurrentMovementInput.x * player.moveSpeed, player.rigid.velocity.y);
    }
}