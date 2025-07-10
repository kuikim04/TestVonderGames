using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class PlayerStat : MonoBehaviour
{
    public float atackPower = 5;
    public float maxHP = 100;
    private float currentHP;

    public static event Action<float, float> OnHPChanged;
    public static event Action<float> OnHealedRequest;
    public static event Action OnDeath;

    private void OnEnable()
    {
        OnHealedRequest += HandleHealRequest;
    }

    private void OnDisable() 
    { 
        OnHealedRequest -= HandleHealRequest; 
    }

    private void Start()
    {
        currentHP = maxHP;
        OnHPChanged?.Invoke(currentHP, maxHP);
    }

    private void Update()
    {
        if (currentHP < 0) currentHP = 0;

        if (Input.GetKeyUp(KeyCode.P))
            TakeDamage(atackPower);
    }

    public void TakeDamage(float damage)
    {
        float dmg = UnityEngine.Random.Range(damage * 0.8f, damage * 1.2f);
        currentHP = Mathf.Max(currentHP - dmg, 0);

        OnHPChanged?.Invoke(currentHP, maxHP);

        if (currentHP <= 0)
            Die();
    }

    public void Heal(float amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
        OnHPChanged?.Invoke(currentHP, maxHP);
    }

    private void HandleHealRequest(float amount)
    {
        Heal(amount);
    }

    public static void RequestHeal(float amount) 
    { 
        OnHealedRequest?.Invoke(amount); 
    }

    private void Die()
    {
        Debug.Log("Player Died");
        StartCoroutine(WaitForChangeHP());
    }

    private IEnumerator WaitForChangeHP()
    {
        OnDeath?.Invoke();
        yield return new WaitForSeconds(4f);
        Heal(maxHP);
    }

    public float GetCurrentHP() => currentHP;
}
