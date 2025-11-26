using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    // 혹시 애니메이션보다 조금 더 오래 남기고 싶을 때 쓰는 변수
    public float extraDelay = 0f;

    void Start()
    {
        // 1. 내 몸에 붙은 애니메이터 찾기
        Animator anim = GetComponent<Animator>();

        if (anim != null)
        {
            // 2. 현재 재생 중인 애니메이션의 길이(시간)를 알아내기
            // (GetCurrentAnimatorStateInfo(0)은 첫 번째 레이어의 상태를 가져옵니다)
            float animLength = anim.GetCurrentAnimatorStateInfo(0).length;

            // 3. (애니메이션 길이 + 추가 시간) 뒤에 자폭 명령 예약
            Destroy(gameObject, animLength + extraDelay);
        }
        else
        {
            // 만약 애니메이터가 없다면? (안전을 위해 1초 뒤 삭제 or 그냥 두기)
            // 여기서는 기본 1초 뒤 삭제로 설정합니다.
            Destroy(gameObject, 1.0f + extraDelay);
        }
    }
}