using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public PlayerInputReader inputReader;
    public Rigidbody2D rigid;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("Settings")]
    public float moveSpeed = 6f;
    public float jumpPower = 15f;

    // 상태 머신
    public PlayerStateMachine StateMachine { get; private set; }

    // 상태 인스턴스들 (미리 만들어두고 재사용)
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerAirState AirState { get; private set; }

    // 입력값 저장용 변수
    public Vector2 CurrentMovementInput { get; private set; }

    private void Awake()
    {
        // 1. 상태 머신 생성
        StateMachine = new PlayerStateMachine();

        // 2. 상태 인스턴스 생성 (this를 넘겨줌)
        IdleState = new PlayerIdleState(this, StateMachine);
        MoveState = new PlayerMoveState(this, StateMachine);
        JumpState = new PlayerJumpState(this, StateMachine);
        AirState = new PlayerAirState(this, StateMachine);
    }

    private void Start()
    {
        // 3. 이벤트 구독 (InputReader -> Controller)
        inputReader.OnMoveEvent += OnMove;
        inputReader.OnJumpEvent += OnJump;

        // 4. 초기 상태 설정
        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        StateMachine.CurrentState.Update();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.FixedUpdate();
    }

    // --- 입력 이벤트 핸들러 ---
    private void OnMove(Vector2 input)
    {
        CurrentMovementInput = input;
        // 방향 전환 (Flip)
        if (input.x != 0)
        {
            spriteRenderer.flipX = input.x < 0;
        }
    }

    private void OnJump()
    {
        // 점프는 특정 상태(Idle, Move)에서만 가능하도록 상태 내부에서 처리할 수도 있지만,
        // 일단 지금은 현재 상태가 '점프 가능'한지 체크하지 않고 바로 상태 전환을 시도하거나,
        // 상태에게 "점프 키 눌림"을 알려주는 방식을 씁니다.
        // 여기서는 간단히: "바닥에 있을 때만 점프 가능"
        if (IsGrounded())
        {
            StateMachine.ChangeState(JumpState);
        }
    }

    // --- 헬퍼 메서드 ---
    // 바닥 체크 (간단한 Raycast)
    public bool IsGrounded()
    {
        // 1. 플레이어의 발바닥 위치 찾기 (Collider가 필요합니다)
        // BoxCollider2D나 CapsuleCollider2D를 가져옵니다.
        Collider2D collider = GetComponent<Collider2D>();

        // 레이 발사 위치: 콜라이더의 중심
        Vector2 start = collider.bounds.center;

        // 레이 길이: (콜라이더 높이의 절반) + (여유분 0.1f)
        // 이렇게 하면 플레이어 크기가 커져도 자동으로 맞게 계산됩니다.
        float rayLength = collider.bounds.extents.y + 0.1f;

        // 디버그용: 씬(Scene) 화면에 빨간 선을 그립니다.
        Debug.DrawRay(start, Vector2.down * rayLength, Color.red);

        // 실제 감지
        return Physics2D.Raycast(start, Vector2.down, rayLength, LayerMask.GetMask("Platform"));
    }
}