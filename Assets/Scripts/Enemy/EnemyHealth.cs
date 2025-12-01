using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    [Header("Death Settings")]
    public float deathDelay = 1.5f;
    public int karmaReward = 10; // 이 적을 죽이면 얻는 업보 양

    [Header("Hit Settings")]
    public float hitStunTime = 0.5f;

    private Animator anim;
    private Rigidbody2D rb;

    private Enemy enemyScript;   
    private Enemy2 enemy2Script; 

    protected override void Awake()
    {
        base.Awake();
        
        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();

        rb = GetComponent<Rigidbody2D>();

        enemyScript = GetComponent<Enemy>();
        enemy2Script = GetComponent<Enemy2>();
    }

    public override void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;

        base.TakeDamage(amount);
    
        if (currentHealth > 0)
        {
            if (anim != null) anim.SetTrigger("Hit");
            StartCoroutine(ApplyHitStun());
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerStats pStats = player.GetComponent<PlayerStats>();
            if (pStats != null)
            {
                pStats.IncreaseBodhicitta(10f); // 적에게 피해를 줄 때마다 보리심 5 획득
            }
        }
    }

    IEnumerator ApplyHitStun()
    {
        if (enemyScript != null) enemyScript.enabled = false;
        if (enemy2Script != null) enemy2Script.enabled = false;

        if (rb != null) rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(hitStunTime);

        if (currentHealth > 0)
        {
            if (enemyScript != null) enemyScript.enabled = true;
            if (enemy2Script != null) enemy2Script.enabled = true;
        }
    }

    public override void Die()
    {
        StopAllCoroutines(); 

        // 1. 애니메이션 실행
        if (anim != null) anim.SetBool("IsDead", true);

        // 2. 플레이어에게 Karma 지급
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerStats pStats = player.GetComponent<PlayerStats>();
            if (pStats != null)
            {
                pStats.AddKarma(karmaReward); // 설정한 만큼 업보 증가
            }
        }

        // 3. 적 기능 정지 및 삭제 대기
        DisableEnemy(); 
        StartCoroutine(DeathDelay());
    }

    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(deathDelay);
        base.Die(); 
    }

    void DisableEnemy()
    {
        if (enemyScript != null) enemyScript.enabled = false;
        if (enemy2Script != null) enemy2Script.enabled = false;
        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }
}