using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("UI References")]
    public Image healthBarFill; // 붉은색 채워지는 이미지
    public EnemyHealth targetEnemy; // 연결할 적 스크립트

    private float maxHealth;

    void Start()
    {
        // 타겟이 연결 안 되어 있으면 부모에서 찾기
        if (targetEnemy == null)
        {
            targetEnemy = GetComponentInParent<EnemyHealth>();
        }

        if (targetEnemy != null)
        {
            // Health 스크립트에 maxHealth가 public이라면 targetEnemy.maxHealth 사용
            // 여기서는 현재 체력을 Max로 가정합니다.
            maxHealth = targetEnemy.currentHealth; 
        }
    }

    void Update()
    {
        if (targetEnemy == null || healthBarFill == null) return;

        // 1. 체력바 비율 갱신
        float hpRatio = targetEnemy.currentHealth / maxHealth;
        healthBarFill.fillAmount = hpRatio;

        // 2. 적이 죽었을 때 처리
        if (targetEnemy.currentHealth <= 0)
        {
            gameObject.SetActive(false); // 체력바 숨기기
        }
    }

    // 적이 좌우 반전(Flip X)될 때 체력바가 같이 뒤집히는 것을 방지하려면
    void LateUpdate()
    {
        // 부모(적)가 왼쪽을 봐서 scale.x가 -1이 되어도, 체력바는 정상적으로 보이게 함
        if (transform.lossyScale.x < 0)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

}