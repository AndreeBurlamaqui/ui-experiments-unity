using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResourceGroup : MonoBehaviour, IWorldSpaceSelectable
{

    [SerializeField] private ResourceEntry[] resources;
    public IReadOnlyList<ResourceEntry> Resources => resources;

    public void Tick()
    {
        for(int r = 0; r < resources.Length; r++)
        {
            resources[r].Update(Time.deltaTime);
        }
    }

    [ContextMenu("On click to collect")]
    public void OnSelect()
    {
        // Check if it's already collected
        if (ResourceManager.Instance.IsGroupOwned(this))
        {
            // If so, show resource UI
            GameplayUIController.Instance.DisplayResourceUI(this);
        }
        else
        {
            // If not, offer to buy
            ResourceManager.Instance.RegisterAsResource(this);
        }
    }

    /// <summary>
    /// Collect resources from this group, limited by a worker's carry capacity.
    /// Splits capacity across available resources in order.
    /// </summary>
    /// <param name="carryCapacity">How much the worker can carry in total.</param>
    /// <returns>List of collected resources (type + amount actually taken).</returns>
    public List<CollectedResource> Collect(double carryCapacity)
    {
        var collected = new List<CollectedResource>();
        if (carryCapacity <= 0) return collected;

        // Work on a copy of available amounts
        var available = new List<(ResourceEntry entry, double amount)>();
        for (int r = 0; r < resources.Length; r++)
        {
            ResourceEntry entry = resources[r];
            if (entry.CurrentAmount > 0)
                available.Add((entry, entry.CurrentAmount));
        }

        if (available.Count == 0) return collected;

        // Sort by abundance ascending (scarce first, abundant last)
        available.Sort((a, b) => a.amount.CompareTo(b.amount));

        double remaining = carryCapacity;

        for (int a = 0; a < available.Count; a++)
        {
            (ResourceEntry entry, double amount) pair = available[a];
            if (remaining <= 0) break;

            // Proportional target: fraction of remaining based on relative abundance
            double totalAvailable = 0;
            foreach (var p in available) totalAvailable += p.amount;
            double fraction = pair.amount / totalAvailable;

            double target = Math.Min(pair.amount, Math.Ceiling(carryCapacity * fraction));

            // Ensure at least 1 if available
            if (pair.amount > 0 && target < 1)
                target = 1;

            // Clamp by remaining capacity
            target = Math.Min(target, remaining);

            double taken = pair.entry.Decrease(target);
            if (taken > 0)
            {
                collected.Add(new CollectedResource(pair.entry.Type, taken));
                remaining -= taken;
            }
        }

        // If there’s leftover capacity, dump it into the most abundant resource
        if (remaining > 0 && available.Count > 0)
        {
            var last = available[available.Count - 1];
            double taken = last.entry.Decrease(remaining);
            if (taken > 0)
                collected.Add(new CollectedResource(last.entry.Type, taken));
        }

        return collected;
    }
}

/// <summary>
/// Result of a collect operation: type + amount collected.
/// </summary>
public struct CollectedResource
{
    public ResourceType Type;
    public double Amount;

    public CollectedResource(ResourceType type, double amount)
    {
        Type = type;
        Amount = amount;
    }
}