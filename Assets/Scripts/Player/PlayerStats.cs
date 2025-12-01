using UnityEngine;
using System; // Action을 쓰기 위해 필요

public class PlayerStats : MonoBehaviour
{
    [Header("Basic Stats")]
    public float maxHealth = 100f;
    public float maxBodhicitta = 100f; // 최대 보리심 (마나)
    public int level = 1;
    public int karma = 0;

    [Header("State")]
    public bool isInvincible = false; // 무적 상태 (스킬용)

    [Header("Inventory")]
    public int potionCount = 3; // 시작 시 포션 3개
    public int maxPotionCount = 5; // 최대 소지 개수
    public float potionHealAmount = 30f; // 포션 회복량

    // UI 갱신용 이벤트 (현재 개수)
    public event Action<int> OnPotionCountChanged;

    // 외부에서 읽기만 가능하도록 프로퍼티 설정
    public float CurrentHealth { get; private set; }
    public float CurrentBodhicitta { get; private set; }

    // --- [이벤트 정의] (UI 업데이트용) ---
    // float: 현재값, float: 최대값
    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnBodhicittaChanged;
    public event Action OnLevelChanged;
    public event Action OnDie; // 사망 시 발생

    private void Awake()
    {
        // 초기화
        CurrentHealth = maxHealth;
        CurrentBodhicitta = 0f; // 보리심은 0에서 시작 (기획)
    }

    private void Start()
    {
        // 시작하자마자 UI 갱신을 위해 이벤트 한 번 발생
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        OnBodhicittaChanged?.Invoke(CurrentBodhicitta, maxBodhicitta);

        // 포션 개수 UI 갱신
        OnPotionCountChanged?.Invoke(potionCount); // 시작 시 UI 갱신
    }

    private void Update()
    {
        // [테스트용 치트키] - 나중에 게임 완성되면 지우세요!

        // K키: 자해 (체력 -10) -> 사망 테스트용
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(10);
        }

        // H키: 회복 (체력 +10)
        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(10);
        }

        // L키: 마나 충전 (보리심 +10) -> 스킬 테스트용
        if (Input.GetKeyDown(KeyCode.L))
        {
            IncreaseBodhicitta(10);
        }
    }
    // --- [체력 관련 메서드] ---

    public void TakeDamage(float damage)
    {
        // 1. 무적 상태거나 이미 죽었으면 무시
        if (isInvincible || CurrentHealth <= 0) return;

        // 2. 체력 감소
        CurrentHealth -= damage;
        Debug.Log($"플레이어 피격! 남은 체력: {CurrentHealth}");

        // 3. 이벤트 알림 (UI야 체력바 줄여라!)
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        // 4. 사망 체크
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Die();
        }
    }

    public void Heal(float amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > maxHealth) CurrentHealth = maxHealth;

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        Debug.Log($"체력 회복: {CurrentHealth}");
    }

    private void Die()
    {
        Debug.Log("플레이어 사망!");
        OnDie?.Invoke(); // 컨트롤러에게 "나 죽었어"라고 알림
    }

    // --- [보리심(마나) 관련 메서드] ---

    public void IncreaseBodhicitta(float amount)
    {
        CurrentBodhicitta += amount;
        if (CurrentBodhicitta > maxBodhicitta) CurrentBodhicitta = maxBodhicitta;

        OnBodhicittaChanged?.Invoke(CurrentBodhicitta, maxBodhicitta);
        Debug.Log($"보리심 증가: {CurrentBodhicitta}");
    }

    public bool UseBodhicitta(float amount)
    {
        if (CurrentBodhicitta < amount)
        {
            Debug.Log("보리심 부족!");
            return false; // 사용 실패
        }

        CurrentBodhicitta -= amount;
        OnBodhicittaChanged?.Invoke(CurrentBodhicitta, maxBodhicitta);
        return true; // 사용 성공
    }
    public bool UsePotion()
    {
        // 1. 포션이 없으면 실패
        if (potionCount <= 0)
        {
            Debug.Log("포션이 없습니다!");
            return false;
        }

        // 2. 체력이 이미 꽉 찼으면 실패 (선택 사항: 아까우니까)
        if (CurrentHealth >= maxHealth)
        {
            Debug.Log("체력이 이미 가득 찼습니다.");
            return false;
        }

        // 3. 포션 사용 (개수 감소 & 회복)
        potionCount--;
        Heal(potionHealAmount);

        // UI 알림
        OnPotionCountChanged?.Invoke(potionCount);
        Debug.Log($"포션 사용! 남은 개수: {potionCount}");

        return true; // 사용 성공
    }

    // (나중에 아이템 먹어서 포션 채울 때 사용)
    public void AddPotion(int amount)
    {
        potionCount += amount;
        if (potionCount > maxPotionCount) potionCount = maxPotionCount;
        OnPotionCountChanged?.Invoke(potionCount);
    }

    public void AddKarma(int amount)
    {
        karma += amount;
        Debug.Log($"업보(Karma) 획득! 현재 업보: {karma}");

        // (나중에 업보 UI가 생긴다면 여기서 이벤트를 호출하면 됩니다)
        // OnKarmaChanged?.Invoke(karma); 
    }
}