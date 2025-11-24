using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillCaster : MonoBehaviour
{
    [Header("Settings")]
    // 모든 스킬 조합법을 여기에 등록합니다 (총 9개 예정)
    public List<SkillCombinationData> combinations;

    private PlayerController controller;
    private PlayerStats stats;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        stats = GetComponent<PlayerStats>();
    }

    // 스킬 발동 시도 (Controller가 호출함)
    public SkillData GetCurrentSkill()
    {
        // 1. 현재 들고 있는 무기 ID 확인
        // (무기가 없으면 ID를 0이나 -1로 처리하는 방어 코드 필요)
        int mainId = controller.CurrentMainWeapon != null ? controller.CurrentMainWeapon.weaponId : -1;
        int subId = controller.CurrentSubWeapon != null ? controller.CurrentSubWeapon.weaponId : -1;

        // 2. 조합 리스트 뒤져서 맞는 스킬 찾기
        foreach (var combo in combinations)
        {
            if (combo.mainWeaponId == mainId && combo.subWeaponId == subId)
            {
                // 3. 스킬 찾음! -> 사용 가능 여부 체크 (마나, 쿨타임)
                if (CanUseSkill(combo.skillData))
                {
                    return combo.skillData;
                }
                else
                {
                    return null; // 마나 부족 등으로 사용 불가
                }
            }
        }

        Debug.LogWarning($"조합을 찾을 수 없음: Main {mainId} + Sub {subId}");
        return null;
    }

    // 마나 체크 및 쿨타임 체크 (쿨타임은 나중에 구현)
    private bool CanUseSkill(SkillData data)
    {
        if (data == null) return false;

        // 마나 부족 체크
        if (stats.CurrentBodhicitta < data.bodhicittaCost)
        {
            Debug.Log("보리심(마나)이 부족합니다!");
            return false;
        }

        return true;
    }
}