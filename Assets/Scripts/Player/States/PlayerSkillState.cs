using UnityEngine;

public class PlayerSkillState : PlayerBaseState
{
    private float timer;
    private SkillData currentSkill;

    public PlayerSkillState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        // 1. 스킬 데이터 가져오기
        currentSkill = player.skillCaster.GetCurrentSkill();

        if (currentSkill == null)
        {
            stateMachine.ChangeState(player.IdleState);
            return;
        }

        // 2. 스킬 실행 (Caster에게 위임)
        player.skillCaster.CastSkill(currentSkill);

        // 3. 애니메이션 트리거
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

        // 버프는 위에서 이미 나갔으므로, 여기는 '시전 동작이 필요한 스킬'만 남음
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
        // 1. 돌진(Dash) 스킬은 Caster가 직접 물리 제어를 하므로 건드리지 않음
        if (currentSkill != null && currentSkill.skillType == SkillType.Dash) return;

        // 2. [수정] 이동 로직 적용 (AttackState와 동일하게 변경)
        // X축: 입력받은 대로 이동 (공중 이동 가능)
        // Y축: 현재 리지드바디의 속도 유지 (중력 적용!) ✨ 핵심

        player.rigid.velocity = new Vector2(player.CurrentMovementInput.x * player.moveSpeed, player.rigid.velocity.y);
    }
}