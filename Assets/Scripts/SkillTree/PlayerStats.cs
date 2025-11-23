using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public float attackPower = 10f;
    public float defensePower = 10f;

    void Awake()
    {
        if (Instance = null) Instance = this;
        else Destroy(gameObject);
    }

    public void AttackPowerUpgrade(float percent)
    {
        attackPower += attackPower * (percent / 100f);
        Debug.Log("공격력 증가! 현재 공격력 = " + attackPower);
    }

    public void DefensePowerUpgrade(float percent)
    {
        defensePower += defensePower * (percent / 100f);
        Debug.Log("방어력 증가! 현재 방어력 = " + defensePower);
    }
}
