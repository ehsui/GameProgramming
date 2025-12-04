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

    void LateUpdate()
    {
        if (transform.parent == null) return;

        // 1. 현재 내 원래 크기 수치(절댓값)를 가져옵니다.
        Vector3 myScale = transform.localScale;
        float xSize = Mathf.Abs(myScale.x); 

        // 2. 부모의 "월드 기준" 스케일(lossyScale)을 확인합니다.
        //    (부모가 부모의 부모에 의해 뒤집혔든, 스스로 뒤집혔든 최종 결과만 봅니다)
        float parentGlobalX = transform.parent.lossyScale.x;

        // 3. 부모가 월드에서 음수(뒤집힘) 상태라면 -> 나도 음수로 설정해서 (- * - = +)로 상쇄
        //    부모가 월드에서 양수(정상) 상태라면 -> 나도 양수로 설정 (+ * + = +)
        if (parentGlobalX < 0)
        {
            myScale.x = -xSize; 
        }
        else
        {
            myScale.x = xSize;
        }

        transform.localScale = myScale;
    }

}