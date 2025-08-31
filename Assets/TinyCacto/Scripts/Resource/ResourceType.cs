using UnityEngine;

[CreateAssetMenu(fileName = "ResourceType", menuName = "Tiny Cacto/Resource Type")]
public class ResourceType : ScriptableObject
{
    [field: SerializeField] public string Title { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
}
