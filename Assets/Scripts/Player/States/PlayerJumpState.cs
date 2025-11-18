using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        player.animator.SetBool("isJumping", true);

        // 점프 힘 가하기 (즉발적인 힘)
        player.rigid.velocity = new Vector2(player.rigid.velocity.x, 0); // Y축 속도 초기화
        player.rigid.AddForce(Vector2.up * player.jumpPower, ForceMode2D.Impulse);

        // 바로 공중 상태로 전환해도 되지만, 보통 점프 프레임 확보를 위해 약간 딜레이를 주거나
        // 바로 AirState로 넘깁니다. 여기서는 바로 넘깁니다.
        stateMachine.ChangeState(player.AirState);
    }

    public override void Exit() { }
    public override void Update() { }
    public override void FixedUpdate() { }
}