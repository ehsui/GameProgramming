using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EnemyActive : MonoBehaviour
{
    public GameObject wizardBoss;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (wizardBoss != null)
            {
                wizardBoss.SetActive(true); 

                gameObject.SetActive(false);
            }
        }
    }
}
