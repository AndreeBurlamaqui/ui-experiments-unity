using System;
using UnityEngine;

[Serializable]
public class ResourceEntry
{
    [SerializeField] private ResourceType type;
    [SerializeField] private double currentAmount;
    [SerializeField] private UpgradeInstance extractionSpeed; // units per second

    private double _accumulator;

    public ResourceType Type => type;
    public double CurrentAmount => currentAmount;

    /// <summary>
    /// Update this resource production by deltaTime.
    /// </summary>
    public void Update(float deltaTime)
    {
        var efficacy = extractionSpeed.GetTotalPower();
        if (efficacy <= 0) return;

        _accumulator += efficacy * deltaTime;
        if (_accumulator >= 1.0)
        {
            var whole = Math.Floor(_accumulator);
            currentAmount += whole;
            _accumulator -= whole;
        }
    }

    /// <summary>
    /// Try decreasing by a given <paramref name="amount"/>.
    /// Will give either the requested amount, or the remaining.
    /// </summary>
    /// <param name="amount">How much to try to take.</param>
    /// <returns>The actual amount removed.</returns>
    public double Decrease(double amount)
    {
        if (amount <= 0)
            return 0;

        double removed;
        if (currentAmount >= amount)
        {
            currentAmount -= amount;
            removed = amount;
        }
        else
        {
            removed = currentAmount;
            currentAmount = 0;
        }

        return removed;
    }


    public void Add(double amount)
    {
        if (amount <= 0) return;
        currentAmount += amount;
    }
}