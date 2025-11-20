using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoTrigger : MonoBehaviour
{
    public GameObject InfoText;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InfoText.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InfoText.SetActive(false);
        }
    }
}
