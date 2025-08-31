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

    [SerializeField] private List<CollectedResource> backpack = new();

    [Header("STATS")]
    [SerializeField] private UpgradeInstance moveSpeed;
    [SerializeField] private UpgradeInstance boatCapacity;
    [SerializeField] private UpgradeInstance loadSpeed;

    private ResourceGroup attachedIsland;

    public void Initiate(ResourceGroup _attachedIsland)
    {
        attachedIsland = _attachedIsland;
        transform.position = ResourceManager.Instance.transform.position;
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
        var speed = moveSpeed.GetTotalPower();
        var movement = LMotion.Create(transform.position, ResourceManager.Instance.transform.position, speed)
            .BindToPosition(transform);
        await movement;

        // Empty backpack giving everything to nexus
        backpack.Clear();
        SetState(CollectingState.COLLECTING);
    }

    private async void OnCollecting()
    {
        var speed = moveSpeed.GetTotalPower();
        Debug.Log($"Worker started movement to collect. Will take {speed} seconds");
        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * 2;
        var collectPos = attachedIsland.transform.position + offset;
        var movement = LMotion.Create(transform.position, collectPos, speed)
            .BindToPosition(transform);
        await movement;

        var efficacy = loadSpeed.GetTotalPower();
        Debug.Log($"Worker started collecting. Will take {efficacy} seconds");
        var collect = LMotion.Create(0, 1, efficacy)
            .RunWithoutBinding();
        await collect;

        // Based on efficacy, collect resource from group
        var capacity = boatCapacity.GetTotalPower();
        var collected = attachedIsland.Collect(capacity);
        backpack.AddRange(collected);
        SetState(CollectingState.DELIVERING);
    }
}
