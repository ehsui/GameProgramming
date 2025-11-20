using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierInfo : MonoBehaviour
{
    public GameObject barrierInfoText;
    public LeverInfo lever;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !lever.leverActived)
        {
            barrierInfoText.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            barrierInfoText.SetActive(false);
        }
    }
}
