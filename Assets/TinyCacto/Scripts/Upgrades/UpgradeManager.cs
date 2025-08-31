using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [SerializeField] private GlobalMultiplier[] multipliers;
    private Dictionary<UpgradeData, GlobalMultiplier> cachedMultipliers = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        for(int m = 0; m < multipliers.Length; m++)
        {
            var globalData = multipliers[m];
            globalData.Initiate();
            cachedMultipliers.Add(globalData.Data, globalData);
        }
    }


    public UpgradeInstance GetGlobalMultiplier(UpgradeData targetData)
    {
        if(!cachedMultipliers.TryGetValue(targetData, out var cachedGlobalMultiplier))
        {
            // If not found, we need to create it
            for (int m = 0; m < multipliers.Length; m++)
            {
                var globalData = multipliers[m];
                if (targetData == globalData.Data)
                {
                    cachedMultipliers.Add(globalData.Data, globalData);
                    cachedGlobalMultiplier = globalData;
                    break;
                }
            }
        }

        if (cachedGlobalMultiplier != null)
        {
            return cachedGlobalMultiplier.Multiplier;
        }
        else
        {
            // By design every upgrade should have a global version
            // If we reach here, something went wrong and need bugfix
            Debug.LogError("Couldn't found requested global multiplier of upgrade type " + targetData.Title);
            return null;
        }
    }

    [System.Serializable]
    class GlobalMultiplier
    {
        public UpgradeData Data;
        public UpgradeInstance Multiplier;

        public void Initiate()
        {
            Multiplier = new UpgradeInstance(Data);

            // TODO: Get current global value from save file
        }
    }
}
