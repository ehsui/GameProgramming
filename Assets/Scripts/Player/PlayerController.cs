using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }   

    [Header("References")]
    public PlayerInputReader inputReader;
    public Rigidbody2D rigid;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public PlayerAttack playerAttack;
    public PlayerStats stats; 

    [Header("Settings")]
    public float moveSpeed = 6f;
    public float jumpPower = 15f;
    public int maxJumpCount = 2; 
    public int currentJumpCount = 0; 

    [Header("Weapons")]
    public WeaponData[] mainWeapons; 
    public WeaponData[] subWeapons;  

    public WeaponData CurrentMainWeapon { get; private set; }
    public WeaponData CurrentSubWeapon { get; private set; }
    [Header("References")]
    public PlayerSkillCaster skillCaster;

    [Header("Effects")]
    public GameObject potionEffectPrefab; 

    
    public PlayerStateMachine StateMachine { get; private set; }

    
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerAirState AirState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }
    public PlayerDeadState DeadState { get; private set; }
    public PlayerSkillState SkillState { get; private set; }
    public bool IsFacingRight => !spriteRenderer.flipX;

    
    public Vector2 CurrentMovementInput { get; private set; }


    private void Awake()
    {
        
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
        
        StateMachine = new PlayerStateMachine();
        stats = GetComponent<PlayerStats>(); 
        skillCaster = GetComponent<PlayerSkillCaster>();
        
        IdleState = new PlayerIdleState(this, StateMachine);
        MoveState = new PlayerMoveState(this, StateMachine);
        JumpState = new PlayerJumpState(this, StateMachine);
        AirState = new PlayerAirState(this, StateMachine);
        AttackState = new PlayerAttackState(this, StateMachine);
        DeadState = new PlayerDeadState(this, StateMachine);
        SkillState = new PlayerSkillState(this, StateMachine); 

    }

    private void Start()
    {
        
        inputReader.OnMoveEvent += OnMove;
        inputReader.OnJumpEvent += OnJump;
        inputReader.OnAttackEvent += OnAttack;
            
        inputReader.OnMainWeaponSwitchEvent += SwapMainWeapon;
        inputReader.OnSubWeaponSwitchEvent += SwapSubWeapon;
        inputReader.OnSkillEvent += OnSkill;
        inputReader.OnPotionEvent += OnPotion; 

        SwapMainWeapon(0);
        SwapSubWeapon(0);
        
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

    
    private void HandleDie()
    {
        StateMachine.ChangeState(DeadState);
    }

    private void OnMove(Vector2 input)
    {
        CurrentMovementInput = input;
        
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

    
    
    public bool IsGrounded()
    {
        
        
        Collider2D collider = GetComponent<Collider2D>();

        
        Vector2 start = collider.bounds.center;

        
        
        float rayLength = collider.bounds.extents.y + 0.1f;

        
        Debug.DrawRay(start, Vector2.down * rayLength, Color.red);

        
        return Physics2D.Raycast(start, Vector2.down, rayLength, LayerMask.GetMask("Platform"));
    }

    
    private void OnAttack()
    {
        

        
        if (StateMachine.CurrentState == IdleState ||
            StateMachine.CurrentState == MoveState ||
            StateMachine.CurrentState == AirState ||
            StateMachine.CurrentState == JumpState)
        {
            
            StateMachine.ChangeState(AttackState);
        }
    }

    private void OnSkill()
    {
        
        
        if (StateMachine.CurrentState != DeadState)
        {
            StateMachine.ChangeState(SkillState);
        }
    }

    
    private void SwapMainWeapon(int index)
    {
        
        if (mainWeapons == null || mainWeapons.Length == 0 || index >= mainWeapons.Length) return;

        
        CurrentMainWeapon = mainWeapons[index];
        Debug.Log($"주무기 변경: {CurrentMainWeapon.weaponName}");

        
        
        if (CurrentMainWeapon.animatorController != null)
        {
            animator.runtimeAnimatorController = CurrentMainWeapon.animatorController;
        }

        
        else if (CurrentMainWeapon.weaponSprite != null)
        {
            
            
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
    
    private void OnPotion()
    {
        
        if (StateMachine.CurrentState == DeadState) return;

        
        if (stats.UsePotion())
        {
            PlayPotionEffect();
        }
    }

    private void PlayPotionEffect()
    {
        if (potionEffectPrefab != null)
        {
            
            Vector2 spawnPos = transform.position + Vector3.up * 1.0f; 
            GameObject effect = Instantiate(potionEffectPrefab, spawnPos, Quaternion.identity);

            
            effect.transform.SetParent(this.transform);

            
        }
    }
}