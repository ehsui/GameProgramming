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
    public float patternInterval = 2f; // 패턴 사이 대기 시간

    [Header("애니메이션 시간 (초)")]
    public float attack1Time = 0.8f;
    public float attack2Time = 0.8f;
    public float castTime = 2.0f; // 폭발 기 모으는 시간

    // 내부 변수
    private Animator anim;
    private SpriteRenderer sr;
    private Vector3 defaultScale;

    // 이동 관련 상태 변수 (Update에서 사용)
    private bool isChasing = false;
    private bool isRunningToCast = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        defaultScale = transform.localScale;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        // 1초 뒤에 패턴 시작 루프를 가동합니다.
        Invoke("SelectRandomPattern", 1f);
    }

    void Update()
    {
        if (player == null) return;

        // 플레이어 바라보기 (항상 실행)
        LookAtPlayer();

        // [패턴 3] 추적 이동 로직
        if (isChasing)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.position.x, transform.position.y), moveSpeed * Time.deltaTime);
        }

        // [패턴 4] 폭발 위치로 이동 로직
        if (isRunningToCast)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.position.x, transform.position.y), moveSpeed * Time.deltaTime);
        }
    }

    // ====================================================
    // 1. 패턴 선택기 (랜덤 뽑기)
    // ====================================================
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

    // ====================================================
    // 2. 패턴 정의 (Invoke로 연결)
    // ====================================================

    // --- 패턴 1: 단일 공격 ---
    void Pattern1_Attack()
    {
        print("패턴 1: 단일 공격");
        anim.SetTrigger("Attack1");

        // 공격 모션이 끝나는 시간 뒤에 순간이동 실행
        Invoke("Teleport", attack1Time);
    }

    // --- 패턴 2: 연속 공격 ---
    void Pattern2_ComboStart()
    {
        print("패턴 2: 콤보 시작 (1타)");
        anim.SetTrigger("Attack1");

        // 1타가 끝나기 직전에 2타 함수를 예약
        Invoke("Pattern2_ComboFinish", attack1Time * 0.9f);
    }

    void Pattern2_ComboFinish()
    {
        print("패턴 2: 콤보 마무리 (2타)");
        anim.SetTrigger("Attack2");

        // 2타가 끝나는 시간 뒤에 순간이동
        Invoke("Teleport", attack2Time);
    }

    // --- 패턴 3: 추격 후 공격 ---
    void Pattern3_ChaseStart()
    {
        print("패턴 3: 추격 시작");
        isChasing = true; // Update에서 이동 시작
        anim.SetBool("Move", true);

        // 1초 동안 쫓아가다가 멈추고 공격 함수 실행
        Invoke("Pattern3_ChaseEndAndAttack", 1.0f);
    }

    void Pattern3_ChaseEndAndAttack()
    {
        isChasing = false; // 이동 멈춤
        anim.SetBool("Move", false);

        anim.SetTrigger("Attack1"); // 공격!

        // 공격 후 순간이동
        Invoke("Teleport", attack1Time);
    }

    // --- 패턴 4: 폭발 마법 ---
    void Pattern4_ExplosionStart()
    {
        print("패턴 4: 자리 잡기 이동");
        isRunningToCast = true;
        anim.SetBool("Move", true);

        // 1초만 이동하고 기 모으기 시작
        Invoke("Pattern4_PrepareCast", 1.0f);
    }

    void Pattern4_PrepareCast()
    {
        isRunningToCast = false;
        anim.SetBool("Move", false);
        print("기 모으는 중...");

        // 시전 시간(castTime) 뒤에 폭발
        Invoke("Pattern4_Boom", castTime);
    }

    void Pattern4_Boom()
    {
        print("폭발!");
        // Instantiate(explosionEffect, ...); // 이펙트 생성은 나중에

        // 폭발 보여주고 0.5초 뒤에 순간이동
        Invoke("Teleport", 0.5f);
    }


    // ====================================================
    // 3. 마무리 및 순환 (순간이동)
    // ====================================================
    void Teleport()
    {
        // 1. 순간이동 연출 (투명화 등 필요하면 여기에)

        // 2. 위치 이동 (플레이어 주변 랜덤 좌표)
        float randomX = UnityEngine.Random.Range(2f, 5f); // 2~5미터 거리
        float direction = UnityEngine.Random.Range(0, 2) == 0 ? -1f : 1f; // 왼쪽 or 오른쪽

        Vector2 targetPos = new Vector2(player.position.x + (randomX * direction), transform.position.y);
        transform.position = targetPos;

        print("순간이동 완료!");

        // 3. 다시 패턴 선택기로 돌아감 (무한 반복의 고리)
        Invoke("SelectRandomPattern", patternInterval);
    }

    // 플레이어 바라보기 (좌우 반전)
    void LookAtPlayer()
    {
        // 플레이어가 오른쪽에 있으면
        if (player.position.x > transform.position.x)
        {
            // 원래 크기의 X값을 양수(+)로 (오른쪽 보기)
            transform.localScale = new Vector3(Mathf.Abs(defaultScale.x), defaultScale.y, defaultScale.z);
        }
        else
        {
            // 원래 크기의 X값을 음수(-)로 (왼쪽 보기)
            transform.localScale = new Vector3(-Mathf.Abs(defaultScale.x), defaultScale.y, defaultScale.z);
        }
    }
}
