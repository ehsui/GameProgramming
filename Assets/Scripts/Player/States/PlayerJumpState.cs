using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        
        player.currentJumpCount++;

        
        player.animator?.Play("Jump", 0, 0f);
        player.animator?.SetBool("isJumping", true);

        
        player.rigid.velocity = new Vector2(player.rigid.velocity.x, 0);
        player.rigid.AddForce(Vector2.up * player.jumpPower, ForceMode2D.Impulse);

        stateMachine.ChangeState(player.AirState);
    }

    public override void Exit() { }
    public override void Update() { }
    public override void FixedUpdate() { }
}