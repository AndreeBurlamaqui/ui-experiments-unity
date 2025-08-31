using System.Collections.Generic;
using UnityEngine;

public class CollectingData
{
    [field: SerializeField] public ResourceGroup CollectingOn { get; private set; }
    private List<Worker> everyWorker = new();

    public CollectingData(ResourceGroup group, Worker workerPrefab)
    {
        CollectingOn = group;

        // Create first worker
        var firstWorker = Object.Instantiate(workerPrefab, ResourceManager.Instance.transform.position, Quaternion.identity);
        firstWorker.Initiate(group);
        everyWorker.Add(firstWorker);
    }

    public void Tick()
    {
        CollectingOn.Tick();

        for (int w = 0; w < everyWorker.Count; w++)
        {
            everyWorker[w].Tick();
        }
    }

    #region UPGRADES

    public void UpgradeExtractionSpeed()
    {

    }

    public void UpgradeWorkerCount()
    {

    }

    public void UpgradeWorkerFillSpeed()
    {

    }

    public void UpgradeWorkerCapacity()
    {

    }

    #endregion
}
