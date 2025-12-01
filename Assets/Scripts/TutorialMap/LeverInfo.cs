using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LeverInfo : MonoBehaviour
{
    public GameObject leverInfoText;
    public GameObject[] ExitBarrier;

    public bool leverActived = false;
    bool isNear = false;

    public CinemachineVirtualCamera playerCamera;
    public CinemachineVirtualCamera barrierCamera;

    void Update()
    {
        if (isNear && !leverActived && Input.GetKeyDown(KeyCode.E))
        {
            ActiveLever();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isNear = true;
            if (!leverActived)
            {
                leverInfoText.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        isNear = false;
        if (collision.CompareTag("Player"))
        {
            leverInfoText.SetActive(false);
        }
    }

    void ActiveLever()
    {
        leverActived = true;

        if (ExitBarrier != null)
        {
            foreach (GameObject i in ExitBarrier)
            {
                i.SetActive(false);
            }
        }
        leverInfoText.SetActive(false);

        barrierCamera.Priority = 20;

        Invoke(nameof(ReturnCameraPriority), 4f);
    }

    void ReturnCameraPriority()
    {
        barrierCamera.Priority = 9;
    }
}
