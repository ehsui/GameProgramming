using UnityEngine;

public class BossHitbox : MonoBehaviour
{
    [Header("데미지 설정")]
    public int damage = 10; // 인스펙터에서 공격별로 다르게 설정 가능

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어와 충돌했는지 확인
        if (collision.CompareTag("Player"))
        {
            // 플레이어의 PlayerStats 스크립트를 가져옴
            PlayerStats pStats = collision.GetComponent<PlayerStats>();

            // 스크립트가 잘 붙어있다면 데미지 주기
            if (pStats != null)
            {
                pStats.TakeDamage(damage);
                Debug.Log($"<color=red>공격 명중!</color> {gameObject.name}가 {damage} 데미지를 입혔습니다.");
            }
        }
        
    }
}