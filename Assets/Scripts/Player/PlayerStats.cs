using UnityEngine;
using System; 
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    [Header("Basic Stats")]
    public float maxHealth = 100f;
    public float maxBodhicitta = 100f; 
    public int level = 1;
    public int karma = 0;

    [Header("State")]
    public bool isInvincible = false; 

    [Header("Inventory")]
    public int potionCount = 3; 
    public int maxPotionCount = 5; 
    public float potionHealAmount = 30f; 

    
    public event Action<int> OnPotionCountChanged;

    
    public float CurrentHealth { get; private set; }
    public float CurrentBodhicitta { get; private set; }

    
    
    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnBodhicittaChanged;
    
    public event Action<int> OnLevelChanged; 
    
    public event Action<int, int> OnKarmaChanged;

    public event Action OnDie;

    private void Awake()
    {
        
        CurrentHealth = maxHealth;
        CurrentBodhicitta = 0f; 
    }

    private void Start()
    {
        
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        OnBodhicittaChanged?.Invoke(CurrentBodhicitta, maxBodhicitta);

        
        OnPotionCountChanged?.Invoke(potionCount); 

        
        OnLevelChanged?.Invoke(level);
        OnKarmaChanged?.Invoke(karma, GetRequiredKarma());
    }

    private void Update()
    {
        

        
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(10);
        }

        
        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(10);
        }

        
        if (Input.GetKeyDown(KeyCode.L))
        {
            IncreaseBodhicitta(10);
        }
    }
    

    public void TakeDamage(float damage)
    {
        
        if (isInvincible || CurrentHealth <= 0) return;

        
        CurrentHealth -= damage;
        Debug.Log($"플레이어 피격! 남은 체력: {CurrentHealth}");

        
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        
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
        OnDie?.Invoke(); 

        
        CurrentHealth = maxHealth;  
        CurrentBodhicitta = 0f;     
        level = 1;
        karma = 0;
        potionCount = 3;

        
        SceneManager.LoadScene("Lobby"); 
    }

    

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
            return false; 
        }

        CurrentBodhicitta -= amount;
        OnBodhicittaChanged?.Invoke(CurrentBodhicitta, maxBodhicitta);
        return true; 
    }
    public bool UsePotion()
    {
        
        if (potionCount <= 0)
        {
            Debug.Log("포션이 없습니다!");
            return false;
        }

        
        if (CurrentHealth >= maxHealth)
        {
            Debug.Log("체력이 이미 가득 찼습니다.");
            return false;
        }

        
        potionCount--;
        Heal(potionHealAmount);

        
        OnPotionCountChanged?.Invoke(potionCount);
        Debug.Log($"포션 사용! 남은 개수: {potionCount}");

        return true; 
    }

    
    public void AddPotion(int amount)
    {
        potionCount += amount;
        if (potionCount > maxPotionCount) potionCount = maxPotionCount;
        OnPotionCountChanged?.Invoke(potionCount);
    }

    
    
    
    private int GetRequiredKarma()
    {
        return level * 100;
    }

    public void AddKarma(int amount)
    {
        karma += amount;
        Debug.Log($"업보(Karma) 획득: {amount} (현재: {karma}/{GetRequiredKarma()})");

        
        while (karma >= GetRequiredKarma())
        {
            karma -= GetRequiredKarma(); 
            LevelUp();
        }

        
        OnKarmaChanged?.Invoke(karma, GetRequiredKarma());
    }

    private void LevelUp()
    {
        level++;
        Debug.Log($"레벨업! 레벨 {level} 달성!");

        
        maxHealth += 20;
        maxBodhicitta += 10;

        
        CurrentHealth = maxHealth;
        CurrentBodhicitta = maxBodhicitta;

        
        OnLevelChanged?.Invoke(level);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        OnBodhicittaChanged?.Invoke(CurrentBodhicitta, maxBodhicitta);
    }
}