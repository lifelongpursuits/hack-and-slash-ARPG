using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private bool isElite = false;
    [SerializeField] private GameObject deathEffect;

    private float currentHealth;
    public event Action OnEnemyDeath;

    private void Start()
    {
        if (isElite)
        {
            maxHealth *= 2;
        }
        currentHealth = maxHealth;
        Debug.Log($"Enemy initialized with {currentHealth} health");
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"Enemy TakeDamage called with damage: {damage}");
        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage. Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Debug.Log("Enemy health reached 0, calling Die()");
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy Die() called");
        // Spawn death effect if assigned
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        OnEnemyDeath?.Invoke();
        Debug.Log("Destroying enemy object");
        Destroy(gameObject);
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
} 