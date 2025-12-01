using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Cinemachine;

public class CameraSetting : MonoBehaviour
{
    void Start()
    {
        CinemachineVirtualCamera vcam = GetComponent<CinemachineVirtualCamera>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            vcam.Follow = player.transform;
        }
        else
        {
            print("플레이어를 찾을 수 없습니다! PEPE의 태그가 'Player'인지 확인해주세요.");
        }
    }

}
