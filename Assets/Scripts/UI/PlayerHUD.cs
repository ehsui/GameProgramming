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

    // [추가됨] 보조 무기 UI 참조
    [Header("Weapon UI References")]
    public Image subWeaponImage;        // 현재 보조 무기를 보여줄 UI 이미지
    public Sprite[] subWeaponSprites;   // 보조 무기 스프라이트 배열 (0, 1, 2 순서)

    // === 플레이어 스탯 참조 ===
    private PlayerStats playerStats;
    private PlayerInputReader inputReader;

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

            // [추가됨] 3. PlayerInputReader 연결 및 구독
            inputReader = playerObject.GetComponent<PlayerInputReader>();
            if (inputReader != null)
            {
                // 보조 무기 교체 이벤트 구독
                inputReader.OnSubWeaponSwitchEvent += UpdateSubWeaponUI;
                
                // 시작 시 기본 무기(0번) 아이콘으로 초기화 (선택 사항)
                UpdateSubWeaponUI(0); 
            }
            else
            {
                Debug.LogError("HUDManager: PlayerInputReader를 찾을 수 없습니다.");
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

        // [추가됨] 이벤트 구독 해제
        if (inputReader != null)
        {
            inputReader.OnSubWeaponSwitchEvent -= UpdateSubWeaponUI;
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

    // [추가됨] 보조 무기 아이콘 교체 함수
    // index: 0, 1, 2 (InputReader에서 4,5,6키를 누르면 각각 0,1,2가 넘어옴)
    public void UpdateSubWeaponUI(int index)
    {
        if (subWeaponImage == null) return;

        // 인덱스가 배열 범위 내에 있는지 확인하여 오류 방지
        if (index >= 0 && index < subWeaponSprites.Length)
        {
            if (subWeaponSprites[index] != null)
            {
                subWeaponImage.sprite = subWeaponSprites[index];
            }
        }
        else
        {
            Debug.LogWarning($"SubWeapon Index {index}가 범위를 벗어났습니다.");
        }
    }
}