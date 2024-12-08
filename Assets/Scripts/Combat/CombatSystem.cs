using UnityEngine;
using System;

public class CombatSystem : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float basicAttackDamage = 10f;
    [SerializeField] private float basicAttackRange = 5f;
    [SerializeField] private float basicAttackCooldown = 0.5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float attackAngle = 90f;

    private PlayerStats playerStats;
    private float lastBasicAttackTime;

    // Events for combat effects
    public event Action<Vector3, Vector3> OnBasicAttack;

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
            Debug.LogError("PlayerStats not found on Player!");
            
        Debug.Log($"CombatSystem initialized. Enemy Layer: {enemyLayer.value}");
    }

    private void Update()
    {
        HandleCombatInput();
    }

    private void HandleCombatInput()
    {
        // Basic Attack (Left Click)
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Left click detected");
            if (CanBasicAttack())
            {
                PerformBasicAttack();
            }
            else
            {
                Debug.Log("Attack on cooldown");
            }
        }
    }

    private bool CanBasicAttack()
    {
        return Time.time >= lastBasicAttackTime + basicAttackCooldown;
    }

    private void PerformBasicAttack()
    {
        Debug.Log("Performing basic attack");
        lastBasicAttackTime = Time.time;

        // Get attack direction from mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPoint = transform.position + transform.forward * basicAttackRange;
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            targetPoint = hit.point;
            Debug.Log($"Attack targeting point: {hit.point}");
        }

        Vector3 attackDirection = (targetPoint - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(attackDirection.x, 0, attackDirection.z));

        // Find enemies in range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, basicAttackRange, enemyLayer);
        Debug.Log($"Found {hitColliders.Length} potential targets in range");

        foreach (var hitCollider in hitColliders)
        {
            Debug.Log($"Checking collider: {hitCollider.gameObject.name} on layer: {hitCollider.gameObject.layer}");
            
            if (hitCollider.TryGetComponent<EnemyHealth>(out var enemyHealth))
            {
                Vector3 directionToEnemy = (hitCollider.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(attackDirection, directionToEnemy);
                
                Debug.Log($"Enemy found at angle: {angle} degrees from attack direction");
                
                if (angle <= attackAngle / 2)
                {
                    float totalDamage = basicAttackDamage * (playerStats != null ? playerStats.DamageMultiplier : 1f);
                    Debug.Log($"Enemy within attack angle! Attacking for {totalDamage} damage");
                    enemyHealth.TakeDamage(totalDamage);
                }
                else
                {
                    Debug.Log($"Enemy outside attack angle ({angle} > {attackAngle/2})");
                }
            }
            else
            {
                Debug.Log($"Found object '{hitCollider.gameObject.name}' in range but it doesn't have EnemyHealth component");
            }
        }

        // Trigger effects
        OnBasicAttack?.Invoke(transform.position + transform.forward, attackDirection);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, basicAttackRange);

        // Visualize attack angle
        if (Application.isPlaying)
        {
            Vector3 forward = transform.forward;
            Vector3 right = Quaternion.Euler(0, attackAngle/2, 0) * forward;
            Vector3 left = Quaternion.Euler(0, -attackAngle/2, 0) * forward;
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, forward * basicAttackRange);
            Gizmos.DrawRay(transform.position, right * basicAttackRange);
            Gizmos.DrawRay(transform.position, left * basicAttackRange);
        }
    }
}
