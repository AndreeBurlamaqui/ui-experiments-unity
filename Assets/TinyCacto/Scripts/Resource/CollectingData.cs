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
        var firstWorker = Object.Instantiate(workerPrefab, Nexus.Instance.transform.position, Quaternion.identity);
        firstWorker.Initiate(new Worker.WorkerData(CollectingOn, 5, 5, 5));
        everyWorker.Add(firstWorker);
    }

    public void Tick()
    {
        for(int w = 0; w < everyWorker.Count; w++)
        {
            everyWorker[w].Tick();
        }
    }
}
