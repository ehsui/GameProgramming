using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    // === UI Image 및 Text 요소 참조 ===
    [Header("UI Image References")]
    public Image healthBarFill;
    public Image bodhicittaBarFill;

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
                
                // 4. 초기값 갱신
                UpdateHealthBar(playerStats.CurrentHealth, playerStats.maxHealth);
                UpdateBodhicittaBar(playerStats.CurrentBodhicitta, playerStats.maxBodhicitta);
                
                UpdatePotionIcons(playerStats.potionCount);
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
            
            // ? [수정 3] 주석 해제 & 이름 일치
            playerStats.OnPotionCountChanged -= UpdatePotionIcons;
        }
    }

    // --- [UI 업데이트 함수들] ---

    public void UpdateHealthBar(float current, float max)
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = current / max;
        }
    }

    public void UpdateBodhicittaBar(float current, float max)
    {
        if (bodhicittaBarFill != null)
        {
            bodhicittaBarFill.fillAmount = current / max;
        }
    }

    // 포션 아이콘 업데이트
    public void UpdatePotionIcons(int count)
    {
        // 배열 크기만큼 반복
        for (int i = 0; i < potionIcons.Length; i++)
        {        
            if (potionIcons[i] != null)
            {
                // 현재 개수(count)보다 인덱스(i)가 작으면 '채워진 물약', 아니면 '빈 물약'
                // 예: count가 2개면 -> index 0(참), index 1(참), index 2(거짓)
                potionIcons[i].sprite = (i < count) ? filledPotionSprite : emptyPotionSprite;
            }
        }
    }
}