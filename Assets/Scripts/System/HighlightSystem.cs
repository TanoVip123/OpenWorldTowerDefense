using System.Collections.Generic;
using UnityEngine;

public class HighlightSystem : IUpdatableSystem, IGameSystem
{
    private World _world;
    public void Initialize(World world)
    {
        _world = world;
        _world.AddSystem(this);
        Debug.Log("HighlightSystem initialized");
    }

    public void Shutdown() => _world.highlightEntities.Clear();

    public void Update(float deltaTime)
    {
        //Debug.Log("HighlightSystem Update called");
        if (_world.Events.GetEvents(out List<HighlightEntitiesEvent> highlightEvents) && highlightEvents.Count > 0)
        {
            Debug.Log($"HighlightSystem received {highlightEvents.Count} HighlightEntitiesEvent(s)");
            foreach (HighlightEntitiesEvent highlightEvent in highlightEvents)
            {
                IReadOnlyList<EntityID> entityIDs = highlightEvent.EntityIDs;

                // unhighlight previously highlighted entities
                foreach (EntityID entityId in _world.highlightEntities)
                {
                    // Logic to unhighlight the entity, e.g., remove highlight component or change material
                    if (GameBootstrap.World.GetEntityObject(entityId, out GameObject entityObject))
                    {
                        if (entityObject.TryGetComponent(out HighlightDisplay display))
                        {
                            display.SetHighlight(false);
                        }
                    }
                }
                _world.highlightEntities = new List<EntityID>(entityIDs);
                Debug.Log($"HighlightSystem received HighlightEntitiesEvent for EntityIDs: {string.Join(", ", entityIDs)}");
                foreach (EntityID entityID in _world.highlightEntities)
                {
                    // Logic to highlight the entity, e.g., add highlight component or change material
                    if (GameBootstrap.World.GetEntityObject(entityID, out GameObject entityObject))
                    {
                        if (entityObject.TryGetComponent(out HighlightDisplay display))
                        {
                            display.SetHighlight(true);
                        }
                    }
                }
            }
        }
    }
}

