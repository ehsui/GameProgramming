using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class WizardBoss : MonoBehaviour
{
    [Header("기본 설정")]
    public Transform player;
    public float moveSpeed = 3f;
    public float patternInterval = 2.5f;

    [Header("애니메이션 시간")]
    public float attack1Time = 1.0f;
    public float attack2Time = 1.0f;
    public float castTime = 2.0f;

    [Header("플레이어에게 입히는 데미지 설정")]
    public float attackDamage = 10f; // 접촉 시 줄 데미지
    public float explosionRadius = 3.5f; // 폭발 범위

    // 내부 변수
    private Animator anim;
    private SpriteRenderer sr;
    private Vector3 defaultScale;

    // 이동 관련 상태 변수 (Update에서 사용)
    private bool isChasing = false;
    private bool isRunningToCast = false;

    public GameObject teleportEffect;
    public GameObject explosionEffect;
    public float effectDestroyTime = 0.9f;
    public Vector3 effectOffset = new Vector3(0, -7.5f, 0);
    public Vector3 explosionEffectOffset = new Vector3(0, 0.0f, 0);

    public Transform explosionZone;

    void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        defaultScale = transform.localScale;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        Invoke("SelectRandomPattern", 1f);
    }

    void Update()
    {
        if (player == null) return;
        LookAtPlayer();

        if (isChasing)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.position.x, transform.position.y), moveSpeed * Time.deltaTime);
        }

        if (isRunningToCast)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.position.x, transform.position.y), moveSpeed * Time.deltaTime);
        }
    }

    void SelectRandomPattern()
    {
        int rand = UnityEngine.Random.Range(0, 4); // 0, 1, 2, 3 중 하나 랜덤

        switch (rand)
        {
            case 0: Pattern1_Attack(); break;
            case 1: Pattern2_ComboStart(); break;
            case 2: Pattern3_ChaseStart(); break;
            case 3: Pattern4_ExplosionStart(); break;

        }
    }

    void Pattern1_Attack()
    {
        anim.SetTrigger("Attack1");

        // 공격 모션이 끝나는 시간 뒤에 순간이동 실행
        Invoke("Teleport", attack1Time);
    }

    void Pattern2_ComboStart()
    {
        anim.SetTrigger("Attack1");

        Invoke("Pattern2_ComboFinish", attack1Time * 0.9f);
    }

    void Pattern2_ComboFinish()
    {

        anim.SetTrigger("Attack2");

        Invoke("Teleport", attack2Time);
    }

    void Pattern3_ChaseStart()
    {

        isChasing = true; // Update에서 이동 시작
        anim.SetBool("Move", true);

        Invoke("Pattern3_ChaseEndAndAttack", 1.0f);
    }

    void Pattern3_ChaseEndAndAttack()
    {
        isChasing = false; // 이동 멈춤
        anim.SetBool("Move", false);

        anim.SetTrigger("Attack1");

        Invoke("Teleport", attack1Time);
    }

    void Pattern4_ExplosionStart()
    {

        isRunningToCast = true;
        anim.SetBool("Move", true);

        Invoke("Pattern4_PrepareCast", 1.0f);
    }

    void Pattern4_PrepareCast()
    {
        isRunningToCast = false;
        anim.SetBool("Move", false);

        Invoke("Pattern4_Boom", castTime);
    }

    void Pattern4_Boom()
    {
        if (explosionEffect != null)
        {
            Vector3 spawnPos = transform.position;

            if (explosionZone != null)
            {
                spawnPos = explosionZone.position;
            }

            GameObject boom = Instantiate(explosionEffect, spawnPos, Quaternion.identity);
            Destroy(boom, 2.0f);
        }

        Invoke("Teleport", 0.5f);
    }

    void Teleport()
    {
        float randomX = UnityEngine.Random.Range(2f, 5f);
        float direction = UnityEngine.Random.Range(0, 2) == 0 ? -1f : 1f;

        Vector2 targetPos = new Vector2(player.position.x + (randomX * direction), transform.position.y);
        transform.position = targetPos;

        if (teleportEffect != null)
        {
            Vector3 spawnPos = transform.position + effectOffset;
            GameObject effect = Instantiate(teleportEffect, spawnPos, Quaternion.identity);
            Destroy(effect, effectDestroyTime);
        }
        Invoke("SelectRandomPattern", patternInterval);
    }

    void LookAtPlayer()
    {
        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(defaultScale.x), defaultScale.y, defaultScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-Mathf.Abs(defaultScale.x), defaultScale.y, defaultScale.z);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 부딪힌 게 플레이어인지 확인
        if (collision.CompareTag("Player"))
        {
            print("플레이어와 보스가 충돌");
            // 2. 플레이어 스크립트 가져오기
            PlayerStats stats = collision.GetComponent<PlayerStats>();

            // 3. 데미지 주기
            if (stats != null)
            {
                stats.TakeDamage(attackDamage);
                print("보스에 닿아서 데미지!");
            }
        }
    }
}
