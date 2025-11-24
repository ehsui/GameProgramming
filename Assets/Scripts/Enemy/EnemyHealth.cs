using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    public override void TakeDamage(float amount)
    {

        base.TakeDamage(amount);
    }

    public override void Heal(float amount)
    {
        base.Heal(amount);
        Debug.Log("적 체력 회복: " + currentHealth);

    }

    public override void Die()
    {
        base.Die();

        // TODO: 이후 적만의 죽음 행동이 있다면 추가
    }

}
