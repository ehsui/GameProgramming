using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInfo : MonoBehaviour
{
    public GameObject moveInfoText;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            moveInfoText.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            moveInfoText.SetActive(false);
        }
    }

    void Start()
    {

    }

}

