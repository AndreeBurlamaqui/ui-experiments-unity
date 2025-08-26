using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Nexus control what the player is collecting and with which workers
/// MAY NEED REFACTOR LATER
/// </summary>
public class Nexus : MonoBehaviour
{
    public static Nexus Instance;

    [SerializeField] private Worker workerPrefab;
    [SerializeField] private List<CollectingData> collectingResources = new();

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterAsResource(ResourceGroup group)
    {
        collectingResources.Add(new CollectingData(group, workerPrefab));
    }

    private void Update()
    {
        for(int r = 0; r < collectingResources.Count; r++)
        {
            collectingResources[r].Tick();
        }
    }
}
