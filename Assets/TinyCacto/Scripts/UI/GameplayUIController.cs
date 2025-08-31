using System;
using UnityEngine;
using UnityEngine.UIElements;

public class GameplayUIController : MonoBehaviour
{
    public static GameplayUIController Instance;

    [Header("RESOURCE GROUPS")]
    [SerializeField] private VisualTreeAsset resourcePrefab;
    [SerializeField] private VisualTreeAsset upgradePrefab;
    private VisualElement groupRoot, resourceRoot, upgradeRoot;
    private VisualElement[] resourceInstances, upgradeInstances;

    private VisualElement root;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Build all templates so we can fill them later
        root = GetComponent<UIDocument>().rootVisualElement;
        groupRoot = root.Q<VisualElement>("GroupRoot");
        resourceRoot = root.Q<VisualElement>("ResourcesRoot");
        int curDatas = resourceRoot.childCount;
        resourceInstances = new VisualElement[curDatas]; // By design this is the only amount we'll allow
        for(int d = 0; d < curDatas; d++)
        {
            resourceInstances[d] = resourceRoot.ElementAt(d);
        }

        upgradeRoot = root.Q<VisualElement>("UpgradeRoot");

        var closeButton = root.Q<Button>("ResourcesCloseButton");
        closeButton.clicked += OnCloseResourceUI;

        SwitchResourceUIState(false);
    }

    public void SwitchResourceUIState(bool newState)
    {
        Debug.Log("Switching resource ui state to " + newState);
        // TODO: Based on current state, animate it or not
        groupRoot.SetEnabled(newState);
        if (newState)
        {
            groupRoot.style.display = DisplayStyle.Flex;
            //groupRoot.Focus();
        }
        else
        {
            groupRoot.style.display = DisplayStyle.None;
        }
    }

    public void DisplayResourceUI(ResourceGroup group)
    {
        SwitchResourceUIState(true);
        var resourcesDatas = group.Resources;
        for (int i = 0; i < resourceInstances.Length; i++)
        {

            var resourceUI = resourceInstances[i];
            if (i >= resourcesDatas.Count)
            {
                // Deactivate this instance
                resourceUI.SetEnabled(false);
                resourceUI.style.display = DisplayStyle.None;
                continue;
            }

            resourceUI.SetEnabled(true);
            resourceUI.style.display = DisplayStyle.Flex;
            resourceUI.dataSource = resourcesDatas[i];
            Debug.Log("Updating resource ui " + resourceUI.name);
        }
    }

    private void OnCloseResourceUI()
    {
        SwitchResourceUIState(false);
    }
}
