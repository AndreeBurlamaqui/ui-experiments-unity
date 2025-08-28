using UnityEngine;

[CreateAssetMenu(fileName = "ResourceType", menuName = "Scriptable Objects/ResourceType")]
public class ResourceType : ScriptableObject
{
    [field: SerializeField] public string Title { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
}
