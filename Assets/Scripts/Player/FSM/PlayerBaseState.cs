using UnityEngine;

public abstract class PlayerBaseState
{
    protected PlayerController player;
    protected PlayerStateMachine stateMachine;

    // 생성자: 컨트롤러와 상태머신을 받아서 기억함
    public PlayerBaseState(PlayerController player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public abstract void Enter();       // 상태 진입 시 실행
    public abstract void Exit();        // 상태 종료 시 실행
    public abstract void Update();      // 매 프레임 실행 (로직)
    public abstract void FixedUpdate(); // 물리 업데이트 실행
}