using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
    }

    private IEnumerator Start()
    {
        // Load every upgrades by addressables label
        var loader = Addressables.LoadAssetsAsync<UpgradeData>("upgrades");
        yield return loader;

        var upgradesLoadedCount = loader.Result.Count;
        multipliers = new GlobalMultiplier[upgradesLoadedCount];
        for(int m = 0; m < upgradesLoadedCount; m++)
        {
            var globalData = loader.Result[m];
            multipliers[m] = new GlobalMultiplier(globalData);
            cachedMultipliers.Add(globalData, multipliers[m]);
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

        public GlobalMultiplier(UpgradeData data)
        {
            Data = data;
            Multiplier = new UpgradeInstance(Data);

            // TODO: Get current global value from save file
        }
    }
}
