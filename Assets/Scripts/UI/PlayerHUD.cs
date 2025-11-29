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

    // === 플레이어 스탯 참조 (PlayerStats 하나만 씀) ===
    private PlayerStats playerStats;

    void Start()
    {
        // 1. 태그로 플레이어 찾기
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        
        if (playerObject != null)
        {
            // 2. PlayerStats 컴포넌트 하나만 가져오면 됨
            playerStats = playerObject.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                // 3. 이벤트 구독 (모든 이벤트가 PlayerStats에 있음)
                playerStats.OnHealthChanged += UpdateHealthBar;
                playerStats.OnBodhicittaChanged += UpdateBodhicittaBar;
                //playerStats.OnPotionChanged += UpdatePotionIcons; 
                
                // 4. 초기값 갱신 (UI 강제 동기화)
                UpdateHealthBar(playerStats.CurrentHealth, playerStats.maxHealth);
                UpdateBodhicittaBar(playerStats.CurrentBodhicitta, playerStats.maxBodhicitta);
                //UpdatePotionIcons(playerStats.potionCount);
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
            //playerStats.OnPotionChanged -= UpdatePotionIcons;
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

    public void UpdatePotionIcons(int count)
    {
        for (int i = 0; i < potionIcons.Length; i++)
        {        
            if (potionIcons[i] != null)
            {
                // 현재 개수(count)보다 인덱스(i)가 작으면 '채워진 물약', 아니면 '빈 물약'
                potionIcons[i].sprite = (i < count) ? filledPotionSprite : emptyPotionSprite;
            }
        }
    }

}