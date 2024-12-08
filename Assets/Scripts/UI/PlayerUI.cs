using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] private Image healthFillImage;
    [SerializeField] private TextMeshProUGUI healthText;
    
    [Header("Mana Bar")]
    [SerializeField] private Image manaFillImage;
    [SerializeField] private TextMeshProUGUI manaText;

    [Header("Experience Bar")]
    [SerializeField] private Image experienceFillImage;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Cooldown Indicators")]
    [SerializeField] private Image basicAttackCooldownImage;
    [SerializeField] private Image specialAttackCooldownImage;

    private PlayerStats playerStats;
    private LevelSystem levelSystem;
    private CombatSystem combatSystem;

    private void Start()
    {
        // Find the player and get required components
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerStats = player.GetComponent<PlayerStats>();
            levelSystem = player.GetComponent<LevelSystem>();
            combatSystem = player.GetComponent<CombatSystem>();

            // Subscribe to events
            playerStats.OnHealthChanged += UpdateHealthBar;
            playerStats.OnManaChanged += UpdateManaBar;
            levelSystem.OnXPGained += UpdateExperienceBar;
            levelSystem.OnLevelUp += UpdateLevelText;
        }
        else
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
        }

        // Initial UI update
        UpdateHealthBar(1f);
        UpdateManaBar(1f);
        UpdateLevelText(1);
    }

    private void UpdateHealthBar(float healthPercentage)
    {
        healthFillImage.fillAmount = healthPercentage;
        healthText.text = $"{Mathf.Round(playerStats.CurrentHealth)}/{playerStats.MaxHealth}";
    }

    private void UpdateManaBar(float manaPercentage)
    {
        manaFillImage.fillAmount = manaPercentage;
        manaText.text = $"{Mathf.Round(playerStats.CurrentMana)}/{playerStats.MaxMana}";
    }

    private void UpdateExperienceBar(float xpPercentage)
    {
        experienceFillImage.fillAmount = xpPercentage;
    }

    private void UpdateLevelText(int level)
    {
        levelText.text = $"Level {level}";
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (playerStats != null)
        {
            playerStats.OnHealthChanged -= UpdateHealthBar;
            playerStats.OnManaChanged -= UpdateManaBar;
        }
        if (levelSystem != null)
        {
            levelSystem.OnXPGained -= UpdateExperienceBar;
            levelSystem.OnLevelUp -= UpdateLevelText;
        }
    }
} 