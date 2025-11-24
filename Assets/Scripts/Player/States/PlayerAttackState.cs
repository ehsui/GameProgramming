// PlayerAttackState.cs

using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    private float timer;
    private float attackDelay;

    public PlayerAttackState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        // [삭제] player.rigid.velocity = Vector2.zero; 
        // 공격 중 멈추게 하던 범인입니다. 삭제하세요!

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
            // 공격이 끝나면 상황에 맞춰 돌아갈 상태 결정
            if (player.IsGrounded())
            {
                // 땅에 있고 입력이 있으면 걷기, 없으면 대기
                if (player.CurrentMovementInput.x != 0)
                    stateMachine.ChangeState(player.MoveState);
                else
                    stateMachine.ChangeState(player.IdleState);
            }
            else
            {
                // 공중이면 AirState로
                stateMachine.ChangeState(player.AirState);
            }
        }
    }

    public override void FixedUpdate()
    {
        // [추가] 공격 중에도 이동 가능하도록 이동 로직 추가!
        // (MoveState나 AirState에 있는 코드와 동일)
        player.rigid.velocity = new Vector2(player.CurrentMovementInput.x * player.moveSpeed, player.rigid.velocity.y);

        // (선택 사항) 공격 중에 뒤를 돌 수 있게 하려면 아래 코드 추가.
        // 보통 공격 중에는 방향 전환을 막기도 하지만, 원하시면 주석 해제하세요.
        /*
        if (player.CurrentMovementInput.x != 0)
        {
            player.spriteRenderer.flipX = player.CurrentMovementInput.x < 0;
        }
        */
    }
}