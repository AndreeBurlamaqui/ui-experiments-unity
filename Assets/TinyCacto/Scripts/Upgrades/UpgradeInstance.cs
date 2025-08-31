using UnityEngine;

[System.Serializable]
public class UpgradeInstance
{
    [field: SerializeField] public UpgradeData Data { get; private set; }
    [field: SerializeField] public int CurrentLevel { get; private set; } = 1;
    [field: SerializeField] public float Multiplier { get; set; } = 1f;

    public UpgradeInstance(UpgradeData data)
    {
        Data = data;
    }

    /// <summary>
    /// Upgrade by N levels (default 1), returns the total gold cost.
    /// </summary>
    public float Upgrade(int levels = 1)
    {
        float cost = Data.GetCost(CurrentLevel, levels);
        CurrentLevel += levels;
        return cost;
    }

    /// <summary>
    /// Total power including local multiplier and optional global multiplier.
    /// </summary>
    public float GetTotalPower()
    {
        float basePower = Data.GetTotalPower(CurrentLevel);
        return basePower * (Multiplier * UpgradeManager.Instance.GetGlobalMultiplier(Data).Multiplier);
    }
}
