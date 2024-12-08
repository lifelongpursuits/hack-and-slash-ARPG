using UnityEngine;
using System;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float maxMana = 100f;
    [SerializeField] private float healthRegenRate = 1f;
    [SerializeField] private float manaRegenRate = 2f;

    private float currentHealth;
    private float currentMana;
    private float damageMultiplier = 1f;
    private float armor = 0f;

    public event Action<float> OnHealthChanged;
    public event Action<float> OnManaChanged;
    public event Action OnPlayerDeath;

    public float CurrentHealth => currentHealth;
    public float CurrentMana => currentMana;
    public float MaxHealth => maxHealth;
    public float MaxMana => maxMana;
    public float DamageMultiplier => damageMultiplier;
    public float Armor => armor;

    private void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
    }

    private void Update()
    {
        RegenerateResources();
    }

    private void RegenerateResources()
    {
        if (currentHealth < maxHealth)
        {
            ModifyHealth(healthRegenRate * Time.deltaTime);
        }

        if (currentMana < maxMana)
        {
            ModifyMana(manaRegenRate * Time.deltaTime);
        }
    }

    public void ModifyHealth(float amount)
    {
        if (amount < 0)
        {
            // Apply armor reduction to damage
            amount *= (1f - (armor / (armor + 100f)));
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void ModifyMana(float amount)
    {
        currentMana = Mathf.Clamp(currentMana + amount, 0, maxMana);
        OnManaChanged?.Invoke(currentMana / maxMana);
    }

    public void ModifyMaxHealth(float amount)
    {
        maxHealth += amount;
        ModifyHealth(amount); // Heal for the increased amount
    }

    public void ModifyMaxMana(float amount)
    {
        maxMana += amount;
        ModifyMana(amount); // Restore mana for the increased amount
    }

    public void ModifyDamageMultiplier(float amount)
    {
        damageMultiplier += amount;
    }

    public void ModifyArmor(float amount)
    {
        armor += amount;
    }

    public bool HasEnoughMana(float amount)
    {
        return currentMana >= amount;
    }

    public void RestoreFullHealth()
    {
        ModifyHealth(maxHealth);
    }

    public void RestoreFullMana()
    {
        ModifyMana(maxMana);
    }

    private void Die()
    {
        OnPlayerDeath?.Invoke();
        // Additional death logic here
    }
}
