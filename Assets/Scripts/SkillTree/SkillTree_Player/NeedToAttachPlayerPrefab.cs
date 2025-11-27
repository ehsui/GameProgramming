using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 레벨 저장을 위한 스크립트
// 플레이어 프리팹에 붙이기
public class NeedToAttachPlayerPrefab : MonoBehaviour
{
    public PlayerLevelSaveData levelData;
    public PlayerStats playerStats;
    
    void Start()
    {
        if(levelData.level <= 0)
            levelData.level = 1;
    }

    void Update()
    {
        // 플레이 중 레벨업하면 savadata에 저장
        if (levelData.level != playerStats.level)
            levelData.level = playerStats.level;
    }
}
