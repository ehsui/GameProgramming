using UnityEngine;

public class PlayerSkillState : PlayerBaseState
{
    private float timer;
    private SkillData currentSkill;

    public PlayerSkillState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        currentSkill = player.skillCaster.GetCurrentSkill();

        if (currentSkill == null)
        {
            stateMachine.ChangeState(player.IdleState);
            return;
        }

        player.skillCaster.CastSkill(currentSkill);

        
        if (currentSkill.skillType == SkillType.Buff ||
            currentSkill.skillType == SkillType.Stealth ||
            currentSkill.skillType == SkillType.Heal)
        {
            if (player.CurrentMovementInput.x != 0)
                stateMachine.ChangeState(player.MoveState);
            else
                stateMachine.ChangeState(player.IdleState);
            return;
        }

        timer = 0f;
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        if (currentSkill == null) return;

        timer += Time.deltaTime;

        
        float duration = currentSkill.duration > 0 ? currentSkill.duration : 0.5f;

        if (timer >= duration)
        {
            if (player.IsGrounded())
                stateMachine.ChangeState(player.IdleState);
            else
                stateMachine.ChangeState(player.AirState);
        }
    }

    public override void FixedUpdate()
    {
        
        if (currentSkill != null && currentSkill.skillType == SkillType.Dash) return;

        
        
        

        player.rigid.velocity = new Vector2(player.CurrentMovementInput.x * player.moveSpeed, player.rigid.velocity.y);
    }
}