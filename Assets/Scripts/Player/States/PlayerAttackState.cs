

using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    private float timer;
    private float attackDelay;

    public PlayerAttackState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        
        

        player.animator?.SetTrigger("attack");

        WeaponData weapon = player.CurrentMainWeapon;
        attackDelay = weapon != null ? weapon.attackRate : 0.5f;
        timer = 0f;

        player.playerAttack?.PerformAttack(weapon);
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        timer += Time.deltaTime;

        if (timer >= attackDelay)
        {
            
            if (player.IsGrounded())
            {
                
                if (player.CurrentMovementInput.x != 0)
                    stateMachine.ChangeState(player.MoveState);
                else
                    stateMachine.ChangeState(player.IdleState);
            }
            else
            {
                
                stateMachine.ChangeState(player.AirState);
            }
        }
    }

    public override void FixedUpdate()
    {
        
        
        player.rigid.velocity = new Vector2(player.CurrentMovementInput.x * player.moveSpeed, player.rigid.velocity.y);

        /*
        if (player.CurrentMovementInput.x != 0)
        {
            player.spriteRenderer.flipX = player.CurrentMovementInput.x < 0;
        }
        */
    }
}