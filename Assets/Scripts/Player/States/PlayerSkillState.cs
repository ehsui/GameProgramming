using UnityEngine;

public class PlayerSkillState : PlayerBaseState
{
    private float timer;
    private SkillData currentSkill;

    public PlayerSkillState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        player.rigid.velocity = Vector2.zero; // 스킬 시전 중 이동 정지 (기획에 따라 변경 가능)

        // 1. 시전할 스킬 가져오기
        currentSkill = player.skillCaster.GetCurrentSkill();

        if (currentSkill == null)
        {
            // 스킬을 못 찾았거나 마나가 부족하면 즉시 Idle로 복귀
            stateMachine.ChangeState(player.IdleState);
            return;
        }

        // 2. 마나 소모 (Stats에 요청)
        player.stats.UseBodhicitta(currentSkill.bodhicittaCost);

        // 3. 애니메이션 재생 (아직 없다면 트리거만)
        player.animator?.SetTrigger("skill");
        // 혹은 skillData에 있는 애니메이션 이름을 쓸 수도 있음

        // 4. 로그 출력 (임시)
        Debug.Log($"[스킬 발동] {currentSkill.skillName} !!");

        timer = 0f;
    }

    public override void Exit()
    {
        // 스킬 종료 처리
    }

    public override void Update()
    {
        if (currentSkill == null) return;

        timer += Time.deltaTime;

        // 지속 시간이 끝나면 Idle로 복귀
        // (스킬 데이터에 duration이 0이면 기본 0.5초로 처리)
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
        // 스킬 중 이동 불가라면 비워두고, 가능하다면 MoveState 로직 복사
    }
}