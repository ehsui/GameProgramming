using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 필수

public class PlayerHUD : MonoBehaviour
{
    // === UI Image 및 Text 요소 참조 ===
    [Header("UI Image References")]
    public Image healthBarFill;
    public Image bodhicittaBarFill;

    [Header("UI Text References")]
    public TextMeshProUGUI levelText; // 레벨 숫자 표시 (예: "Lv.1")
    public TextMeshProUGUI karmaText; // 업보 숫자 표시 (예: "150 / 200")

    [Header("Potion UI References")]
    public Image[] potionIcons;
    public Sprite emptyPotionSprite;
    public Sprite filledPotionSprite;

    // === 플레이어 스탯 참조 ===
    private PlayerStats playerStats;

    void Start()
    {
        // 1. 태그로 플레이어 찾기
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        
        if (playerObject != null)
        {
            playerStats = playerObject.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                // 3. 이벤트 구독
                playerStats.OnHealthChanged += UpdateHealthBar;
                playerStats.OnBodhicittaChanged += UpdateBodhicittaBar;
                playerStats.OnPotionCountChanged += UpdatePotionIcons;
                
                // 레벨, 업보 이벤트 구독
                playerStats.OnLevelChanged += UpdateLevelText;
                playerStats.OnKarmaChanged += UpdateKarmaText;
                
                // 4. 초기값 갱신
                UpdateHealthBar(playerStats.CurrentHealth, playerStats.maxHealth);
                UpdateBodhicittaBar(playerStats.CurrentBodhicitta, playerStats.maxBodhicitta);
                UpdatePotionIcons(playerStats.potionCount);

                // 시작 시 텍스트 갱신
                // (필요 경험치는 PlayerStats 로직이 level * 100 이므로 여기서 계산해서 초기화)
                UpdateLevelText(playerStats.level);
                UpdateKarmaText(playerStats.karma, playerStats.level * 100);
            }
        }
        else
        {
            Debug.LogError("HUDManager: Player 태그를 찾을 수 없습니다.");
        }
    }

    // 오브젝트가 사라질 때 연결 해제
    void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnHealthChanged -= UpdateHealthBar;
            playerStats.OnBodhicittaChanged -= UpdateBodhicittaBar;
            playerStats.OnPotionCountChanged -= UpdatePotionIcons;
            playerStats.OnLevelChanged -= UpdateLevelText;
            playerStats.OnKarmaChanged -= UpdateKarmaText;
        }
    }

    // --- [UI 업데이트 함수들] ---

    public void UpdateHealthBar(float current, float max)
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = current / max;
    }

    public void UpdateBodhicittaBar(float current, float max)
    {
        if (bodhicittaBarFill != null)
            bodhicittaBarFill.fillAmount = current / max;
    }

    public void UpdatePotionIcons(int count)
    {
        for (int i = 0; i < potionIcons.Length; i++)
        {        
            if (potionIcons[i] != null)
                potionIcons[i].sprite = (i < count) ? filledPotionSprite : emptyPotionSprite;
        }
    }

    // 레벨 텍스트 업데이트
    public void UpdateLevelText(int level)
    {
        if (levelText != null)
        {
            levelText.text = $"Lv.{level}"; // 출력 예시: Lv.1
        }
    }

    // 업보(경험치) 텍스트 업데이트
    public void UpdateKarmaText(int current, int max)
    {
        if (karmaText != null)
        {
            karmaText.text = $"{current} / {max}"; // 출력 예시: 50 / 100
        }
    }
}