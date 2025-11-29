using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }   //싱글톤

    [Header("References")]
    public PlayerInputReader inputReader;
    public Rigidbody2D rigid;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public PlayerAttack playerAttack;
    public PlayerStats stats; // 스탯 컴포넌트 참조

    [Header("Settings")]
    public float moveSpeed = 6f;
    public float jumpPower = 15f;
    public int maxJumpCount = 2; // 최대 점프 횟수
    public int currentJumpCount = 0; // 현재 점프 횟수

    [Header("Weapons")]
    public WeaponData[] mainWeapons; // 인스펙터에서 할당할 무기 리스트 (3개)
    public WeaponData[] subWeapons;  // 인스펙터에서 할당할 보조무기 리스트 (3개)

    public WeaponData CurrentMainWeapon { get; private set; }
    public WeaponData CurrentSubWeapon { get; private set; }
    [Header("References")]
    public PlayerSkillCaster skillCaster;

    // 상태 머신
    public PlayerStateMachine StateMachine { get; private set; }

    // 상태 인스턴스들 (미리 만들어두고 재사용)
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerAirState AirState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }
    public PlayerDeadState DeadState { get; private set; }
    public PlayerSkillState SkillState { get; private set; }
    public bool IsFacingRight => !spriteRenderer.flipX;

    // 입력값 저장용 변수
    public Vector2 CurrentMovementInput { get; private set; }


    private void Awake()
    {
        // --- [싱글톤 패턴 적용] --- 다른 씬에 플레이어 추가하면 안됩니당
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // 상태 머신 생성
        StateMachine = new PlayerStateMachine();
        stats = GetComponent<PlayerStats>(); // 같은 오브젝트에 있다고 가정
        skillCaster = GetComponent<PlayerSkillCaster>();
        // 상태 인스턴스 생성 (this를 넘겨줌)
        IdleState = new PlayerIdleState(this, StateMachine);
        MoveState = new PlayerMoveState(this, StateMachine);
        JumpState = new PlayerJumpState(this, StateMachine);
        AirState = new PlayerAirState(this, StateMachine);
        AttackState = new PlayerAttackState(this, StateMachine);
        DeadState = new PlayerDeadState(this, StateMachine);
        SkillState = new PlayerSkillState(this, StateMachine); // 상태 생성

    }

    private void Start()
    {
        // 이벤트 구독
        inputReader.OnMoveEvent += OnMove;
        inputReader.OnJumpEvent += OnJump;
        inputReader.OnAttackEvent += OnAttack;
            
        inputReader.OnMainWeaponSwitchEvent += SwapMainWeapon;
        inputReader.OnSubWeaponSwitchEvent += SwapSubWeapon;
        inputReader.OnSkillEvent += OnSkill;

        SwapMainWeapon(0);
        SwapSubWeapon(0);
        // 초기 상태 설정
        StateMachine.Initialize(IdleState);
        stats.OnDie += HandleDie;
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
    private void HandleDie()
    {
        StateMachine.ChangeState(DeadState);
    }

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
        if (IsGrounded() || currentJumpCount < maxJumpCount)
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

    // 공격 입력 핸들러 (추가)
    private void OnAttack()
    {
        // [수정 전] if (StateMachine.CurrentState == IdleState || StateMachine.CurrentState == MoveState)

        // [수정 후] 공중(Air, Jump) 상태에서도 공격 허용
        if (StateMachine.CurrentState == IdleState ||
            StateMachine.CurrentState == MoveState ||
            StateMachine.CurrentState == AirState ||
            StateMachine.CurrentState == JumpState)
        {
            // 공격 쿨타임 체크 등은 여기서 하거나 State 내부에서 처리
            StateMachine.ChangeState(AttackState);
        }
    }

    private void OnSkill()
    {
        // 공격 중, 이동 중, 대기 중일 때 스킬 사용 가능
        // (사망 상태나 피격 경직 중에는 불가능)
        if (StateMachine.CurrentState != DeadState)
        {
            StateMachine.ChangeState(SkillState);
        }
    }

    // 무기 교체 로직 (추가)
    private void SwapMainWeapon(int index)
    {
        // [방어 코드]
        if (mainWeapons == null || mainWeapons.Length == 0 || index >= mainWeapons.Length) return;

        // 1. 데이터 교체
        CurrentMainWeapon = mainWeapons[index];
        Debug.Log($"주무기 변경: {CurrentMainWeapon.weaponName}");

        // 2. 애니메이터 컨트롤러 교체 (핵심!) ✨
        // 무기 데이터에 AnimatorController가 들어있다면 교체해줍니다.
        if (CurrentMainWeapon.animatorController != null)
        {
            animator.runtimeAnimatorController = CurrentMainWeapon.animatorController;
        }

        // 3. (만약 애니메이션 없이 이미지만 바꾼다면)
        else if (CurrentMainWeapon.weaponSprite != null)
        {
            // 애니메이터가 있으면 스프라이트 교체를 덮어쓰기 때문에, 
            // 애니메이터가 없을 때만 스프라이트를 직접 교체합니다.
            spriteRenderer.sprite = CurrentMainWeapon.weaponSprite;
        }
    }

    private void SwapSubWeapon(int index)
    {
        if (index >= 0 && index < subWeapons.Length)
        {
            CurrentSubWeapon = subWeapons[index];
            Debug.Log($"보조무기 변경: {CurrentSubWeapon.weaponName}");
        }
    }
}