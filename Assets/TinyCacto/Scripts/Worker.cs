using LitMotion;
using LitMotion.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    [field: SerializeField] public CollectingState State { get; private set; }
    public enum CollectingState
    {
        IDLE,
        COLLECTING,
        DELIVERING,
        ON_PAUSE
    }

    [SerializeField] private List<ResourceAmount> backpack = new();

    [field: SerializeField] public WorkerData Data { get; private set; }
    [System.Serializable]
    public class WorkerData
    {
        public ResourceGroup resourceData;

        [Header("STATS")]
        [Tooltip("How long worker takes to reach destination. In seconds")] public float speed;
        [Tooltip("How much worker can carry. In units")] public float capacity;
        [Tooltip("How quickly it can collect. In seconds")] public float efficacy;

        public WorkerData(ResourceGroup group, float speed, float capacity, float efficacy)
        {
            resourceData = group;
            this.speed = speed;
            this.capacity = capacity;
            this.efficacy = efficacy;
        }
    }

    public void Initiate(WorkerData data)
    {
        Data = data;
        transform.position = Nexus.Instance.transform.position;
        SetState(CollectingState.COLLECTING);
    }

    public void SetState(CollectingState newState)
    {
        Debug.Log($"Changing worker state from {State} to {newState}");
        State = newState;

        switch (State)
        {
            case CollectingState.DELIVERING:
                OnDelivering();
                break;

            case CollectingState.COLLECTING:
                OnCollecting();
                break;
        }
    }

    public void Tick()
    {
        switch (State)
        {
            case CollectingState.ON_PAUSE or CollectingState.IDLE:
                // If it's on pause, check which should be the next state
                if (backpack.Count <= 0)
                {
                    // If backpack is empty, should collect
                    SetState(CollectingState.COLLECTING);
                }
                else
                {
                    // If backpack is filled, should deliver
                    SetState(CollectingState.DELIVERING);
                }
                break;
        }
    }

    private async void OnDelivering()
    {
        // How quickly should it go to nexus and then empty backpack
        var movement = LMotion.Create(transform.position, Nexus.Instance.transform.position, Data.speed)
            .BindToPosition(transform);
        await movement;

        // Empty backpack giving everything to nexus
        backpack.Clear();
        SetState(CollectingState.COLLECTING);
    }

    private async void OnCollecting()
    {
        Debug.Log($"Worker started movement to collect. Will take {Data.speed} seconds");
        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * 2;
        var collectPos = Data.resourceData.transform.position + offset;
        var movement = LMotion.Create(transform.position, collectPos, Data.speed)
            .BindToPosition(transform);
        await movement;

        Debug.Log($"Worker started collecting. Will take {Data.efficacy} seconds");
        var collect = LMotion.Create(0, 1, Data.efficacy)
            .RunWithoutBinding();
        await collect;

        // Based on efficacy, collect resource from group
        var collected = Data.resourceData.Collect();
        backpack.AddRange(collected);
        SetState(CollectingState.DELIVERING);
    }
}
