using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGroup : MonoBehaviour
{

    [SerializeField] private ResourceType[] resources;

    public void OnClick()
    {
        // Check if it's already collected

        // If not, offer to buy
    }

    [ContextMenu("On click to collect")]
    public void InitiateCollection()
    {
        Nexus.Instance.RegisterAsResource(this);
    }

    public List<ResourceAmount> Collect()
    {
        List<ResourceAmount> collected = new();
        for (int r = 0; r < resources.Length; r++)
        {
            collected.Add(new(resources[r], 1));
        }

        return collected;
    }
}

[System.Serializable]
public struct ResourceAmount
{
    public ResourceType type;
    public int amount;

    public ResourceAmount(ResourceType type, int amount)
    {
        this.type = type;
        this.amount = amount;
    }

    public override bool Equals(object obj)
    {
        return obj is ResourceAmount other &&
               EqualityComparer<ResourceType>.Default.Equals(type, other.type) &&
               amount == other.amount;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(type, amount);
    }

    public void Deconstruct(out ResourceType type, out int amount)
    {
        type = this.type;
        amount = this.amount;
    }

    public static implicit operator (ResourceType type, int amount)(ResourceAmount value)
    {
        return (value.type, value.amount);
    }

    public static implicit operator ResourceAmount((ResourceType type, int amount) value)
    {
        return new ResourceAmount(value.type, value.amount);
    }
}