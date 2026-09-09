using UnityEngine;
public class EntityView : MonoBehaviour
{
    private EntityID? _entityID;
    public EntityID EntityID { get => _entityID.Value; private set => _entityID = value; }
    public bool ComponentRegistered = false;

    public void RegisterEntity() => _entityID = GameBootstrap.World.RegisterEntity(gameObject);

    public void RegisterComponents()
    {
        if (_entityID.HasValue)
        {
            foreach (IComponentAuthor author in GetComponents<IComponentAuthor>())
            {
                author.RegisterToWorld(GameBootstrap.World, _entityID.Value);
            }
            ComponentRegistered = true;
        }
        else
        {
            Debug.LogError("EntityID is null. Cannot register components.");
        }

    }
    public void Start()
    {
        if (!_entityID.HasValue)
        {
            RegisterEntity();
        }
        if (!ComponentRegistered)
        {
            RegisterComponents();
        }

        // Need to traverse through children to find all PhysicalBridge components and register them to the world as well.
    }
}
