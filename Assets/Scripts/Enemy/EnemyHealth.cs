using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    [Header("Death Settings")]
    public float deathDelay = 1.5f;

    [Header("Hit Settings")]
    public float hitStunTime = 0.5f;

    private Animator anim;
    private Rigidbody2D rb;

    // 🔥 두 종류의 이동 스크립트 변수를 모두 선언
    private Enemy enemyScript;   // 근거리 적 (예전 버전)
    private Enemy2 enemy2Script; // 원거리 적 (새 버전)

    protected override void Awake()
    {
        base.Awake();
        
        // 1. Animator 찾기 (자식까지 뒤짐)
        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();

        rb = GetComponent<Rigidbody2D>();

        // 2. 두 스크립트를 모두 찾아봄 (없으면 null이 들어감)
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
            
            // 경직 시작
            StartCoroutine(ApplyHitStun());
        }
    }

    // 경직(Hit Stun) 처리
    IEnumerator ApplyHitStun()
    {
        // 1. 있는 스크립트는 다 끔
        if (enemyScript != null) enemyScript.enabled = false;
        if (enemy2Script != null) enemy2Script.enabled = false;

        if (rb != null) rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(hitStunTime);

        // 2. 살아있다면 다시 킴 (있는 것만)
        if (currentHealth > 0)
        {
            if (enemyScript != null) enemyScript.enabled = true;
            if (enemy2Script != null) enemy2Script.enabled = true;
        }
    }

    public override void Die()
    {
        StopAllCoroutines(); 

        if (anim != null) anim.SetBool("IsDead", true);

        DisableEnemy(); // 영구 정지

        StartCoroutine(DeathDelay());
    }

    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(deathDelay);
        base.Die(); 
    }

    void DisableEnemy()
    {
        // 두 스크립트 중 존재하는 것을 확실히 끔
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