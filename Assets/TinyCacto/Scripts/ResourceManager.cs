using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central hub controlling which resource groups are owned and 
/// which workers are assigned to collect from them.
/// </summary>
public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    [SerializeField] private Worker workerPrefab;
    [SerializeField] private List<CollectingData> activeCollecting = new();
    private readonly Dictionary<ResourceGroup, int> indexOfActive = new Dictionary<ResourceGroup, int>(128);

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        for (int r = 0; r < activeCollecting.Count; r++)
        {
            activeCollecting[r].Tick();
        }
    }

    /// <summary>Returns true if the group is already owned/registered.</summary>
    public bool IsGroupOwned(ResourceGroup group) => group != null && indexOfActive.ContainsKey(group);

    public void RegisterAsResource(ResourceGroup group)
    {
        if (group == null || indexOfActive.ContainsKey(group))
        {
            return;
        }

        int i = activeCollecting.Count;
        activeCollecting.Add(new CollectingData(group, workerPrefab));
        indexOfActive.Add(group, i);
    }

    /// <summary>
    /// Tries to get the current CollectingData for a group without allocations.
    /// </summary>
    public bool TryGetCollection(ResourceGroup group, out CollectingData data)
    {
        data = null;
        if (group != null && indexOfActive.TryGetValue(group, out int i))
        {
            data = activeCollecting[i];
        }

        return data != null;
    }
}
