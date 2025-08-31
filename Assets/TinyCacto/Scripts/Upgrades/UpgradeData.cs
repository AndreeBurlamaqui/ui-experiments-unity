using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Tiny Cacto/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    [field: SerializeField, Tooltip("Name of the upgrade")] public string Title { get; private set; }

    // Cost is related to how much gold the player will need to invest
    [SerializeField, Tooltip("Cost at level 1 (in gold)")] private float BaseCost;
    [SerializeField, Tooltip("Multiplier per level")] private float CostMultiplier;

    // Power is a general "value" related to the upgrade
    // For extraction speed, this could be seconds per resource.
    // For boats, this could be number of active boats.
    [SerializeField, Tooltip("Initial power value")] private float BasePower;
    [SerializeField, Tooltip("How power scales per level")] private float PowerMultiplier;

    /// <summary>
    /// Calculate the cost to upgrade from current level by N levels.
    /// </summary>
    public float GetCost(int currentLevel, int levelsToBuy = 1)
    {
        float totalCost = 0f;
        for (int i = 0; i < levelsToBuy; i++)
        {
            int level = currentLevel + i;
            totalCost += BaseCost * Mathf.Pow(CostMultiplier, level);
        }
        return totalCost;
    }

    public float GetTotalPower(int currentLevel)
    {
        if (PowerMultiplier == 1f)
            return BasePower * currentLevel;

        // Geometric series sum: BasePower * (r^n - 1) / (r - 1)
        return BasePower * (Mathf.Pow(PowerMultiplier, currentLevel) - 1f) / (PowerMultiplier - 1f);
    }
}
