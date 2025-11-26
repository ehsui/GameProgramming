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
        player.animator?.SetTrigger("skill");

        // ✨ [수정 1] 버프(Buff) 류 스킬은 시전 즉시 상태 탈출! (이동 가능하게)
        if (currentSkill.skillType == SkillType.Buff)
        {
            // 바로 Idle(혹은 이동 중이었다면 Move) 상태로 복귀
            if (player.CurrentMovementInput.x != 0)
                stateMachine.ChangeState(player.MoveState);
            else
                stateMachine.ChangeState(player.IdleState);
            return; // 여기서 Enter 종료
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
        // ✨ [수정 2] 이동을 강제로 막는 로직 완화

        // 돌진(Dash)은 Caster가 속도를 제어하므로 건드리지 않음
        if (currentSkill != null && currentSkill.skillType == SkillType.Dash) return;

        // 나머지 스킬(투사체 발사 등) 중에서도 '이동 사격'을 허용하고 싶다면?
        // 아래 코드를 지우거나 조건을 추가하면 됩니다.
        // 일단은 "시전 중 이동 불가"를 유지하되, 버프는 이미 나갔으므로 영향받지 않음.
        player.rigid.velocity = Vector2.zero;
    }
}