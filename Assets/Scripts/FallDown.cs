using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDown : MonoBehaviour
{
    public Transform restartZone;
    float fallDownDamage = 5f;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            PlayerStats playerStats = collider.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                playerStats.TakeDamage(fallDownDamage);
            }

            if (restartZone != null)
            {
                collider.transform.position = restartZone.position;
                Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = Vector2.zero;
                }
            }
        }
    }
}
