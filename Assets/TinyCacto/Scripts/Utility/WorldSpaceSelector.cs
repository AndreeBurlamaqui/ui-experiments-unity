using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class WorldSpaceSelector : MonoBehaviour
{
    public static WorldSpaceSelector Instance { get; private set; }

    [SerializeField] private Camera mainCamera;

    // Cache of colliders to clickable objects
    private Dictionary<Collider, IWorldSpaceSelectable> clickableCache = new Dictionary<Collider, IWorldSpaceSelectable>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (mainCamera == null)
            mainCamera = Camera.main;
    }


    private void Update()
    {
        if (IsPointerOverUI())
            return;

        // Mouse click
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryClickObject(Mouse.current.position.ReadValue());
        }

        // Touch tap
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            TryClickObject(touchPosition);
        }
    }

    private void TryClickObject(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            if (!clickableCache.TryGetValue(hit.collider, out var clickable))
            {
                clickable = hit.collider.GetComponent<IWorldSpaceSelectable>();
                if (clickable != null)
                    clickableCache[hit.collider] = clickable;
            }

            clickable?.OnSelect();
        }
    }

    /// <summary>
    /// Detects if the mouse is over a UI element.
    /// </summary>
    private bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}

public interface IWorldSpaceSelectable
{
    public void OnSelect();
}