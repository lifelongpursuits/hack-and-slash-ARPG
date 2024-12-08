using UnityEngine;
using System;

public class LevelSystem : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private int startingLevel = 1;
    [SerializeField] private float baseXPRequirement = 100f;
    [SerializeField] private float xpRequirementMultiplier = 1.5f;

    [Header("Stat Points")]
    [SerializeField] private int statPointsPerLevel = 5;
    
    [Header("Base Stats")]
    [SerializeField] private float baseHealthPerPoint = 10f;
    [SerializeField] private float baseManaPerPoint = 5f;
    [SerializeField] private float baseDamagePerPoint = 2f;
    [SerializeField] private float baseArmorPerPoint = 1f;

    private int currentLevel;
    private float currentXP;
    private float xpToNextLevel;
    private int availableStatPoints;

    private PlayerStats playerStats;

    public event Action<int> OnLevelUp;
    public event Action<float> OnXPGained;

    public int CurrentLevel => currentLevel;
    public float CurrentXP => currentXP;
    public float XPToNextLevel => xpToNextLevel;
    public int AvailableStatPoints => availableStatPoints;

    // Stat allocations
    private int strengthPoints;
    private int intelligencePoints;
    private int vitalityPoints;
    private int defensePoints;

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        InitializeLevel();
    }

    private void InitializeLevel()
    {
        currentLevel = startingLevel;
        currentXP = 0;
        CalculateXPRequirement();
        availableStatPoints = (currentLevel - 1) * statPointsPerLevel;
    }

    private void CalculateXPRequirement()
    {
        xpToNextLevel = baseXPRequirement * Mathf.Pow(xpRequirementMultiplier, currentLevel - 1);
    }

    public void AddExperience(float xpAmount)
    {
        currentXP += xpAmount;
        OnXPGained?.Invoke(currentXP / xpToNextLevel);

        while (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentXP -= xpToNextLevel;
        currentLevel++;
        availableStatPoints += statPointsPerLevel;
        CalculateXPRequirement();
        
        // Heal player to full on level up
        if (playerStats != null)
        {
            playerStats.RestoreFullHealth();
            playerStats.RestoreFullMana();
        }

        OnLevelUp?.Invoke(currentLevel);
    }

    public void AllocateStatPoint(StatType statType)
    {
        if (availableStatPoints <= 0) return;

        switch (statType)
        {
            case StatType.Strength:
                strengthPoints++;
                playerStats.ModifyDamageMultiplier(baseDamagePerPoint);
                break;
            case StatType.Intelligence:
                intelligencePoints++;
                playerStats.ModifyMaxMana(baseManaPerPoint);
                break;
            case StatType.Vitality:
                vitalityPoints++;
                playerStats.ModifyMaxHealth(baseHealthPerPoint);
                break;
            case StatType.Defense:
                defensePoints++;
                playerStats.ModifyArmor(baseArmorPerPoint);
                break;
        }

        availableStatPoints--;
    }

    public int GetStatPoints(StatType statType)
    {
        return statType switch
        {
            StatType.Strength => strengthPoints,
            StatType.Intelligence => intelligencePoints,
            StatType.Vitality => vitalityPoints,
            StatType.Defense => defensePoints,
            _ => 0
        };
    }
}

public enum StatType
{
    Strength,
    Intelligence,
    Vitality,
    Defense
}
