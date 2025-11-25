using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    private Animator anim; // 애니메이터 참조 변수

    protected override void Awake()
    {
        base.Awake(); // 부모 클래스(Health)의 체력 초기화 코드를 먼저 실행
        anim = GetComponent<Animator>(); // 같은 오브젝트에 있는 Animator 가져오기
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount); // 체력 감소 및 Die() 체크

        // 체력이 0보다 클 때만 피격 모션을 재생
        if (currentHealth > 0)
        {
            if (anim != null)
            {
                Debug.Log("적이 데미지를 입었습니다. 현재 체력: " + currentHealth);
                anim.SetTrigger("Hit"); // 유니티 Animator 파라미터 이름이 "Hit"이어야 함
            }
        }
    }

    public override void Heal(float amount)
    {
        base.Heal(amount);
        Debug.Log("적 체력 회복: " + currentHealth);
    }

    public override void Die()
    {
        // 죽는 애니메이션을 재생하려면 base.Die()를 바로 부르면 안 됨.
        // base.Die() 안에 Destroy가 있어서 애니메이션 나오기 전에 삭제됨
        
        if (anim != null)
        {
            anim.SetBool("IsDead", true); // 애니메이션 재생
        }

        // 일단은 기본 로직 유지 (바로 삭제됨) -> 나중에 코루틴으로 딜레이 삭제 구현 필요
        base.Die(); 
    }
}