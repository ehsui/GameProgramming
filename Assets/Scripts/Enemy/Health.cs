using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    // 최대 체력과 현재 체력 - 각 적마다 다를 수 있음
    [Header("Health Settings")]
    public int maxHealth = 3;
    public float currentHealth;

    [Header("animation Settings")]
    public float DeathDelayTime = 1.5f;
    
    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public virtual void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    public virtual void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public virtual void Die()
    {
        // 임시 DIE 처리, 이후 죽음 애니메이션 등 필요
        Debug.Log($"{gameObject.name} died");
        Destroy(gameObject);
    }
}
