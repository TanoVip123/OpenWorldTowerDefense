using System.Collections.Generic;
using UnityEngine;

public class SelectSystem : IUpdatableSystem, IBaseGameSystem
{
    // Public for now for easy debugging

    private World _world;

    public void Initialize(World world)
    {
        _world = world;
        _world.AddSystem(this);
        // Initialization logic for the SelectSystem, if needed
    }

    public void Shutdown() => _world.selectedEntities.Clear();// Cleanup logic for the SelectSystem, if needed

    // For now, only allowed to select one entity and selection cleared when selecting another entity.
    public void Update(float deltaTime)
    {
        // Debug.Log("SelectSystem Update called");
        if (_world.Commands.GetCommands(out List<SelectCommand> _selectCommands) && _selectCommands.Count > 0)
        {
            foreach (SelectCommand command in _selectCommands)
            {
                // For simplicity, only one entity can be selected at a time.
                _world.selectedEntities.Clear();
                if (command.TargetEntityID is not EntityID targetEntityID)
                {
                    Debug.Log("Remove all selected target");
                    _world.Events.AddEvent(new HighlightEntitiesEvent(_world.selectedEntities));
                    continue;
                }

                _world.selectedEntities.Add(targetEntityID);

                Debug.Log($"Processed Event: Selected EntityID: {targetEntityID}");

                _world.Events.AddEvent(new HighlightEntitiesEvent(_world.selectedEntities));
            }
        }
    }
}
