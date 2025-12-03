using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ActiveExitDoor : MonoBehaviour
{
    [Header("연결 대상")]
    public GameObject exitDoor;
    public EnemyHealth wiardBossHp; // 보스 체력 확인

    private bool isActivated = false; // 중복 실행 방지 변수

    void Update()
    {
        if (isActivated || wiardBossHp == null) return;

        // 보스의 체력이 0 이하라면 (Health 스크립트의 변수가 public이어야 함)
        // 만약 currentHealth 접근이 안 되면 Health 스크립트에서 public으로 바꿔주세요.
        if (wiardBossHp.currentHealth <= 0)
        {
            ActivateDoor();
        }
    }

    void ActivateDoor()
    {
        isActivated = true; 
        if (exitDoor != null)
        {
            exitDoor.SetActive(true); // 문 활성화
        }
    }
}
